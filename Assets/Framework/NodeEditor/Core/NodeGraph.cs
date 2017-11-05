using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace Framework.NodeEditor
{
    public class NodeGraph
    {
        public event Action<NodeGraph> GraphDestroyed;
        public event Action<Node> NodeAdded;
        public event Action<Node> NodeRemoved;

        public NodeGraphInfo Info { get; private set; }
        public List<Node> Nodes { get; private set; }
        public List<NodeConnection> Connections { get; private set; }

        public NodeGraph()
        {
            Nodes = new List<Node>();
            Connections = new List<NodeConnection>();
        }

        public void Initialize()
        {
            DebugEx.Log<NodeGraph>("Initialized.");
            Info = new NodeGraphInfo(this);
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
            RegisterNode(node);
            return node;
        }

        public void RemoveNode(Node node)
        {
            Assert.IsTrue(Nodes.Contains(node), "Node Graph does not contain node.");

            if (Nodes.Contains(node))
            {
                DebugEx.Log<NodeGraph>("Removed node.");
                node.Destroyed -= RemoveNode;
                Nodes.Remove(node);
                NodeRemoved.InvokeSafe(node);

                // TODO: Gracefully handle disconnections...
                var connections = Connections
                    .Where(x => x.SourceNodeId == node.ID || x.TargetNodeId == node.ID)
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

        public void Connect(NodeConnection connection)
        {
            //DebugEx.Log<NodeGraph>("Connected {0}:(1) to {2}:{3}", sourceNode.Name, sourcePin.ID, targetNode.Name, targetNode.ID);
            //Connect(sourceNode.ID, sourcePin.ID, targetNode.ID, targetPin.ID);

            bool containsConnection = Connections.Contains(connection);

            Assert.IsFalse(containsConnection);

            if (!containsConnection)
            {
                Connections.Add(connection);
                DebugEx.Log<NodeGraph>("Connected {0}:(1) to {2}:{3}", connection.SourceNodeId, connection.SourcePinId, connection.TargetNodeId, connection.TargetPinId);

                // TODO: Unsafe code.
                DebugEx.LogWarning<NodeGraph>("Running unsafe code...");

                return;

                var sourceNode = Nodes.Find(x => x.ID == connection.SourceNodeId);
                var targetNode = Nodes.Find(x => x.ID == connection.TargetNodeId);
                var sourcePin = sourceNode.Pins[connection.SourcePinId];
                var targetPin = targetNode.Pins[connection.TargetPinId];

                sourcePin.ConnectTo(targetPin);
            }
        }

        public void Disconnect(NodeConnection connection)
        {
            bool containsConnection = Connections.Contains(connection);

            Assert.IsTrue(containsConnection);

            if (containsConnection)
            {
                Connections.Remove(connection);
                DebugEx.Log<NodeGraph>("Disconnected {0} from {1}.", connection.SourceNodeId, connection.TargetNodeId);
            }
        }

        void RegisterNode(Node node)
        {
            if (!Nodes.Contains(node))
            {
                DebugEx.Log<NodeGraph>("Registered node.");
                node.Destroyed += RemoveNode;

                Nodes.Add(node);
                NodeAdded.InvokeSafe(node);
            }
        }

        public Node GetStartNode()
        {
            return Nodes.Find(node => node.GetType() == typeof(NodeEventOnStart));
        }

        public List<T> GetNodes<T>() where T : Node
        {
            return Nodes.OfType<T>().ToList();
        }
    }
}
