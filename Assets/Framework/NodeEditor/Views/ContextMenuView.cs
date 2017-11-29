using UnityEngine;
using UnityEditor;
using System;
using Framework.NodeSystem;

namespace Framework.NodeEditor.Views
{
    public class NodeEditorContextMenu
    {
        public event Action<AddNodeEvent> AddNode;
        public event Action ClearNodes;

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
                _menu.AddItem(new GUIContent(x), false, () => AddNode.InvokeSafe(new AddNodeEvent(x)));
            });

            _menu.AddSeparator("");

            _menu.AddItem(new GUIContent("Remove All Nodes"), false, () => ClearNodes.InvokeSafe());
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
