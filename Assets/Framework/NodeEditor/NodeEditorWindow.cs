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

        private NodeEditorView _view;
        private NodeEditor _controller;
        private NodeEditorRootFinder _rootHandler;

        public NodeEditorWindow()
        {
            Assert.raiseExceptions = true;

            _view = new NodeEditorView();
            _controller = new NodeEditor(_view);
            _rootHandler = new NodeEditorRootFinder(_controller);
        }

        void OnGUI()
        {
            // TEMP: Need to move this...somehow.
            BeginWindows();
            _view.DrawWindows();
            EndWindows();

            _view.GraphView.WindowSize = position;
            _view.Draw();

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
