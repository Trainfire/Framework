using UnityEngine;
using System;
using System.Linq;
using Framework.NodeSystem;

namespace Framework.NodeEditorViews
{
    public class NodeEditorNodeView : BaseView
    {
        public Node Node { get; private set; }
        public Rect Rect { get; private set; }

        private int _windowId;
        private NodeEditorPinView _pinView;

        public NodeEditorNodeView(Node node, int windowId)
        {
            Node = node;
            _windowId = windowId;

            _pinView = new NodeEditorPinView();

            InputListener.MouseUp += InputListener_MouseUp;
        } 

        protected override void OnDispose()
        {
            Node = null;
            _pinView = null;
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
            Rect = new Rect(Node.Position.x - offset.x, Node.Position.y - offset.y, viewSize.x, viewSize.y);
            Rect = GUI.Window(_windowId, Rect, InternalDraw, Node.Name);

            // Set node position to untransformed position.
            Node.Position = Rect.position + offset;
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
            _pinView.Draw(pin, pin.LocalRect.Contains(InputListener.MousePosition));
        }

        void InputListener_MouseUp(EditorMouseEvent mouseEvent)
        {
            Node.TriggerPositionChanged();
        }
    }
}
