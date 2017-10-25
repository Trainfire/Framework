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
        public event Action<NodePin> MouseClickedPin;
        public event Action<NodePin> MouseReleasedOverPin;
        public event Action MouseReleased;

        public NodeGraphInfo GraphInfo { get; set; }

        private EditorInputListener _inputListener;
        private Dictionary<Node, NodeView> _nodeViews;
        private NodeConnectionView _nodeConnectionView;

        public NodeEditorGraphView()
        {
            _inputListener = new EditorInputListener();
            _inputListener.MouseLeftClicked += InputListener_MouseLeftClicked;
            _inputListener.MouseLeftReleased += InputListener_MouseLeftReleased;
            _inputListener.DeletePressed += InputListener_DeletePressed;

            _nodeViews = new Dictionary<Node, NodeView>();

            _nodeConnectionView = new NodeConnectionView();
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

        void NodeView_MouseClickedPin(NodePin nodePin)
        {
            MouseClickedPin.InvokeSafe(nodePin);
        }

        void NodeView_MouseReleasedOverPin(NodePin nodePin)
        {
            MouseReleasedOverPin.InvokeSafe(nodePin);
        }

        void InputListener_MouseLeftReleased(EditorMouseEvent mouseEvent)
        {
            GetAnyPinUnderMouse((pin) =>
            {
                DebugEx.Log<NodeEditorGraphView>("Mouse released over Pin {0}. (Node ID: {1})", pin.Name, pin.Node.ID);
                MouseReleasedOverPin.InvokeSafe(pin);
            });
            MouseReleased.InvokeSafe();
        }

        void InputListener_MouseLeftClicked(EditorMouseEvent mouseEvent)
        {
            GetAnyPinUnderMouse((pin) =>
            {
                DebugEx.Log<NodeEditorGraphView>("Pin {0} was clicked. (Node ID: {1})", pin.Name, pin.Node.ID);
                MouseClickedPin.InvokeSafe(pin);
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

            // Draw nodes.
            _nodeViews.Values.ToList().ForEach(x => x.Draw());

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
