using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Framework.NodeEditor
{
    public class NodeEditorWindow : EditorWindow
    {
        private const string WindowName = "Node Editor";

        public NodeGraph Graph { get { return _graph; } }
        private NodeGraph _graph;

        private NodeEditorGraphView _graphView;
        private NodeEditorContextMenu _contextMenu;

        public NodeEditorWindow()
        {
            _graphView = new NodeEditorGraphView();
            _contextMenu = new NodeEditorContextMenu(this);
        }

        [MenuItem("Window/Node Editor")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<NodeEditorWindow>(WindowName);
        }

        void OnGUI()
        {
            // TEMP: Just find a graph in the scene and load it.
            if (GUILayout.Button("Load Graph"))
                OnLoad();

            _contextMenu.Draw();

            BeginWindows();
            _graphView.Draw();
            EndWindows();

            Repaint();
        }

        void OnLoad()
        {
            var selection = Selection.activeGameObject;

            if (selection != null && selection.GetComponent<NodeGraph>())
            {
                _graph = selection.GetComponent<NodeGraph>();
            }
            else
            {
                DebugEx.LogWarning<NodeEditorWindow>("Failed to open graph as no Node Graph is currently selected in the scene.");

                var existingGraph = FindObjectOfType<NodeGraph>();

                if (existingGraph == null)
                {
                    DebugEx.LogWarning<NodeEditorWindow>("Creating a default graph...");
                    _graph = new GameObject("Node Graph Root").AddComponent<NodeGraph>();
                }
                else
                {
                    _graph = existingGraph;
                }
            }

            DebugEx.LogWarning<NodeEditorWindow>("Selecting graph...");

            Selection.activeGameObject = _graph.gameObject;

            _graphView.Load(_graph);
        }
    }
}
