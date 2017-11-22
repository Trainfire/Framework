using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using Framework.NodeSystem;

namespace Framework.NodeEditor.Views
{
    public class NodeView : BaseView
    {
        public event Action<NodeView> NodeSelected;
        public event Action<NodeView> NodeDeleted;

        public Node Node { get; private set; }

        private int _windowId;
        private Rect _rect;
        private NodePinView _pinDrawer;

        public NodeView(Node node, int windowId)
        {
            Node = node;

            _windowId = windowId;

            _pinDrawer = new NodePinView();

            InputListener.MouseDown += InputListener_MouseLeftClicked;
            InputListener.DeletePressed += InputListener_DeletePressed;
        }

        protected override void OnDestroy()
        {
            InputListener.MouseDown -= InputListener_MouseLeftClicked;
            InputListener.DeletePressed -= InputListener_DeletePressed;

            Node = null;
        }

        protected override void OnDraw() { }

        public void Draw(Vector2 offset)
        {
            if (Node == null)
                return;

            // NB: A whole bunch of hacks.
            const float nodeWidth = 100f;
            const float headerHeight = 20f;
            const float pinHeight = 20f;
            var height = (Math.Max(Node.InputPins.Count, Node.OutputPins.Count) * pinHeight) + headerHeight;
            var viewSize = new Vector2(nodeWidth, height);

            // Subtract offset due to inverted co-ordinates.
            _rect = new Rect(Node.Position.x - offset.x, Node.Position.y - offset.y, viewSize.x, viewSize.y);
            _rect = GUI.Window(_windowId, _rect, InternalDraw, Node.Name);

            // Set node position to untransformed position.
            Node.Position = _rect.position + offset;
        }

        void InternalDraw(int windowId)
        {
            InputListener.ProcessEvents();

            if (Node == null)
                return;

            GUILayout.BeginHorizontal();

            // Inputs
            GUILayout.BeginVertical();
            Node.InputPins.ForEach(x => DrawPin(x));
            GUILayout.EndVertical();

            // Outputs
            GUILayout.BeginVertical();
            Node.OutputPins.ForEach(x => DrawPin(x));
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            if (GetPinUnderMouse() == null)
                GUI.DragWindow();
        }

        public NodePin GetPinUnderMouse(Action<NodePin> OnPinExists = null)
        {
            var pinUnderMouse = Node
                .Pins
                .Where(x => x.LocalRect.Contains(InputListener.MousePosition))
                .FirstOrDefault();

            if (pinUnderMouse != null)
                OnPinExists.InvokeSafe(pinUnderMouse);

            return pinUnderMouse != null ? pinUnderMouse : null;
        }

        void DrawPin(NodePin pin)
        {
            _pinDrawer.Draw(pin, pin.LocalRect.Contains(InputListener.MousePosition));
        }

        void InputListener_MouseLeftClicked(EditorMouseEvent mouseEvent)
        {
            NodeSelected.InvokeSafe(this);
        }

        void InputListener_DeletePressed()
        {
            NodeDeleted.InvokeSafe(this);
        }
    }
}
