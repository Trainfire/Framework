using UnityEngine.Assertions;
using Framework.NodeSystem;
using System;

namespace Framework.NodeEditor
{
    public class NodeEditor
    {
        public event Action<NodeGraphData> GraphSaved;

        private static INodeEditorLogger _logger;
        public static INodeEditorLogger Logger
        {
            get
            {
                if (_logger == null)
                    _logger = new NullNodeEditorLogger();
                return _logger;
            }
            set
            {
                if (value != null)
                    _logger = value;
            }
        }

        private NodeGraphRunner _runner;
        private NodeGraph _graph;
        private INodeEditorUserEventsListener _eventListener;

        public NodeEditor(NodeGraph graph, INodeEditorUserEventsListener inputHandler)
        {
            _graph = graph;

            _eventListener = inputHandler;
            _eventListener.Duplicate += Input_Duplicate;
            _eventListener.Delete += Input_Delete;
            _eventListener.SelectNode += Input_SelectNode;
            _eventListener.AddNode += Input_AddNode;
            _eventListener.SaveGraph += Save;
            _eventListener.RevertGraph += RevertGraph;
            _eventListener.RunGraph += RunGraph;

            _runner = new NodeGraphRunner();
        }

        public void Load(NodeGraphData graphData)
        {
            Logger.Log<NodeEditor>("Loading graph from root...");

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
            _graph.State.RevertToLastGraph();
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
                Logger.Log<NodeEditor>("Do a duplicate, yeah?");
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
            _eventListener.Delete -= Input_Delete;
            _eventListener.Duplicate -= Input_Duplicate;
            _eventListener.SelectNode -= Input_SelectNode;
            _eventListener.AddNode -= Input_AddNode;
            _eventListener.RemoveAllNodes -= Input_RemoveAllNodes;
            _eventListener.SaveGraph -= Save;
            _eventListener.RevertGraph -= RevertGraph;
            _eventListener.RunGraph -= RunGraph;

            ClearGraph();
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

    public interface INodeEditorLogger
    {
        void Log<TSource>(string message, params object[] args);
        void LogWarning<TSource>(string message, params object[] args);
        void LogError<TSource>(string message, params object[] args);
    }

    public class NullNodeEditorLogger : INodeEditorLogger
    {
        public void Log<TSource>(string message, params object[] args) { }
        public void LogError<TSource>(string message, params object[] args) { }
        public void LogWarning<TSource>(string message, params object[] args) { }
    }

    #region Events

    public class AddNodeEvent
    {
        public string NodeId { get; private set; }

        public AddNodeEvent(string nodeId)
        {
            NodeId = nodeId;
        }
    }
#endregion
}