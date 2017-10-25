using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;

namespace Framework.NodeEditor
{
    class NodeEditorController
    {
        private NodeGraph _graph;
        private NodeEditorPinConnector _pinConnector;
        private NodeEditorView _view;

        public NodeEditorController(NodeEditorView view)
        {
            _pinConnector = new NodeEditorPinConnector();

            _view = view;
            _view.ContextMenu.OnAddNode += ContextMenu_OnAddNode;
            _view.ContextMenu.OnClearNodes += ContextMenu_OnClearNodes;
            _view.GraphView.MouseClickedPin += GraphView_MouseClickedPin;
            _view.GraphView.MouseReleasedOverPin += GraphView_MouseReleasedOverPin;
            _view.GraphView.MouseReleased += GraphView_MouseReleased;

            Selection.selectionChanged += GetGraphFromSelection;
        }

        void GetGraphFromSelection()
        {
            var selection = Selection.activeGameObject;
            
            if (selection != null)
            {
                var graph = selection.GetComponent<NodeGraph>();

                if (graph != null)
                {
                    Assert.IsNotNull(graph, "Graph was null");

                    DebugEx.Log<NodeEditorController>("Loaded graph.");

                    _graph = graph;
                    _view.GraphView.Load(_graph);
                }
            }
        }

        #region Pin Connection Callbacks
        void GraphView_MouseClickedPin(NodePin nodePin)
        {
            _pinConnector.StartConnection(nodePin);
        }

        void GraphView_MouseReleasedOverPin(NodePin nodePin)
        {
            _pinConnector.AttemptMakeConnection(nodePin);
        }

        void GraphView_MouseReleased()
        {
            _pinConnector.CancelConnection();
        }
        #endregion

        #region Context Menu
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
        #endregion
    }
}