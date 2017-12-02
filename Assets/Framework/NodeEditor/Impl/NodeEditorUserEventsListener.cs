using UnityEngine;
using System;
using Framework.NodeEditorViews;
using NodeSystem.Editor;
using NodeSystem;

namespace Framework
{
    class NodeEditorUserEventsListener : INodeEditorUserEventsListener
    {
        public event Action<Node> SelectNode;
        public event Action<NodePin> MouseDownOverPin;
        public event Action MouseDown;
        public event Action MouseUp;
        public event Action<NodePin> MouseUpOverPin;
        public event Action<NodePin> MouseHoverEnterPin;
        public event Action MouseHoverLeavePin;

        public event Action RunGraph;
        public event Action SaveGraph;
        public event Action RevertGraph;

        public event Action<AddNodeEvent> AddNode;
        public event Action RemoveAllNodes;

        public event Action Duplicate;
        public event Action Delete;

        private EditorInputListener _inputListener;
        private NodeEditorView _editorView;

        private NodePin _lastHoveredPin;

        public NodeEditorUserEventsListener(NodeEditorView editorView)
        {
            _editorView = editorView;

            _editorView.ContextMenu.AddNode += ContextMenu_AddNode;
            _editorView.ContextMenu.ClearNodes += ContextMenu_ClearNodes;

            _editorView.MenuView.Revert += MenuView_Revert;
            _editorView.MenuView.Run += MenuView_Run;
            _editorView.MenuView.Save += MenuView_Save;

            _inputListener = new EditorInputListener();
            _inputListener.DeletePressed += InputListener_DeletePressed;
            _inputListener.MouseDown += InputListener_MouseDown;
            _inputListener.MouseUp += InputListener_MouseUp;
            _inputListener.KeyPressed += InputListener_KeyPressed;
        }

        void MenuView_Revert() { RevertGraph.InvokeSafe(); }
        void MenuView_Run() { RunGraph.InvokeSafe(); }
        void MenuView_Save() { SaveGraph.InvokeSafe(); }

        void ContextMenu_AddNode(AddNodeEvent addNodeEvent) { AddNode.InvokeSafe(addNodeEvent); }
        void ContextMenu_ClearNodes() { RemoveAllNodes.InvokeSafe(); }

        void InputListener_MouseUp(EditorMouseEvent mouseEvent)
        {
            _editorView.GraphView.GetAnyPinUnderMouse((pin) =>
            {
                DebugEx.Log<NodeEditorUserEventsListener>("Mouse released over Pin {0}. (Node ID: {1}) (Button: {2})", 
                    pin.Name, 
                    pin.Node.ID, 
                    mouseEvent.Button);

                if (mouseEvent.IsLeftMouse)
                    MouseUpOverPin.InvokeSafe(pin);
            });

            MouseUp.InvokeSafe();
        }

        void InputListener_MouseDown(EditorMouseEvent mouseEvent)
        {
            _editorView.GraphView.GetNodeUnderMouse((node) =>
            {
                if (_editorView.GraphView.WindowSize.Contains(mouseEvent.Position))
                    SelectNode.InvokeSafe(node);
            });
            _editorView.GraphView.GetAnyPinUnderMouse((pin) => MouseDownOverPin.InvokeSafe(pin));
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

            var currentHoveredPin = _editorView.GraphView.GetAnyPinUnderMouse();
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
