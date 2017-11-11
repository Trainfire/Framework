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

            _scrollPosition = GUI.BeginScrollView(new Rect(0, 0, WindowSize.width, WindowSize.height), _scrollPosition, new Rect(0, 0, 2000f, 2000f), true, true);

            DrawNodes();
            DrawConnections();

            GUI.EndScrollView();

            DrawDebug();
        }

        void DrawNodes()
        {
            _nodeViews.Values.ToList().ForEach(x => x.Draw(_scrollPosition));
        }

        void DrawConnections()
        {
            if (GraphHelper == null)
                return;

            GraphHelper.Connections.ForEach(connection =>
            {
                Handles.BeginGUI();
                Handles.DrawLine(DrawPinConnectionHandle(connection.StartPin), DrawPinConnectionHandle(connection.EndPin));
                Handles.EndGUI();
            });
        }

        Vector2 DrawPinConnectionHandle(NodePin pin)
        {
            Vector2 position = pin.ScreenPosition;

            // Adjust for pin position using magic numbers. ;)
            position.x += pin.Node.IsOutputPin(pin) ? pin.LocalRect.width : -15f;
            position.y += pin.LocalRect.height * 0.5f;

            // Draw handle.
            const float size = 10f;
            var handlePosition = position - new Vector2(size * 0.5f, size * 0.5f);
            var rect = new Rect(handlePosition, new Vector2(size, size));
            GUI.Box(rect, "");

            return position;
        }

        void DrawDebug()
        {
            GUILayout.BeginArea(new Rect(new Vector2(0f, Screen.height - 120f), new Vector2(400f, 1000f)));

            DrawHeader("Graph Info");

            if (GraphHelper == null)
            {
                DrawField("No graph loaded");
            }
            else
            {
                DrawField("Node Count", GraphHelper.NodeCount);
                DrawField("Mouse Pos", _inputListener.MousePosition);

                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Run"))
                    RunGraph.InvokeSafe();

                GUILayout.EndHorizontal();
            }

            GUILayout.EndArea();

            if (_selectedNode != null && GraphHelper != null)
            {
                // Node Debug
                GUILayout.BeginArea(new Rect(new Vector2(Screen.width - 400f, Screen.height - 120f), new Vector2(400f, 1000f)));

                DrawHeader("Selected Node: " + _selectedNode.Name);

                _selectedNode.Pins.ForEach(pin =>
                {
                    DrawField(string.Format("{0} (ID: {1}) (Connected: {2})", pin.Name, pin.Index, GraphHelper.IsPinConnected(pin)), pin.ToString());
                });

                GUILayout.EndArea();
            }
        }

        void DrawHeader(string label)
        {
            var guiStyle = new GUIStyle();
            guiStyle.fontStyle = FontStyle.Bold;

            GUILayout.Label(label, guiStyle);
        }

        void DrawField(string label, object value = null)
        {
            string valueStr = string.Empty;

            if (value != null)
                valueStr = value.ToString() == string.Empty ? "N/A" : value.ToString();

            var outStr = value != null ? string.Format("{0}: {1}", label, valueStr) : label;
            GUILayout.Label(outStr);
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

            return outPin;
        }
    }
}
