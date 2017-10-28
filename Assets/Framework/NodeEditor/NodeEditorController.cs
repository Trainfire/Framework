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
            _view.GraphView.MouseLeftClickedPin += GraphView_MouseLeftClickedPin;
            _view.GraphView.MouseLeftReleasedOverPin += GraphView_MouseLeftReleasedOverPin;
            _view.GraphView.MouseMiddleClickedPin += GraphView_MouseMiddleClickedPin;
            _view.GraphView.MouseReleased += GraphView_MouseReleased;
            _view.GraphView.NodeSelected += GraphView_NodeSelected;
            _view.GraphView.NodeDeleted += GraphView_NodeDeleted;

            Selection.selectionChanged += GetGraphFromSelection;
        }

        void GetGraphFromSelection()
        {
            var selection = Selection.activeGameObject;
            
            if (selection != null)
            {
                var graph = selection.GetComponentInParent<NodeGraph>();

                // Don't reload if graph is same as previous graph.
                if (graph != _graph)
                {
                    if (graph != null)
                    {
                        Assert.IsNotNull(graph, "Graph was null.");

                        DebugEx.Log<NodeEditorController>("Loaded graph.");

                        _graph = graph;
                        _graph.NodeAdded += Graph_NodeAdded;
                        _graph.NodeRemoved += Graph_NodeRemoved;

                        _graph.Nodes.ForEach(node => _view.GraphView.AddNodeView(node));

                        _view.GraphView.GraphInfo = _graph.Info;
                    }
                    else
                    {
                        _graph = null;
                        _view.GraphView.RemoveAllNodeViews();
                    }
                }
            }
        }

        #region Graph Callbacks
        void Graph_NodeRemoved(Node node)
        {
            _view.GraphView.RemoveNodeView(node);
        }

        void Graph_NodeAdded(Node node)
        {
            _view.GraphView.AddNodeView(node);
        }

        void Graph_NodeDestroyed(Node node)
        {
            _view.GraphView.RemoveNodeView(node);
        }
        #endregion

        #region Graph View Callbacks
        void GraphView_NodeDeleted(Node node)
        {
            _graph.RemoveNode(node);
        }

        void GraphView_NodeSelected(Node node)
        {
            Selection.activeGameObject = node.gameObject;
        }
        #endregion

        #region Pin Connection Callbacks
        void GraphView_MouseLeftClickedPin(NodePin nodePin)
        {
            _pinConnector.StartConnection(nodePin);
        }

        void GraphView_MouseLeftReleasedOverPin(NodePin nodePin)
        {
            _pinConnector.AttemptMakeConnection(nodePin);
        }

        void GraphView_MouseReleased()
        {
            _pinConnector.CancelConnection();
        }

        void GraphView_MouseMiddleClickedPin(NodePin nodePin)
        {
            if (nodePin.ConnectedPin != null)
                nodePin.Disconnect();
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