using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using Framework.NodeSystem;
using NodeSystem.Editor;

namespace Framework.NodeEditorViews
{
    public class NodeEditorGraphView : BaseView
    {
        public Rect WindowSize { get; set; }

        private Dictionary<Node, NodeEditorNodeView> _nodeViews;
        private Vector2 _scrollPosition;

        protected override void OnInitialize()
        {
            _nodeViews = new Dictionary<Node, NodeEditorNodeView>();

            GraphHelper.NodeAdded += AddNodeView;
            GraphHelper.NodeRemoved += RemoveNodeView;
        }

        public void AddNodeView(Node node)
        {
            bool containsNode = _nodeViews.ContainsKey(node);

            Assert.IsFalse(containsNode);

            if (!containsNode)
            {
                var nodeView = new NodeEditorNodeView(node, _nodeViews.Count);
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
                nodeView.Dispose();

                _nodeViews.Remove(node);

                DebugEx.Log<NodeEditorGraphView>("Node was removed.");
            }
        }

        public void Clear()
        {
            DebugEx.Log<NodeEditorGraphView>("Clearing graph view...");

            _nodeViews.ToList().ForEach(x => RemoveNodeView(x.Key));

            Assert.IsTrue(_nodeViews.Count == 0);
        }

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
            if (GraphHelper == null)
                return;

            GraphHelper.Connections.ForEach(connection => NodeEditorHelper.DrawConnection(connection));
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

        protected override void OnDispose()
        {
            GraphHelper.NodeAdded -= AddNodeView;
            GraphHelper.NodeRemoved -= RemoveNodeView;
        }
    }
}
