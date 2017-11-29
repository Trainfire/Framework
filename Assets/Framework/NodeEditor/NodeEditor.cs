using UnityEngine.Assertions;
using Framework.NodeSystem;
using System;

namespace Framework.NodeEditor
{
    public class NodeEditor
    {
        public event Action<NodeGraphData> GraphSaved;
        
        private NodeGraphRunner _runner;
        private NodeGraphState _graphState;
        private NodeGraph _graph;
        private INodeEditorUserEventsListener _inputHandler;

        public NodeEditor(NodeGraph graph, INodeEditorUserEventsListener inputHandler)
        {
            _graph = graph;

            _inputHandler = inputHandler;
            _inputHandler.Duplicate += Input_Duplicate;
            _inputHandler.Delete += Input_Delete;
            _inputHandler.SelectNode += Input_SelectNode;
            _inputHandler.AddNode += Input_AddNode;
            _inputHandler.SaveGraph += Save;
            _inputHandler.RevertGraph += RevertGraph;
            _inputHandler.RunGraph += RunGraph;

            _runner = new NodeGraphRunner();

            _graphState = new NodeGraphState(_graph);
            //_graphState.Changed += GraphState_Changed;
        }

        public void Load(NodeGraphData graphData)
        {
            DebugEx.Log<NodeEditor>("Loading graph from root...");

            // Copy from existing graph data.
            var editingGraphData = new NodeGraphData(graphData);
            _graph.Load(editingGraphData);
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
        }

        #region Input Callbacks
        void Input_SelectNode(Node node)
        {
            _graph.SetSelection(node);
        }

        void Input_Duplicate()
        {
            // TODO.
            if (_graph.Selection != null)
                DebugEx.Log<NodeEditor>("Do a duplicate, yeah?");
        }

        void Input_Delete()
        {
            if (_graph.Selection != null)
                _graph.RemoveNode(_graph.Selection);
        }

        void Input_RemoveAllNodes()
        {
            if (_graph != null)
                _graph.Unload();
        }

        void Input_AddNode(AddNodeEvent addNodeEvent)
        {
            if (_graph != null)
            {
                var factory = new NodeFactory();
                factory.Instantiate(addNodeEvent.NodeId, _graph);
            }
        }
        #endregion

        public void Destroy()
        {
            _inputHandler.Delete -= Input_Delete;
            _inputHandler.Duplicate -= Input_Duplicate;
            _inputHandler.SelectNode -= Input_SelectNode;
            _inputHandler.AddNode -= Input_AddNode;
            _inputHandler.RemoveAllNodes -= Input_RemoveAllNodes;
            _inputHandler.SaveGraph -= Save;
            _inputHandler.RevertGraph -= RevertGraph;
            _inputHandler.RunGraph -= RunGraph;

            ClearGraph();
        }
    }

    public class NodeEditorGraphEvents
    {
        public NodeEditorGraphEvents(NodeGraph graph, NodeGraphState graphState)
        {
            graph.NodeAdded += Graph_NodeAdded;
        }

        private void Graph_NodeAdded(Node obj)
        {
            throw new NotImplementedException();
        }
    }

    public interface INodeEditorUserEventsListener
    {
        event Action<Node> SelectNode;
        event Action<NodePin> MouseDownOverPin;
        event Action<NodePin> MouseUpOverPin;
        event Action MouseUp;
        event Action MouseDown;
        event Action<NodePin> MouseHoverEnterPin;
        event Action MouseHoverLeavePin;

        event Action RunGraph;
        event Action SaveGraph;
        event Action RevertGraph;

        event Action<AddNodeEvent> AddNode;
        event Action RemoveAllNodes;
        event Action Duplicate;
        event Action Delete;
    }

    public class AddNodeEvent
    {
        public string NodeId { get; private set; }

        public AddNodeEvent(string nodeId)
        {
            NodeId = nodeId;
        }
    }
}