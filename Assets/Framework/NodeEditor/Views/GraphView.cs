using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using Framework.NodeSystem;

namespace Framework.NodeEditor.Views
{
    public class NodeEditorGraphView : BaseView
    {
        private NodeGraphHelper _graphHelper;
        public NodeGraphHelper GraphHelper
        {
            set
            {
                if (_graphHelper != null)
                {
                    _graphHelper.NodeAdded -= AddNodeView;
                    _graphHelper.NodeRemoved -= RemoveNodeView;
                }

                _graphHelper = value;

                _graphHelper.NodeAdded += AddNodeView;
                _graphHelper.NodeRemoved += RemoveNodeView;
            }
        }

        public Rect WindowSize { get; set; }

        private Dictionary<Node, NodeView> _nodeViews;
        private Node _selectedNode;

        private Vector2 _scrollPosition;

        public NodeEditorGraphView()
        {
            _nodeViews = new Dictionary<Node, NodeView>();
        }

        public void AddNodeView(Node node)
        {
            bool containsNode = _nodeViews.ContainsKey(node);

            Assert.IsFalse(containsNode);

            if (!containsNode)
            {
                var nodeView = new NodeView(node, _nodeViews.Count);
                nodeView.NodeSelected += NodeView_Selected;

                _nodeViews.Add(node, nodeView);
            }
        }

        public void RemoveNodeView(Node node)
        {
            bool containsNode = _nodeViews.ContainsKey(node);

            Assert.IsTrue(containsNode);

            if (containsNode)
            {
                var nodeView = _nodeViews[node];
                nodeView.NodeSelected -= NodeView_Selected;
                nodeView.Destroy();

                _nodeViews.Remove(node);

                DebugEx.Log<NodeEditorGraphView>("Node was removed.");
            }
        }

        public void Clear()
        {
            DebugEx.Log<NodeEditorGraphView>("Clearing graph view...");

            _nodeViews.ToList().ForEach(x => RemoveNodeView(x.Key));
            GraphHelper = null;

            Assert.IsTrue(_nodeViews.Count == 0);
        }

        #region Callbacks
        void NodeView_Selected(NodeView nodeView)
        {
            _selectedNode = nodeView.Node;
        }
        #endregion

        #region Draw
        protected override void OnDraw()
        {
            _scrollPosition = GUI.BeginScrollView(WindowSize, _scrollPosition, new Rect(0, 0, 2000f, 0f));
            DrawNodes();
            DrawConnections();
            GUI.EndScrollView();
        }

        public void DrawNodes()
        {
            _nodeViews.Values.ToList().ForEach(x => x.Draw(_scrollPosition));
        }

        public void DrawConnections()
        {
            if (_graphHelper == null)
                return;

            _graphHelper.Connections.ForEach(connection => NodeEditorHelper.DrawConnection(connection));
        }
        #endregion

        public Node GetNodeUnderMouse(Action<Node> callback = null)
        {
            var node = _nodeViews.Values
                .Where(x => x.Rect.Contains(InputListener.MousePosition))
                .Select(x => x.Node)
                .FirstOrDefault();

            callback.InvokeSafe(node);

            return node;
        }

        public NodePin GetAnyPinUnderMouse(Action<NodePin> OnPinExists = null)
        {
            NodePin outPin = null;

            var pinViews = _nodeViews.Values.ToList();
            pinViews.ForEach(x => x.GetPinUnderMouse((pin) =>
            {
                outPin = pin;
                OnPinExists.InvokeSafe(pin);
            }));

            return outPin;
        }
    }
}
