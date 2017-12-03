﻿using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Linq;
using System.Collections.Generic;
using NodeSystem;

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

            GraphHelper.Connections.ForEach(connection =>
            {
                Assert.IsTrue(HasNodeView(connection.StartNode), "Could not find a view for the start node of a connection.");
                Assert.IsTrue(HasNodeView(connection.EndNode), "Could not find a view for the end node of a connection.");

                var startPinView = HasNodeView(connection.StartNode) ? _nodeViews[connection.StartNode].GetPinViewData(connection.StartPin) : null;
                var endPinView = HasNodeView(connection.EndNode) ? _nodeViews[connection.EndNode].GetPinViewData(connection.EndPin) : null;

                Assert.IsNotNull(endPinView, "Failed to find a pin view for the start pin on a connection.");
                Assert.IsNotNull(endPinView, "Failed to find a pin view for the end pin on a connection.");

                NodeEditorConnectionView.DrawConnection(startPinView, endPinView);
            });
        }

        public bool HasNodeView(Node node)
        {
            return _nodeViews.ContainsKey(node);
        }

        public NodeEditorNodeView GetNodeViewUnderMouse(Action<NodeEditorNodeView> callback = null)
        {
            NodeEditorNodeView nodeView = null;

            nodeView = _nodeViews
                .Values
                .Where(x => x.Rect.Contains(InputListener.MousePosition))
                .FirstOrDefault();

            callback.InvokeSafe(nodeView);

            return nodeView;
        }

        public NodeEditorPinView GetPinViewUnderMouse(Action<NodeEditorPinView> OnPinExists = null)
        {
            NodeEditorPinView outPinView = null;

            var nodeViews = _nodeViews.Values.ToList();
            nodeViews.ForEach(nodeView => nodeView.GetPinUnderMouse((pinView) =>
            {
                outPinView = pinView;
                OnPinExists.InvokeSafe(pinView);
            }));

            return outPinView;
        }

        protected override void OnDispose()
        {
            GraphHelper.NodeAdded -= AddNodeView;
            GraphHelper.NodeRemoved -= RemoveNodeView;
        }
    }
}
