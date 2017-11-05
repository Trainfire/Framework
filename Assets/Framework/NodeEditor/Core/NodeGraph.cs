using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace Framework.NodeSystem
{
    public class NodeConnection
    {
        public NodePin StartPin { get; private set; }
        public NodePin EndPin { get; private set; }
        public Node StartNode { get { return StartPin.Node; } }
        public Node EndNode { get { return EndPin.Node; } }

        public NodeConnection(NodePin startPin, NodePin endPin)
        {
            StartPin = startPin;
            EndPin = endPin;
        }
    }

    public class NodeGraph
    {
        public event Action<NodeGraph> GraphDestroyed;
        public event Action<NodeGraph> StateChanged;
        public event Action<Node> NodeAdded;
        public event Action<Node> NodeRemoved;

        public NodeGraphHelper Helper { get; private set; }
        public List<Node> Nodes { get; private set; }
        public List<NodeConnection> Connections { get; private set; }
        public bool IsDirty { get; private set; }

        public NodeGraph()
        {
            Nodes = new List<Node>();
            Connections = new List<NodeConnection>();
            Helper = new NodeGraphHelper(this);
        }

        public void SetData(NodeGraphData graphData)
        {
            DebugEx.Log<NodeGraph>("Initializing...");

            DebugEx.Log<NodeGraph>("Reading from graph data...");

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

            return AddNode(nodeData) as TNode;
        }

        public Node AddNode(NodeData nodeData)
        {
            var nodeType = Type.GetType(nodeData.ClassType);
            var node = Activator.CreateInstance(nodeType) as Node;
            node.Initialize(nodeData);

            if (!Nodes.Contains(node))
            {
                DebugEx.Log<NodeGraph>("Registered node.");
                node.Destroyed += RemoveNode;
                node.PinRemoved += Disconnect;

                Nodes.Add(node);
                NodeAdded.InvokeSafe(node);

                StateChanged.InvokeSafe(this);
            }

            return node;
        }

        public void RemoveNode(Node node)
        {
            Assert.IsTrue(Nodes.Contains(node), "Node Graph does not contain node.");

            if (Nodes.Contains(node))
            {
                DebugEx.Log<NodeGraph>("Removed node.");
                node.Destroyed -= RemoveNode;
                node.PinRemoved -= Disconnect;
                Nodes.Remove(node);
                NodeRemoved.InvokeSafe(node);

                // TODO: Gracefully handle disconnections...
                var connections = Connections
                    .Where(x => x.StartPin.Node.ID == node.ID || x.EndPin.Node.ID == node.ID)
                    .ToList();

                connections.ForEach(x => Disconnect(x));
            }
        }

        public void Clear()
        {
            var nodesToClear = Nodes.ToList();
            nodesToClear.ForEach(x => RemoveNode(x));

            var connectionsToClear = Connections.ToList();
            Connections.ForEach(x => Disconnect(x));

            DebugEx.Log<NodeGraph>("Graph cleared.");
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
            Assert.IsFalse(connection.StartPin.WillPinConnectionCreateCircularDependency(connection.EndPin), "Pin connection would create a circular dependency!");

            Connections.Add(connection);

            DebugEx.Log<NodeGraph>("Connected {0}:(1) to {2}:{3}", connection.StartNode.Name, connection.StartPin.Index, connection.EndNode.Name, connection.EndPin.Index);

            if (connection.StartPin != null && connection.EndPin != null)
                connection.StartPin.ConnectTo(connection.EndPin);
        }

        public void Disconnect(NodeConnection connection)
        {
            bool containsConnection = Connections.Contains(connection);

            Assert.IsTrue(containsConnection);

            if (containsConnection)
            {
                Connections.Remove(connection);
                StateChanged.InvokeSafe(this);
                DebugEx.Log<NodeGraph>("Disconnected {0} from {1}.", connection.StartPin.Node.Name, connection.EndPin.Node.Name);
            }
        }

        public void Disconnect(NodePin pin)
        {
            var connection = Connections
                .Where(x => x.StartNode == pin.Node && x.StartPin == pin || x.EndNode == pin.Node && x.EndPin == pin)
                .FirstOrDefault();

            if (connection != null)
                Disconnect(connection);
        }

        public NodeGraphData GetData()
        {
            DebugEx.Log<NodeGraph>("Serializing graph state...");

            var data = new NodeGraphData();

            // TODO: Find a nicer way to do this...
            Nodes.ForEach(node =>
            {
                if (node.GetType() == typeof(NodeConstant))
                {
                    data.Constants.Add(NodeConstantData.Convert(node as NodeConstant));
                }
                else
                {
                    data.Nodes.Add(NodeData.Convert(node));
                }
            });

            Connections.ForEach(connection =>
            {
                data.Connections.Add(new NodeConnectionData(connection.StartPin, connection.EndPin));
            });

            return data;
        }
    }
}
