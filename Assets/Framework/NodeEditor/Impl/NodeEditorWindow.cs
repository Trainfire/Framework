using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System;
using System.Collections.Generic;
using Framework.NodeEditorViews;
using Framework.NodeSystem;
using NodeSystem.Editor;

namespace Framework
{
    /// <summary>
    /// Wraps the actual NodeEditor.
    /// </summary>
    public class NodeEditorWindow : EditorWindow
    {
        private const string WindowName = "Node Editor";

        private NodeGraph _graph;
        private NodeEditorView _view;
        private NodeEditorUserEventsListener _input;
        private NodeEditorController _controller;
        private NodeEditorPinConnector _pinConnector;

        private NodeEditorRootFinder _rootHandler;

        public NodeEditorWindow()
        {
            Assert.raiseExceptions = true;

            NodeEditor.Logger = new NodeEditorLogger();

            _graph = new NodeGraph();
            _view = new NodeEditorView(_graph.Helper);
            _input = new NodeEditorUserEventsListener(_view);
            _controller = new NodeEditorController(_graph, _input);
            _pinConnector = new NodeEditorPinConnector(_graph, _view.ConnectorView, _input);
            _rootHandler = new NodeEditorRootFinder(_controller);
        }

        void OnGUI()
        {
            _input.Update();
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

    public class NodeEditorLogger : INodeEditorLogger
    {
        public void Log<TSource>(string message, params object[] args) { DebugEx.Log<TSource>(message, args); }
        public void LogError<TSource>(string message, params object[] args) { DebugEx.Log<TSource>(message, args); }
        public void LogWarning<TSource>(string message, params object[] args) { DebugEx.Log<TSource>(message, args); }
    }
}
