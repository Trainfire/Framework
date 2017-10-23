using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;

namespace Framework.NodeEditor
{
    class NodeEditorController
    {
        private NodeGraph _graph;
        private NodeEditorView _view;

        public NodeEditorController(NodeEditorView view)
        {
            _view = view;
            _view.ContextMenu.OnAddNode += ContextMenu_OnAddNode;
            _view.ContextMenu.OnClearNodes += ContextMenu_OnClearNodes;

            Selection.selectionChanged += GetGraphFromSelection;
        }

        void GetGraphFromSelection()
        {
            var selection = Selection.activeGameObject;
            
            if (selection != null)
            {
                var graph = selection.GetComponent<NodeGraph>();

                if (graph != null)
                    LoadGraph(graph);
            }
        }

        void LoadGraph(NodeGraph graph)
        {
            Assert.IsNotNull(graph, "Graph was null");

            DebugEx.Log<NodeEditorController>("Loaded graph.");

            _graph = graph;
            _view.GraphView.Load(_graph);
        }

        void ContextMenu_OnClearNodes()
        {
            if (_graph != null)
                _graph.RemoveAllNodes();
        }

        void ContextMenu_OnAddNode(string nodeId)
        {
            if (_graph != null)
            {
                var factory = new NodeFactory();
                factory.Instantiate(nodeId, _graph);
            }
        }
    }
}
