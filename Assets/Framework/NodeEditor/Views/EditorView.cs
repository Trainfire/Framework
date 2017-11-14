using UnityEngine;

namespace Framework.NodeEditor.Views
{
    public class NodeEditorView
    {
        public NodeEditorGraphView GraphView { get; private set; }
        public NodeEditorContextMenu ContextMenu { get; private set; }
        public NodeEditorMenuView MenuView { get; private set; }
        public NodeEditorPinConnectorView ConnectorView { get; private set; }

        public NodeEditorView()
        {
            GraphView = new NodeEditorGraphView();
            ContextMenu = new NodeEditorContextMenu();
            MenuView = new NodeEditorMenuView();
            ConnectorView = new NodeEditorPinConnectorView();
        }

        /// <summary>
        /// Called by NodeEditor. Do not call directly!
        /// </summary>
        public void Draw()
        {
            ContextMenu.Draw();
            MenuView.Draw();
            ConnectorView.Draw();
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
