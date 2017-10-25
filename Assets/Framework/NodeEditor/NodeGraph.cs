using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace Framework.NodeEditor
{
    [ExecuteInEditMode]
    public class NodeGraph : MonoBehaviour
    {
        public event Action<NodeGraph> GraphDestroyed;
        public event Action<Node> NodeAdded;
        public event Action<Node> NodeRemoved;

        public NodeGraphInfo Info { get; private set; }
        public List<Node> Nodes { get; private set; }
        
        [ExecuteInEditMode]
        void OnEnable()
        {
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
                Nodes.Add(node);
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
    }
}
