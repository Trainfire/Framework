using UnityEngine;
using UnityEditor;
using Framework.NodeSystem;

namespace Framework.NodeEditor
{
    /// <summary>
    /// Finds the root gameobject to load/save graph data to and from.
    /// </summary>
    public class NodeEditorRootFinder
    {
        private NodeGraphRoot _root;
        private NodeEditor _controller;

        public NodeEditorRootFinder(NodeEditor controller)
        {
            _controller = controller;
            _controller.GraphSaved += Controller_GraphSaved;
            _controller.NodeSelected += Controller_NodeSelected;

            Selection.selectionChanged += LoadGraphFromSelection;
        }

        void Controller_GraphSaved(NodeGraphData obj)
        {
            _root.GraphData = obj;
        }

        void LoadGraphFromSelection()
        {
            if (Selection.activeGameObject == null)
                return;

            var graphRootFromSelection = Selection.activeGameObject.GetComponentInParent<NodeGraphRoot>();

            bool selectionChanged = graphRootFromSelection == null || graphRootFromSelection != _root;
            if (selectionChanged)
                _controller.ClearGraph();

            // Assign new root.
            _root = graphRootFromSelection;

            if (_root != null)
                _controller.Load(_root.GraphData);
        }

        void Controller_NodeSelected(Node node)
        {
            if (_root != null)
                _root.Selection = node;
        }

        public void Destroy()
        {
            _controller.GraphSaved -= Controller_GraphSaved;
            _controller.NodeSelected -= Controller_NodeSelected;

            Selection.selectionChanged -= LoadGraphFromSelection;
        }
    }
}