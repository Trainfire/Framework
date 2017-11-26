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
            var windowRect = position;

            _view.Draw(BeginWindows, EndWindows);

            // TEMP: Need to move this...somehow.
            //BeginWindows();
            //_view.DrawWindows();
            //EndWindows();

            //var windowRect = position; // This window rect is called position in Unity for some reason...
            //_view.GraphView.WindowSize = new Rect(windowRect.position, new Vector2(windowRect.width - 300f, windowRect.height));
            //_view.Properties.WindowSize = new Rect(new Vector2(windowRect.width - 300f, 0f), new Vector2(300f, windowRect.height));
            //_view.Draw();

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
