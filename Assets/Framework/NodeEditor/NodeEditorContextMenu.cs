using UnityEngine;
using UnityEditor;
using System;

namespace Framework.NodeEditor
{
    public class NodeEditorContextMenu
    {
        public event Action<string> OnAddNode;
        public event Action OnClearNodes;

        private EditorInputListener _inputListener;

        private GenericMenu _menu;

        public NodeEditorContextMenu()
        {
            _inputListener = new EditorInputListener();
            _inputListener.ContextClicked += OnContextClicked;

            _menu = new GenericMenu();

            var factory = new NodeFactory();
            factory.Registry.ForEach(x =>
            {
                _menu.AddItem(new GUIContent(x), false, () => OnAddNode.InvokeSafe(x));
            });

            _menu.AddItem(new GUIContent("Remove All Nodes"), false, () => OnClearNodes.InvokeSafe());
        }

        public void Draw()
        {
            _inputListener.ProcessEvents();
        }

        void OnContextClicked()
        {
            _menu.ShowAsContext();
        }
    }
}
