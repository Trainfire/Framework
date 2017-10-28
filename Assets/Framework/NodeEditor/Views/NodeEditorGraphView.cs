using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Framework.NodeEditor
{
    public class NodeEditorGraphView
    {
        public event Action<Node> NodeSelected;
        public event Action<Node> NodeDeleted;
        public event Action<NodePin> MouseLeftClickedPin;
        public event Action<NodePin> MouseLeftReleasedOverPin;
        public event Action<NodePin> MouseMiddleClickedPin;
        public event Action MouseReleased;

        public NodeGraphInfo GraphInfo { get; set; }

        private EditorInputListener _inputListener;
        private Dictionary<Node, NodeView> _nodeViews;

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
                var nodeView = new NodeView(node);
                nodeView.NodeSelected += NodeView_selected;
                nodeView.NodeDeleted += NodeView_Deleted;

                _nodeViews.Add(node, nodeView);
            }
        }

        public void RemoveNodeView(Node node)
        {
            DebugEx.Log<NodeEditorGraphView>("Node was removed.");

            bool containsNode = _nodeViews.ContainsKey(node);

            Assert.IsTrue(containsNode);

            if (containsNode)
            {
                var nodeView = _nodeViews[node];
                nodeView.NodeSelected -= NodeView_selected;
                nodeView.Destroy();

                _nodeViews.Remove(node);
            }
        }

        public void RemoveAllNodeViews()
        {
            DebugEx.Log<NodeEditorGraphView>("Removing all nodes from view.");
            _nodeViews.ToList().ForEach(x => RemoveNodeView(x.Key));
        }

        #region Callbacks
        void NodeView_selected(NodeView nodeView)
        {
            NodeSelected.InvokeSafe(nodeView.Node);
        }

        void NodeView_Deleted(NodeView nodeView)
        {
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

            DrawDebug();
            DrawNodes();
            DrawConnections();
        }

        void DrawNodes()
        {
            _nodeViews.Values.ToList().ForEach(x => x.Draw());
        }

        void DrawConnections()
        {
            // TEMP: Get all connected pins in graph.
            var connectedPins = _nodeViews
                .Keys
                .SelectMany(node => node.Pins)
                .Where(pin => pin.ConnectedPin != null)
                .ToList();

            connectedPins.ForEach(pin =>
            {
                Handles.BeginGUI();
                Handles.DrawLine(DrawPinConnectionHandle(pin), DrawPinConnectionHandle(pin.ConnectedPin));
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
            GUILayout.BeginArea(new Rect(new Vector2(0f, Screen.height - 100f), new Vector2(400f, 100f)));

            DrawHeader("Graph Info");

            if (GraphInfo == null)
            {
                DrawField("No graph loaded");
            }
            else
            {
                DrawField("Node Count", GraphInfo.NodeCount);
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
