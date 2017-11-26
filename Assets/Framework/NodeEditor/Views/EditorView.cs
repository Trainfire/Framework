using UnityEngine;
using System;
using Framework.NodeSystem;
using UnityEditor;

namespace Framework.NodeEditor.Views
{
    public class NodeEditorView
    {
        public NodeGraphHelper GraphHelper
        {
            set
            {
                GraphView.GraphHelper = value;
                Debugger.GraphHelper = value;
            }
        }

        public Node Selection
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

        public NodeEditorView()
        {
            GraphView = new NodeEditorGraphView();
            ContextMenu = new NodeEditorContextMenu();
            MenuView = new NodeEditorMenuView();
            ConnectorView = new NodeEditorPinConnectorView();
            Properties = new NodeEditorPropertiesView();
            Debugger = new NodeEditorDebugView();
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
