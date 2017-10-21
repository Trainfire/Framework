using UnityEngine;
using UnityEditor;

namespace Framework.NodeEditor
{
    class NodeEditorContextMenu
    {
        private NodeEditorWindow _nodeEditor;
        private EditorInputListener _inputListener;

        private GenericMenu _menu;

        public NodeEditorContextMenu(NodeEditorWindow nodeEditor)
        {
            _nodeEditor = nodeEditor;

            _inputListener = new EditorInputListener();
            _inputListener.ContextClicked += OnContextClicked;

            _menu = new GenericMenu();
            _menu.AddItem(new GUIContent("Add Node"), false, AddNode);
            _menu.AddItem(new GUIContent("Remove All Nodes"), false, ClearNodes);
        }

        public void Draw()
        {
            _inputListener.ProcessEvents();
        }

        void AddNode()
        {
            // TEMP: Add a basic node.
            if (_nodeEditor.Graph != null)
                _nodeEditor.Graph.AddNode<Node>("Some Nerd Node");
        }

        void ClearNodes()
        {
            _nodeEditor.Graph.RemoveAllNodes();
        }

        void OnContextClicked()
        {
            _menu.ShowAsContext();
        }
    }
}
