﻿using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System.Linq;
using System.Collections.Generic;

namespace Framework.NodeEditor
{
    public class NodeEditorGraphView
    {
        public bool GraphLoaded { get { return _graph != null; } }

        private NodeGraph _graph;
        private EditorInputListener _inputListener;

        private Dictionary<Node, NodeView> _nodeViews;

        public NodeEditorGraphView()
        {
            _inputListener = new EditorInputListener();
            _nodeViews = new Dictionary<Node, NodeView>();
        }

        void Unload(NodeGraph graph)
        {
            _nodeViews = new Dictionary<Node, NodeView>();

            if (graph == null)
                return;

            DebugEx.Log<NodeEditorGraphView>("Removing old graph...");

            graph.GraphDestroyed -= Unload;
            graph.NodeAdded -= OnGraphNodeAdded;
            graph.NodeRemoved -= OnGraphNodeRemoved;
        }

        public void Load(NodeGraph graph)
        {
            // Unload previous graph.
            Unload(_graph);

            Assert.IsNotNull(graph, "Attempted to load a null graph.");

            if (graph != null)
            {
                DebugEx.Log<NodeEditorGraphView>("Loading graph...");

                // Assign new graph.
                _graph = graph;
                _graph.GraphDestroyed += Unload;
                _graph.NodeAdded += OnGraphNodeAdded;
                _graph.NodeRemoved += OnGraphNodeRemoved;

                // Make new view.
                _graph.Nodes.ForEach((node) => AssignView(node));
            }
        }

        void AssignView(Node node)
        {
            var nodeView = new NodeView(node);
            nodeView.NodeSelected += OnViewSelected;
            nodeView.NodeDeleted += OnViewDeleted;

            _nodeViews.Add(node, nodeView);
        }

        #region Callbacks
        void OnViewSelected(NodeView nodeView)
        {
            Selection.activeGameObject = nodeView.Node.gameObject;
        }

        void OnViewDeleted(NodeView nodeView)
        {
            _graph.RemoveNode(nodeView.Node);
        }

        void OnGraphNodeAdded(Node node)
        {
            DebugEx.Log<NodeEditorGraphView>("Node was added.");

            bool containsNode = _nodeViews.ContainsKey(node);

            Assert.IsFalse(containsNode);

            if (!containsNode)
                AssignView(node);
        }

        void OnGraphNodeRemoved(Node node)
        {
            DebugEx.Log<NodeEditorGraphView>("Node was removed.");

            bool containsNode = _nodeViews.ContainsKey(node);

            Assert.IsTrue(containsNode);

            if (containsNode)
            {
                var nodeView = _nodeViews[node];
                nodeView.NodeSelected -= OnViewSelected;
                nodeView.Destroy();

                _nodeViews.Remove(node);
            }
        }
        #endregion

        #region Draw
        public void Draw()
        {
            _inputListener.ProcessEvents();

            if (!GraphLoaded)
                GUILayout.Label("Select a gameobject with a NodeGraph component to view a graph.");

            DrawDebug();

            // Draw nodes.
            _nodeViews.Values.ToList().ForEach(x => x.Draw());
        }

        void DrawDebug()
        {
            GUILayout.BeginArea(new Rect(new Vector2(0f, Screen.height - 100f), new Vector2(400f, 100f)));

            DrawHeader("Graph Info");

            if (_graph == null)
            {
                DrawField("No graph loaded");
            }
            else
            {
                DrawField("Node Count", _graph.Nodes.Count);
                DrawField("Node Views", _nodeViews.Count);
                DrawField("Mouse Pos", _inputListener.MousePosition);
            }

            GUILayout.EndArea();
        }

        void DrawHeader(string label)
        {
            var guiStyle = new GUIStyle();
            guiStyle.fontStyle = FontStyle.Bold;

            GUILayout.Label(label, guiStyle);
        }

        void DrawField(string label, object value = null)
        {
            var str = value != null ? string.Format("{0}: {1}", label, value) : label;
            GUILayout.Label(str);
        }
        #endregion
    }
}