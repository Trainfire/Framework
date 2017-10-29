using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace Framework.NodeEditor
{
    [Serializable]
    public class NodeGraphConnection
    {
        public int SourceNodeID { get; set; }
        public int SourcePinIndex { get; set; }
        public int TargetNodeID { get; set; }
        public int TargetPinIndex { get; set; }

        public NodeGraphConnection(NodePin sourcePin, NodePin targetPin)
        {
            SourceNodeID = sourcePin.Node.ID;
            SourcePinIndex = sourcePin.Index;
            TargetNodeID = targetPin.Node.ID;
            TargetPinIndex = targetPin.Index;
        }
    }

    [ExecuteInEditMode]
    public class NodeGraph : MonoBehaviour
    {
        public event Action<NodeGraph> GraphDestroyed;
        public event Action<Node> NodeAdded;
        public event Action<Node> NodeRemoved;

        public NodeGraphInfo Info { get; private set; }
        public List<Node> Nodes { get; private set; }
        public List<NodeGraphConnection> Connections { get; private set; }
        
        [ExecuteInEditMode]
        void OnEnable()
        {
            
        }

        public void Initialize()
        {
            DebugEx.Log<NodeGraph>("Initialized.");

            if (Connections == null)
                Connections = new List<NodeGraphConnection>();

            // Clear list of nodes and rebuild tree.
            Nodes = new List<Node>();

            var attachedNodes = GetComponentsInChildren<Node>(true).ToList();
            attachedNodes.ForEach(x => RegisterNode(x));

            Info = new NodeGraphInfo(this);
        }

        [ExecuteInEditMode]
        void OnDestroy()
        {
            GraphDestroyed.InvokeSafe(this);
        }

        void RegisterNode(Node node)
        {
            if (!Nodes.Contains(node))
            {
                DebugEx.Log<NodeGraph>("Registered node.");
                node.Destroyed += RemoveNode;
                node.PinAdded += Node_PinAdded;

                Nodes.Add(node);

                node.Initialize();
            }
        }

        public void AddNode<TNode>(string name = "") where TNode : Node
        {
            name = name == "" ? "Untitled Node" : name;

            var node = new GameObject(name).AddComponent<TNode>();
            node.transform.SetParent(transform);

            RegisterNode(node);

            NodeAdded.InvokeSafe(node);
        }

        public void RemoveNode(Node node)
        {
            Assert.IsTrue(Nodes.Contains(node), "Node Graph does not contain node.");

            if (Nodes.Contains(node))
            {
                DebugEx.Log<NodeGraph>("Removed node.");
                node.Destroyed -= RemoveNode;
                node.PinAdded -= Node_PinAdded;
                Nodes.Remove(node);
                NodeRemoved.InvokeSafe(node);

                // TODO: Probably want to register an undo hereas the object is permanently destroy. RIP. :'(
                DestroyImmediate(node.gameObject);
            }
        }

        public void RemoveAllNodes()
        {
            var nodesToClear = Nodes.ToList();
            nodesToClear.ForEach(x => RemoveNode(x));

            DebugEx.Log<NodeGraph>("Graph cleared.");
        }

        public Node GetStartNode()
        {
            return Nodes.Find(node => node.GetType() == typeof(NodeEventOnStart));
        }

        void Node_PinAdded(NodePin pin)
        {
            pin.PinConnected += Pin_PinConnected;
        }

        void Pin_PinConnected(NodePin sourcePin, NodePin targetPin)
        {
            var connection = new NodeGraphConnection(sourcePin, targetPin);
            Connections.Add(connection);
            DebugEx.Log<NodeGraph>("Registered connection from {0} to {1}.", sourcePin.Name, targetPin.Name);
        }
    }
}
