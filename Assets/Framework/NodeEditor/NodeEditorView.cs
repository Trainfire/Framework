using UnityEngine;
using UnityEditor;
using System;

namespace Framework.NodeEditor
{
    public class NodeEditorView
    {
        public NodeEditorGraphView GraphView { get; private set; }
        public NodeEditorContextMenu ContextMenu { get; private set; }

        public NodeEditorView()
        {
            GraphView = new NodeEditorGraphView();
            ContextMenu = new NodeEditorContextMenu();
        }

        /// <summary>
        /// Called by NodeEditor. Do not call directly!
        /// </summary>
        public void Draw()
        {
            ContextMenu.Draw();
        }

        /// <summary>
        /// Called by NodeEditor. Do not call directly!
        /// </summary>
        public void DrawWindows()
        {
            GraphView.Draw();
        }
    }
}
