using UnityEngine;
using System;
using Framework.NodeSystem;
using Framework.NodeEditor.Views;

namespace Framework.NodeEditor
{
    class NodeEditorInputHandler : INodeEditorInputHandler
    {
        public event Action<Node> SelectNode;
        public event Action<NodePin> SelectPin;
        public event Action MouseDown;
        public event Action MouseUp;
        public event Action<NodePin> MouseUpOverPin;
        public event Action<NodePin> MouseHoverEnterPin;
        public event Action MouseHoverLeavePin;

        public event Action Duplicate;
        public event Action Delete;

        private EditorInputListener _inputListener;
        private NodeEditorGraphView _graphView;

        private NodePin _lastHoveredPin;

        public NodeEditorInputHandler(NodeEditorGraphView graphView)
        {
            _graphView = graphView;

            _inputListener = new EditorInputListener();
            _inputListener.DeletePressed += InputListener_DeletePressed;
            _inputListener.MouseDown += InputListener_MouseDown;
            _inputListener.MouseUp += InputListener_MouseUp;
            _inputListener.KeyPressed += InputListener_KeyPressed;
        }

        void InputListener_MouseUp(EditorMouseEvent mouseEvent)
        {
            _graphView.GetAnyPinUnderMouse((pin) =>
            {
                DebugEx.Log<NodeEditorInputHandler>("Mouse released over Pin {0}. (Node ID: {1}) (Button: {2})", 
                    pin.Name, 
                    pin.Node.ID, 
                    mouseEvent.Button);

                if (mouseEvent.IsLeftMouse)
                    MouseUpOverPin.InvokeSafe(pin);
            });

            MouseUp.InvokeSafe();
        }

        void InputListener_MouseDown(EditorMouseEvent obj)
        {
            _graphView.GetNodeUnderMouse((node) => SelectNode.InvokeSafe(node));
        }

        void GraphView_NodeSelected(Node node)
        {
            SelectNode.InvokeSafe(node);
        }

        void InputListener_DeletePressed()
        {
            Delete.InvokeSafe();
        }

        void InputListener_KeyPressed(EditorKeyboardEvent keyboardEvent)
        {
            if (keyboardEvent.Event.control && keyboardEvent.KeyCode == KeyCode.D)
                Duplicate.InvokeSafe();
        }

        public void Update()
        {
            _inputListener.ProcessEvents();

            var currentHoveredPin = _graphView.GetAnyPinUnderMouse();
            if (_lastHoveredPin != null && currentHoveredPin == null)
            {
                MouseHoverLeavePin.InvokeSafe();
            }
            else if (_lastHoveredPin == null && currentHoveredPin != null)
            {
                MouseHoverEnterPin.InvokeSafe(currentHoveredPin);
            }

            _lastHoveredPin = currentHoveredPin;
        }
    }
}
