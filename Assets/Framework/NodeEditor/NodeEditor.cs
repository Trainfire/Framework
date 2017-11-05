using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System;
using System.Collections.Generic;
using Framework.NodeEditor.Views;

namespace Framework.NodeEditor
{
    public class NodeEditor : EditorWindow
    {
        public static string DefaultFilePath { get { return Application.dataPath + "/testGraph.nodegraph"; } }

        private const string WindowName = "Node Editor";

        private NodeEditorView _view;
        private NodeEditorController _controller;

        public NodeEditor()
        {
            Assert.raiseExceptions = true;
            _view = new NodeEditorView();
            _controller = new NodeEditorController(_view);
        }

        void OnGUI()
        {
            _view.Draw();

            // TEMP: Need to move this...somehow.
            BeginWindows();
            _view.DrawWindows();
            EndWindows();

            Repaint();
        }

        [MenuItem("Window/Node Editor")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<NodeEditor>(WindowName);
        }

        void OnDestroy()
        {
            _controller.Destroy();
        }
    }
}
