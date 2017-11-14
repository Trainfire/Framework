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
        public List<NodePinView> PinViews { get { return _pinViews.Values.ToList(); } }

        private int _windowId;
        private Rect _rect;
        private EditorInputListener _inputListener;
        private Node _node;
        private Dictionary<NodePin, NodePinView> _pinViews;

        public NodeView(Node node, int windowId)
        {
            _pinViews = new Dictionary<NodePin, NodePinView>();

            _inputListener = new EditorInputListener();
            _inputListener.MouseDown += InputListener_MouseLeftClicked;
            _inputListener.DeletePressed += InputListener_DeletePressed;

            Node = node;
            Node.PinAdded += AddPin;
            Node.PinRemoved += RemovePin;

            // Restore pins.
            Node.Pins.ForEach(pin => AddPin(pin));

            _windowId = windowId;
        }

        protected override void OnDestroy()
        {
            _inputListener.MouseDown -= InputListener_MouseLeftClicked;
            _inputListener.DeletePressed -= InputListener_DeletePressed;

            _pinViews.Values.ToList().ForEach(x => x.Destroy());
            _pinViews.Clear();

            Node.PinAdded -= AddPin;
            Node.PinRemoved -= RemovePin;

            Node = null;
        }

        protected override void OnDraw() { }

        public void Draw(Vector2 offset)
        {
            if (Node == null)
                return;

            const float headerHeight = 21f;
            const float pinHeight = 24f;
            var height = (Node.Pins.Count * pinHeight) + headerHeight;
            var viewSize = new Vector2(100f, height);

            // Subtract offset due to inverted co-ordinates.
            _rect = new Rect(Node.Position.x - offset.x, Node.Position.y - offset.y, viewSize.x, viewSize.y);
            _rect = GUI.Window(_windowId, _rect, InternalDraw, Node.Name);

            // Set node position to untransformed position.
            Node.Position = _rect.position + offset;
        }

        void InternalDraw(int windowId)
        {
            _inputListener.ProcessEvents();

            if (Node == null)
                return;

            GUILayout.BeginHorizontal();

            // Inputs
            GUILayout.BeginVertical();
            if (Node.InputPins.Count != 0)
                Node.InputPins.ForEach(pin => _pinViews[pin].Draw());
            GUILayout.EndVertical();

            // Outputs
            GUILayout.BeginVertical();
            if (Node.OutputPins.Count != 0)
                Node.OutputPins.ForEach(pin => _pinViews[pin].Draw());
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            if (GetPinUnderMouse() == null)
                GUI.DragWindow();
        }

        public NodePinView GetViewFromPin(NodePin pin)
        {
            bool hasViewForPin = _pinViews.ContainsKey(pin);
            return hasViewForPin ? _pinViews[pin] : null;
        }

        public NodePin GetPinUnderMouse(Action<NodePin> OnPinExists = null)
        {
            var view = _pinViews
                .Values
                .Where(x => x.LocalRect.Contains(_inputListener.MousePosition))
                .FirstOrDefault();

            if (view != null && view.Pin != null)
                OnPinExists.InvokeSafe(view.Pin);

            return view != null ? view.Pin : null;
        }

        void InputListener_MouseLeftClicked(EditorMouseEvent mouseEvent)
        {
            NodeSelected.InvokeSafe(this);
        }

        void InputListener_DeletePressed()
        {
            NodeDeleted.InvokeSafe(this);
        }

        void RemovePin(NodePin pin)
        {
            _pinViews.Remove(pin, true);
        }

        void AddPin(NodePin pin)
        {
            _pinViews.Add(pin, new NodePinView(pin), true);
        }
    }
}
