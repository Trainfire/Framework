using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;
using Framework.NodeSystem;

namespace Framework.NodeEditor.Views
{
    public class NodeEditorGraphView
    {
        public event Action RunGraph;
        public event Action<Node> NodeSelected;
        public event Action<Node> NodeDeleted;
        public event Action<NodePin> MouseLeftClickedPin;
        public event Action<NodePin> MouseLeftReleasedOverPin;
        public event Action<NodePin> MouseMiddleClickedPin;
        public event Action<NodePin> MouseOverPin;
        public event Action MouseReleased;

        public NodeGraphHelper GraphHelper { get; set; }
        public Rect WindowSize { get; set; }

        private EditorInputListener _inputListener;
        private Dictionary<Node, NodeView> _nodeViews;
        private Node _selectedNode;

        private Vector2 _scrollPosition;

        public NodeEditorGraphView()
        {
            _inputListener = new EditorInputListener();
            _inputListener.MouseDown += InputListener_MouseDown;
            _inputListener.MouseUp += InputListener_MouseLeftReleased;
            _inputListener.DeletePressed += InputListener_DeletePressed;

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
                nodeView.NodeDeleted += NodeView_Deleted;

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
                nodeView.NodeDeleted -= NodeView_Deleted;
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
            NodeSelected.InvokeSafe(nodeView.Node);
        }

        void NodeView_Deleted(NodeView nodeView)
        {
            _selectedNode = null;
            NodeDeleted.InvokeSafe(nodeView.Node);
        }

        void InputListener_MouseLeftReleased(EditorMouseEvent mouseEvent)
        {
            GetAnyPinUnderMouse((pin) =>
            {
                DebugEx.Log<NodeEditorGraphView>("Mouse released over Pin {0}. (Node ID: {1}) (Button: {2})", pin.Name, pin.Node.ID, mouseEvent.Button);

                if (mouseEvent.IsLeftMouse)
                    MouseLeftReleasedOverPin.InvokeSafe(pin);
            });
            MouseReleased.InvokeSafe();
        }

        void InputListener_MouseDown(EditorMouseEvent mouseEvent)
        {
            GetAnyPinUnderMouse((pin) =>
            {
                DebugEx.Log<NodeEditorGraphView>("Pin {0} was clicked. (Node ID: {1}) (Button: {2})", pin.Name, pin.Node.ID, mouseEvent.Button);

                if (mouseEvent.IsLeftMouse)
                    MouseLeftClickedPin.InvokeSafe(pin);

                if (mouseEvent.IsMiddleMouse)
                    MouseMiddleClickedPin.InvokeSafe(pin);
            });
        }

        void InputListener_DeletePressed()
        {
            DebugEx.Log<NodeEditorGraphView>("Delete pressed.");
        }
        #endregion

        #region Draw
        public void Draw()
        {
            _inputListener.ProcessEvents();

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

        NodePin GetAnyPinUnderMouse(Action<NodePin> OnPinExists = null)
        {
            NodePin outPin = null;

            var pinViews = _nodeViews.Values.ToList();
            pinViews.ForEach(x => x.GetPinUnderMouse((pin) =>
            {
                outPin = pin;
                OnPinExists.InvokeSafe(pin);
            }));

            MouseOverPin.InvokeSafe(outPin);

            return outPin;
        }
    }
}
