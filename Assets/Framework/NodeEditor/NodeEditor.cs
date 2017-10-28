using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Framework.NodeEditor
{
    public class NodeEditor : EditorWindow
    {
        private const string WindowName = "Node Editor";

        private NodeEditorView _view;

        public NodeEditor()
        {
            _view = new NodeEditorView();
            new NodeEditorController(_view);
            new NodeTests();
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
    }
}
