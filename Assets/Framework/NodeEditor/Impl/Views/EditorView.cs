using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System;
using System.Collections.Generic;
using NodeSystem;

namespace Framework.NodeEditorViews
{
    public class NodeEditorView : IDisposable
    {
        public NodeEditorGraphView GraphView { get; private set; }
        public NodeEditorContextMenuView ContextMenu { get; private set; }
        public NodeEditorMenuStripView MenuView { get; private set; }
        public NodeEditorPinConnectorView ConnectorView { get; private set; }
        public NodeEditorPropertiesView Properties { get; private set; }
        public NodeEditorDebugView Debugger { get; private set; }

        private NodeGraphHelper _graphHelper;
        private List<BaseView> _views;

        public NodeEditorView(NodeGraphHelper graphHelper)
        {
            _graphHelper = graphHelper;

            _views = new List<BaseView>();

            GraphView = AddView(new NodeEditorGraphView());
            ContextMenu = AddView(new NodeEditorContextMenuView());
            MenuView = AddView(new NodeEditorMenuStripView());
            ConnectorView = AddView(new NodeEditorPinConnectorView(GraphView));
            Properties = AddView(new NodeEditorPropertiesView());
            Debugger = AddView(new NodeEditorDebugView());
        }

        TView AddView<TView>(TView instance) where TView : BaseView
        {
            _views.Add(instance);
            Assert.IsNotNull(_graphHelper, "Graph Helper is null.");
            instance.Initialize(_graphHelper);
            return instance;
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

        public void Dispose()
        {
            _views.ForEach(x => x.Dispose());
            _views.Clear();
            _views = null;
        }
    }
}
