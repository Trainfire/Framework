using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;
using NodeSystem.Editor;

namespace NodeSystem
{
    public class NodeGraph
    {
        public event Action<NodeGraph, NodeGraphData> PostLoad;
        public event Action<NodeGraph> PreUnload;
        public event Action<NodeGraph> PostUnload;
        public event Action<NodeGraph> Edited;

        public Node Selection { get; private set; }
        public NodeGraphState State { get; private set; }

        public event Action<Node> NodeSelected;
        public event Action<Node> NodeAdded;
        public event Action<Node> NodeRemoved;

        public NodeGraphHelper Helper { get; private set; }
        public List<Node> Nodes { get; private set; }
        public List<NodeConnection> Connections { get; private set; }

        public NodeGraph()
        {
            Nodes = new List<Node>();
            Connections = new List<NodeConnection>();
            Helper = new NodeGraphHelper(this);
            State = new NodeGraphState(this);
        }

        public void Load(NodeGraphData graphData)
        {
            NodeEditor.Logger.Log<NodeGraph>("Initializing...");

            Unload();

            NodeEditor.Logger.Log<NodeGraph>("Reading from graph data...");

            // TODO: Find a nicer way to do this...
            var allNodes = graphData.Nodes.Concat(graphData.Constants.Cast<NodeData>()).ToList();
            allNodes.ForEach(nodeData =>
            {
                if (nodeData.GetType() == typeof(NodeConstantData))
                {
                    AddNode(nodeData as NodeConstantData);
                }
                else
                {
                    AddNode(nodeData);
                }
            });

            graphData.Connections.ForEach(connectionData => Connect(connectionData));

            if (PostLoad != null)
                PostLoad.Invoke(this, graphData);
        }

        public void Unload()
        {
            PreUnload.InvokeSafe(this);

            var nodesToClear = Nodes.ToList();
            nodesToClear.ForEach(x => RemoveNode(x));

            var connectionsToClear = Connections.ToList();
            Connections.ForEach(x => Disconnect(x));

            Selection = null;

            PostUnload.InvokeSafe(this);

            NodeEditor.Logger.Log<NodeGraph>("Graph cleared.");
        }

        public void AddNode(NodeConstantData constantData)
        {
            var constant = AddNode((NodeData)constantData) as NodeConstant;
            constant.Set(constantData);
        }

        public TNode AddNode<TNode>(string name = "") where TNode : Node
        {
            var nodeData = new NodeData();
            nodeData.Name = name == "" ? "Untitled Node" : name;
            nodeData.ClassType = typeof(TNode).ToString();
            nodeData.ID = Guid.NewGuid().ToString();
            nodeData.Position = new Vector2(50f, 50f);

            return AddNode(nodeData) as TNode;
        }

        public Node AddNode(NodeData nodeData)
        {
            var nodeType = Type.GetType(nodeData.ClassType);
            var node = Activator.CreateInstance(nodeType) as Node;
            node.Initialize(nodeData);

            if (!Nodes.Contains(node))
            {
                NodeEditor.Logger.Log<NodeGraph>("Registered node.");
                node.Destroyed += RemoveNode;
                node.Changed += Node_Changed;
                node.PinRemoved += Node_PinRemoved;

                Nodes.Add(node);
                NodeAdded.InvokeSafe(node);

                Edited.InvokeSafe(this);
            }

            return node;
        }

        public void RemoveNode(Node node)
        {
            Assert.IsTrue(Nodes.Contains(node), "Node Graph does not contain node.");

            if (Nodes.Contains(node))
            {
                NodeEditor.Logger.Log<NodeGraph>("Removed node.");
                node.Destroyed -= RemoveNode;
                node.Changed -= Node_Changed;
                node.PinRemoved -= Node_PinRemoved;
                Nodes.Remove(node);
                NodeRemoved.InvokeSafe(node);

                // TODO: Gracefully handle disconnections...
                var connections = Connections
                    .Where(x => x.StartPin.Node.ID == node.ID || x.EndPin.Node.ID == node.ID)
                    .ToList();

                connections.ForEach(x => Disconnect(x));

                Edited.InvokeSafe(this);
            }
        }

