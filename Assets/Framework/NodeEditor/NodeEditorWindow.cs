using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace Framework.NodeEditor
{
    public class NodeEditorWindow : EditorWindow
    {
        private const string WindowName = "Node Editor";

        private EditorInputListener _inputListener;
        private NodeGraph _graph;
        private NodeEditorGraphView _graphView;

        public NodeEditorWindow()
        {
            _inputListener = new EditorInputListener();
            _inputListener.ContextClicked += OnContextClick;

            _graphView = new NodeEditorGraphView();
        }

        [MenuItem("Window/Node Editor")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow<NodeEditorWindow>(WindowName);
        }

        void OnGUI()
        {
            _inputListener.ProcessEvents();

            // TEMP: Just find a graph in the scene and load it.
            if (GUILayout.Button("Load Graph"))
                OnLoad();

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

        void OnContextClick()
        {
            // TEMP: Spawns a node on right-click.
            if (_graph != null)
                _graph.AddNode<Node>("Example Node");
        }

        void OnDestroy()
        {
            _inputListener.Destroy();
        }
    }
}
