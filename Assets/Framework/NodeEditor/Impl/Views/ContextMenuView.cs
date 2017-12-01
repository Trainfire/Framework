using UnityEngine;
using UnityEditor;
using System;
using Framework.NodeSystem;
using NodeSystem.Editor;

namespace Framework.NodeEditorViews
{
    public class NodeEditorContextMenuView : BaseView
    {
        public event Action<AddNodeEvent> AddNode;
        public event Action ClearNodes;

        private GenericMenu _menu;

        protected override void OnInitialize()
        {
            InputListener.ContextClicked += OnContextClicked;

            _menu = new GenericMenu();

            var factory = new NodeFactory();
            factory.Registry.ForEach(x =>
            {
                _menu.AddItem(new GUIContent(x), false, () => AddNode.InvokeSafe(new AddNodeEvent(x)));
            });

            _menu.AddSeparator("");

            _menu.AddItem(new GUIContent("Remove All Nodes"), false, () => ClearNodes.InvokeSafe());
        }

        void OnContextClicked()
        {
            _menu.ShowAsContext();
        }

        protected override void OnDispose()
        {
            AddNode = null;
            ClearNodes = null;
        }
    }
}