        /// <summary>
        /// Replaces an existing connection with a new connection. This will create a single state change to the graph.
        /// </summary>
        public void Replace(NodeConnection oldConnection, NodeConnection newConnection)
        {
            if (Connections.Contains(oldConnection))
            {
                NodeEditor.Logger.Log<NodeGraph>("Replacing a connection...");
                Connections.Remove(oldConnection);
                Connect(newConnection);
                NodeEditor.Logger.Log<NodeGraph>("Connection replaced.");
            }
            else
            {
                NodeEditor.Logger.LogWarning<NodeGraph>("Cannot replace connection as the old connection is not a part of this graph.");
            }
        }

        public void Connect(NodeConnectionData connectionData)
        {
            var startPin = Helper.GetPin(connectionData.SourceNodeId, connectionData.SourcePinId);
            var endPin = Helper.GetPin(connectionData.TargetNodeId, connectionData.TargetPinId);

            Connect(new NodeConnection(startPin, endPin));
        }

        public void Connect(NodeConnection connection)
        {
            bool hasConnection = Connections.Contains(connection);

            Assert.IsFalse(hasConnection);
            Assert.IsNotNull(connection.StartPin, "Attempted to connect two pins where the start pin was null.");
            Assert.IsNotNull(connection.EndPin, "Attempted to connect two pins where the end pin was null.");
            Assert.IsFalse(connection.StartPin == connection.EndPin, "Attempted to connect a pin to itself.");
            //Assert.IsFalse(connection.StartPin.WillPinConnectionCreateCircularDependency(connection.EndPin), "Pin connection would create a circular dependency!");

            NodeEditor.Logger.Log<NodeGraph>("Connected {0}:(1) to {2}:{3}", connection.StartNode.Name, connection.StartPin.Index, connection.EndNode.Name, connection.EndPin.Index);

            if (connection.StartPin != null && connection.EndPin != null)
            {
                if (connection.StartPin.IsInput() || connection.EndPin.IsOutput())
                {
                    // Input pins can never have multiple connections. Remove it.
                    var existingConnection = Helper.GetConnections(connection.StartPin);
                    if (existingConnection != null)
                        existingConnection.ForEach(x => Connections.Remove(x));
                }

                var startPinId = connection.StartPin.Index;
                var endPinId = connection.EndPin.Index;

                connection.StartPin.Connect(connection.EndPin);
                connection.EndPin.Connect(connection.StartPin);

                // Check if the pin has changed after connecting. This will happen for dynamic pin types.
                if (connection.StartPin.Node.Pins[startPinId] != connection.StartPin || connection.EndPin.Node.Pins[endPinId] != connection.EndPin)
                    connection = new NodeConnection(connection.StartNode.Pins[startPinId], connection.EndNode.Pins[endPinId]);

                Connections.Add(connection);

                Edited.InvokeSafe(this);
            }
        }

        public void Disconnect(NodeConnection connection)
        {
            bool containsConnection = Connections.Contains(connection);

            Assert.IsTrue(containsConnection);

            if (containsConnection)
            {
                Connections.Remove(connection);
                NodeEditor.Logger.Log<NodeGraph>("Disconnected {0} from {1}.", connection.StartPin.Node.Name, connection.EndPin.Node.Name);
                Edited.InvokeSafe(this);
            }
        }

        public void Disconnect(NodePin pin)
        {
            var connection = Connections
                .Where(x => x.StartNode == pin.Node && x.StartPin == pin || x.EndNode == pin.Node && x.EndPin == pin)
                .ToList();

            if (connection.Count != 0)
                connection.ForEach(x => Disconnect(x));
        }

        public void SetSelection(Node node)
        {
            if (node != null)
                Assert.IsTrue(Nodes.Contains(node));
            Selection = node;
            NodeSelected.InvokeSafe(Selection);
        }

        #region Callbacks
        void Node_Changed(Node node)
        {
            NodeEditor.Logger.Log<NodeGraph>("Node {0} changed.", node.Name);
            Edited.InvokeSafe(this);
        }

        void Node_PinRemoved(NodePin pin)
        {
            Disconnect(pin);
            Edited.InvokeSafe(this);
        }
        #endregion
    }
}
