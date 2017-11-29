using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System;
using System.Collections.Generic;
using Framework.NodeEditor.Views;
using Framework.NodeSystem;

namespace Framework.NodeEditor
{
    public class NodeEditorWindow : EditorWindow
    {
        private const string WindowName = "Node Editor";

        private NodeEditorInputHandler _inputHandler;
        private NodeEditorView _view;
        private NodeEditor _controller;
        private NodeEditorRootFinder _rootHandler;

        public NodeEditorWindow()
        {
            Assert.raiseExceptions = true;

            _view = new NodeEditorView();
            _inputHandler = new NodeEditorInputHandler(_view.GraphView);
            _controller = new NodeEditor(_view, _inputHandler);
            _rootHandler = new NodeEditorRootFinder(_controller);
        }

        void OnGUI()
        {
            _inputHandler.Update();
            _view.Draw(BeginWindows, EndWindows);
            Repaint();
        }

        [MenuItem("Window/Node Editor")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<NodeEditorWindow>(WindowName);
        }

        void OnDestroy()
        {
            _controller.Destroy();
        }
    }
}
