using UnityEngine.Assertions;
using Framework.NodeSystem;
using Framework.NodeEditor.Views;
using System;

namespace Framework.NodeEditor
{
    public class NodeEditor
    {
        public event Action<NodeGraphData> GraphSaved;
        public event Action<Node> NodeSelected;

        private NodeGraphRunner _runner;
        private NodeGraphState _graphState;
        private NodeGraph _graph;
        private NodeEditorPinConnector _pinConnector;
        private NodeEditorView _view;

        public NodeEditor(NodeEditorView view)
        {
            _pinConnector = new NodeEditorPinConnector();
            _pinConnector.ConnectionMade += PinConnector_ConnectionMade;

            _runner = new NodeGraphRunner();

            _graph = new NodeGraph();
            _graph.NodeAdded += Graph_NodeAdded;
            _graph.NodeRemoved += Graph_NodeRemoved;

            _graphState = new NodeGraphState(_graph);
            _graphState.Changed += GraphState_Changed;

            _view = view;

            _view.ContextMenu.OnAddNode += ContextMenu_OnAddNode;
            _view.ContextMenu.OnClearNodes += ContextMenu_OnClearNodes;

            _view.GraphView.GraphHelper = _graph.Helper;
            _view.GraphView.MouseLeftClickedPin += GraphView_MouseLeftClickedPin;
            _view.GraphView.MouseLeftReleasedOverPin += GraphView_MouseLeftReleasedOverPin;
            _view.GraphView.MouseMiddleClickedPin += GraphView_MouseMiddleClickedPin;
            _view.GraphView.MouseReleased += GraphView_MouseReleased;
            _view.GraphView.NodeSelected += GraphView_NodeSelected;
            _view.GraphView.NodeDeleted += GraphView_NodeDeleted;
            _view.GraphView.RunGraph += GraphView_RunGraph;

            _view.MenuView.Save += Save;
            _view.MenuView.Revert += RevertGraph;
        }

        public void Load(NodeGraphData graphData)
        {
            DebugEx.Log<NodeEditor>("Loading graph from root...");

            // Copy from existing graph data.
            var editingGraphData = new NodeGraphData(graphData);
            _graph.Load(editingGraphData);

            _view.GraphView.GraphHelper = _graph.Helper;
        }

        void Save()
        {
            Assert.IsNotNull(_graph, "Graph is null.");

            if (_graph != null)
            {
                var saveData = NodeGraphHelper.GetGraphData(_graph);

                GraphSaved.InvokeSafe(saveData);

                _graph.Load(saveData);
            }
        }

        void RevertGraph()
        {
            _graphState.RevertToLastGraph();
        }

        public void ClearGraph()
        {
            _graph.Unload();
            _view.GraphView.Clear();
        }

        #region State Callbacks
        void GraphState_Changed(NodeGraphState graphState)
        {
            _view.MenuView.GraphDirty = graphState.IsDirty;
            _view.MenuView.GraphLoaded = graphState.GraphLoaded;
        }
        #endregion

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
        void GraphView_NodeSelected(Node node)
        {
            NodeSelected.InvokeSafe(node);
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
                _graph.Unload();
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

        public void Destroy()
        {
            _graph.NodeAdded -= Graph_NodeAdded;
            _graph.NodeRemoved -= Graph_NodeRemoved;
            _graphState.Changed -= GraphState_Changed;

            _view.ContextMenu.OnAddNode -= ContextMenu_OnAddNode;
            _view.ContextMenu.OnClearNodes -= ContextMenu_OnClearNodes;

            _view.GraphView.MouseLeftClickedPin -= GraphView_MouseLeftClickedPin;
            _view.GraphView.MouseLeftReleasedOverPin -= GraphView_MouseLeftReleasedOverPin;
            _view.GraphView.MouseMiddleClickedPin -= GraphView_MouseMiddleClickedPin;
            _view.GraphView.MouseReleased -= GraphView_MouseReleased;
            _view.GraphView.NodeSelected -= GraphView_NodeSelected;
            _view.GraphView.NodeDeleted -= GraphView_NodeDeleted;
            _view.GraphView.RunGraph -= GraphView_RunGraph;

            _view.MenuView.Save -= Save;
            _view.MenuView.Revert -= RevertGraph;

            ClearGraph();
        }
    }
}