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

        public NodeGraph()
        {
            Nodes = new List<Node>();
        }

        public void Initialize()
        {
            DebugEx.Log<NodeGraph>("Initialized.");
            Info = new NodeGraphInfo(this);
        }

        public void AddNode<TNode>(string name = "") where TNode : Node
        {
            var nodeData = new NodeData();
            nodeData.Name = name == "" ? "Untitled Node" : name;
            nodeData.ClassType = typeof(TNode).ToString();
            nodeData.ID = Guid.NewGuid().ToString();

            AddNode(nodeData);
        }

        public void AddNode(NodeData nodeData)
        {
            var nodeType = Type.GetType(nodeData.ClassType);
            var node = Activator.CreateInstance(nodeType) as Node;
            node.Initialize(nodeData);
            RegisterNode(node);
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
            }
        }

        public void RemoveAllNodes()
        {
            var nodesToClear = Nodes.ToList();
            nodesToClear.ForEach(x => RemoveNode(x));

            DebugEx.Log<NodeGraph>("Graph cleared.");
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
    }
}
