﻿using UnityEngine;
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

        private NodeEditorPinView _lastHoveredPin;

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
            _editorView.GraphView.GetPinViewUnderMouse((view) =>
            {
                DebugEx.Log<NodeEditorUserEventsListener>("Mouse released over Pin {0}. (Node ID: {1}) (Button: {2})", 
                    view.Pin.Name, 
                    view.Pin.Node.ID, 
                    mouseEvent.Button);

                if (mouseEvent.IsLeftMouse)
                    MouseUpOverPin.InvokeSafe(view.Pin);
            });

            MouseUp.InvokeSafe();
        }

        void InputListener_MouseDown(EditorMouseEvent mouseEvent)
        {
            _editorView.GraphView.GetNodeViewUnderMouse((view) =>
            {
                if (_editorView.GraphView.WindowSize.Contains(mouseEvent.Position) && view != null)
                    SelectNode.InvokeSafe(view.Node);
            });

            _editorView.GraphView.GetPinViewUnderMouse((view) => MouseDownOverPin.InvokeSafe(view.Pin));
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

            var currentHoveredPinView = _editorView.GraphView.GetPinViewUnderMouse();
            if (_lastHoveredPin != null && currentHoveredPinView == null)
            {
                MouseHoverLeavePin.InvokeSafe();
            }
            else if (_lastHoveredPin == null && currentHoveredPinView != null)
            {
                MouseHoverEnterPin.InvokeSafe(currentHoveredPinView.Pin);
            }

            _lastHoveredPin = currentHoveredPinView;
        }
    }
}
