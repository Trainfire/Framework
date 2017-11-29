using UnityEngine.Assertions;
using Framework.NodeSystem;
using Framework.NodeEditor.Views;
using System;

namespace Framework.NodeEditor
{
    public interface INodeEditorInputHandler
    {
        event Action<Node> SelectNode;
        event Action<NodePin> SelectPin;
        event Action<NodePin> MouseUpOverPin;
        event Action MouseUp;
        event Action MouseDown;
        event Action<NodePin> MouseHoverEnterPin;
        event Action MouseHoverLeavePin;

        event Action Duplicate;
        event Action Delete;
    }

    public class NodeEditor
    {
        public event Action<NodeGraphData> GraphSaved;
        
        private NodeGraphRunner _runner;
        private NodeGraphState _graphState;
        private NodeGraph _graph;
        private NodeEditorPinConnector _pinConnector;
        private NodeEditorView _view;
        private INodeEditorInputHandler _inputHandler;

        private Node _selectedNode;

        public NodeEditor(NodeEditorView view, INodeEditorInputHandler inputHandler)
        {
            _inputHandler = inputHandler;
            _inputHandler.Duplicate += Input_Duplicate;
            _inputHandler.Delete += Input_Delete;
            _inputHandler.SelectNode += Input_SelectNode;

            _runner = new NodeGraphRunner();

            _graph = new NodeGraph();
            _graph.NodeAdded += Graph_NodeAdded;
            _graph.NodeRemoved += Graph_NodeRemoved;

            _graphState = new NodeGraphState(_graph);
            _graphState.Changed += GraphState_Changed;

            _view = view;
            _view.GraphHelper = _graph.Helper;

            _view.ContextMenu.OnAddNode += ContextMenu_OnAddNode;
            _view.ContextMenu.OnClearNodes += ContextMenu_OnClearNodes;

            _view.MenuView.Save += Save;
            _view.MenuView.Revert += RevertGraph;
            _view.MenuView.Run += RunGraph;

            _pinConnector = new NodeEditorPinConnector(_graph, _view.ConnectorView, _inputHandler);
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

        void RunGraph()
        {
            _runner.Run(_graph);
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

        #region Input Callbacks
        private void Input_SelectNode(Node node)
        {
            _selectedNode = node;
            _view.Selection = _selectedNode;
        }

        void Input_Duplicate()
        {
            // TODO.
            if (_selectedNode != null)
                DebugEx.Log<NodeEditor>("Do a duplicate, yeah?");
        }

        void Input_Delete()
        {
            if (_selectedNode != null)
                _graph.RemoveNode(_selectedNode);
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
            _inputHandler.Delete -= Input_Delete;
            _inputHandler.Duplicate -= Input_Duplicate;
            _inputHandler.SelectNode -= Input_SelectNode;

            _graph.NodeAdded -= Graph_NodeAdded;
            _graph.NodeRemoved -= Graph_NodeRemoved;
            _graphState.Changed -= GraphState_Changed;

            _view.ContextMenu.OnAddNode -= ContextMenu_OnAddNode;
            _view.ContextMenu.OnClearNodes -= ContextMenu_OnClearNodes;

            _view.MenuView.Save -= Save;
            _view.MenuView.Revert -= RevertGraph;
            _view.MenuView.Run -= RunGraph;

            ClearGraph();
        }
    }
}