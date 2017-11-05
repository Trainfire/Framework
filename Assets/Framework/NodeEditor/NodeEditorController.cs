using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;
using Framework.NodeSystem;
using Framework.NodeEditor.Views;

namespace Framework.NodeEditor
{
    class NodeEditorController
    {
        private NodeGraphRunner _runner;
        private NodeGraph _graph;
        private NodeGraphRoot _root;
        private NodeEditorPinConnector _pinConnector;
        private NodeEditorView _view;

        public NodeEditorController(NodeEditorView view)
        {
            _pinConnector = new NodeEditorPinConnector();
            _pinConnector.ConnectionMade += PinConnector_ConnectionMade;
            _runner = new NodeGraphRunner();

            _view = view;

            _view.ContextMenu.OnAddNode += ContextMenu_OnAddNode;
            _view.ContextMenu.OnClearNodes += ContextMenu_OnClearNodes;

            _view.GraphView.MouseLeftClickedPin += GraphView_MouseLeftClickedPin;
            _view.GraphView.MouseLeftReleasedOverPin += GraphView_MouseLeftReleasedOverPin;
            _view.GraphView.MouseMiddleClickedPin += GraphView_MouseMiddleClickedPin;
            _view.GraphView.MouseReleased += GraphView_MouseReleased;
            _view.GraphView.NodeDeleted += GraphView_NodeDeleted;
            _view.GraphView.NodeSelected += GraphView_NodeSelected;
            _view.GraphView.RunGraph += GraphView_RunGraph;

            _view.MenuView.Save += SaveGraph;
            _view.MenuView.Revert += RevertGraph;

            Selection.selectionChanged += RevertGraph;
        }

        void RevertGraph()
        {
            var selection = Selection.activeGameObject;

            if (selection != null)
            {
                var root = selection.GetComponentInParent<NodeGraphRoot>();

                if (root == null && _graph != null)
                {
                    // TODO: Unload graph properly.
                    _graph = null;
                    _view.GraphView.Clear();
                }

                // Don't reload if graph is same as previous graph.
                if (_graph == null && root != null)
                {
                    Assert.IsNotNull(root, "Root was null.");

                    _root = root;

                    DebugEx.Log<NodeEditorController>("Loading graph...");

                    // Clear existing view.
                    _view.GraphView.Clear();

                    _graph = new NodeGraph();
                    _graph.NodeAdded += Graph_NodeAdded;
                    _graph.NodeRemoved += Graph_NodeRemoved;
                    _graph.StateChanged += Graph_StateChanged;

                    _view.GraphView.GraphHelper = _graph.Helper;

                    DebugEx.Log<NodeEditorController>("Finished loading graph.");
                }

                if (_graph != null)
                {
                    var editingGraphData = new NodeGraphData(_root.GraphData);
                    _graph.SetData(editingGraphData);
                }
            }
        }

        void SaveGraph()
        {
            Assert.IsNotNull(_graph, "Graph is null.");
            Assert.IsNotNull(_root, "Root is null.");

            if (_graph != null && _root != null)
                _root.GraphData = _graph.GetData();
        }

        #region Graph Callbacks
        void Graph_StateChanged(NodeGraph graph)
        {
            _view.MenuView.GraphDirty = true;
        }

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
        void GraphView_NodeSelected(Node node)
        {
            if (_root != null)
                _root.Selection = node;
        }

        void GraphView_NodeDeleted(Node node)
        {
            _graph.RemoveNode(node);
        }

        void GraphView_RunGraph()
        {
            _runner.Run(_graph);
        }
        #endregion

        #region Pin Connection Callbacks
        void PinConnector_ConnectionMade(NodeConnectionData connection)
        {
            _graph.Connect(connection);
        }

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
            _graph.Disconnect(nodePin);
        }
        #endregion

        #region Context Menu
        void ContextMenu_OnClearNodes()
        {
            if (_graph != null)
                _graph.Clear();
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