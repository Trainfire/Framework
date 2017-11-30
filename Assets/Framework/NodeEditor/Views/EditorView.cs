using UnityEngine;
using System;
using Framework.NodeSystem;
using UnityEditor;

namespace Framework.NodeEditor.Views
{
    public class NodeEditorView
    {
        Node Selection
        {
            set
            {
                Properties.SelectedNode = value;
                Debugger.SelectedNode = value;
            }
        }

        public NodeEditorGraphView GraphView { get; private set; }
        public NodeEditorContextMenu ContextMenu { get; private set; }
        public NodeEditorMenuView MenuView { get; private set; }
        public NodeEditorPinConnectorView ConnectorView { get; private set; }
        public NodeEditorPropertiesView Properties { get; private set; }
        public NodeEditorDebugView Debugger { get; private set; }

        private NodeGraphHelper _graphHelper;

        public NodeEditorView(NodeGraphHelper graphHelper)
        {
            GraphView = new NodeEditorGraphView();
            ContextMenu = new NodeEditorContextMenu();
            MenuView = new NodeEditorMenuView();
            ConnectorView = new NodeEditorPinConnectorView();
            Properties = new NodeEditorPropertiesView();
            Debugger = new NodeEditorDebugView();

            _graphHelper = graphHelper;
            _graphHelper.NodeSelected += GraphHelper_NodeSelected;
            GraphView.GraphHelper = _graphHelper;
            Debugger.GraphHelper = _graphHelper;
        }

        void GraphHelper_NodeSelected(Node node)
        {
            Selection = node;
        }

        /// <summary>
        /// Called by NodeEditor. Do not call directly!
        /// </summary>
        public void Draw(Action BeginWindowsFunc, Action EndWindowsFunc)
        {
            float menuHeight = EditorStyles.toolbar.fixedHeight;
            const float propertiesWidth = 350f;

            GUILayout.BeginArea(new Rect(0f, 0f, Screen.width, menuHeight));
            MenuView.Draw();
            MenuView.GraphDirty = _graphHelper.IsGraphDirty;
            MenuView.GraphLoaded = _graphHelper.IsGraphLoaded;
            GUILayout.EndArea();

            BeginWindowsFunc();
            GraphView.WindowSize = new Rect(0f, 0f, Screen.width - propertiesWidth, Screen.height - 20f);
            GraphView.Draw();
            EndWindowsFunc();
            
            GUILayout.BeginArea(new Rect(Screen.width - propertiesWidth, menuHeight, propertiesWidth, Screen.height - menuHeight));
            Properties.Draw();
            GUILayout.EndArea();

            Debugger.Draw();
            ContextMenu.Draw();
            ConnectorView.Draw();
        }
    }
}
