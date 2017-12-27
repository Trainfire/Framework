using System;
using NodeSystem.Editor;

namespace NodeSystem
{
    public class NodeGraphRunner
    {
        private const int MaxExecutions = 256;

        private INodeEditorLogger _logger;
        private NodeGraph _graph;
        private NodeRunner _runner;
        private Node _currentNode;
        private int _executions;

        public NodeGraphRunner()
        {
            _logger = NodeEditor.GetNewLoggerInstance();
            _logger.LogLevel = NodeEditorLogLevel.ErrorsAndWarnings;
        }

        public void ExecuteEvent(NodeGraph graph, string eventName)
        {
            if (graph == null)
            {
                _logger.LogError<NodeGraphRunner>("Cannot run graph as it is null.");
                return;
            }

            _graph = graph;
            _runner = new NodeRunner(_graph.Helper, true);

            var eventNodes = graph.Helper.GetNodes<NodeGraphEvent>(eventName);

            if (eventNodes.Count == 0)
            {
                _logger.LogError<NodeGraphRunner>("Cannot run graph as no node was found for event '{0}'.", eventName);
                return;
            }

            if (eventNodes.Count > 0)
                _logger.LogWarning<NodeGraphRunner>("Found multiple nodes for event '{0}'. Using the first found node...", eventName);

            _logger.Log<NodeGraphRunner>("Executing...");

            _currentNode = eventNodes[0];
            MoveNext();
        }

        void MoveNext()
        {
            NodeEditor.Assertions.IsNotNull(_currentNode);

            _logger.Log<NodeGraphRunner>("Move next: {0} ({1})", _currentNode.Name, _currentNode.ID);

            // Run through all the nodes connected to the current node to prepare it for execution.
            _runner.StartFrom(_currentNode);

            var executeHandler = _currentNode as INodeExecuteHandler;
            if (executeHandler != null)
                executeHandler.Execute();

            _executions++;

            if (_executions == MaxExecutions)
            {
                _logger.LogWarning<NodeGraphRunner>("Max executions have been reached!");
                return;
            }
            else
            {
                var executeOutput = _currentNode as INodeExecuteOutput;

                if (executeOutput != null)
                {
                    var connection = _graph.Helper.GetConnectionFromStartPin(executeOutput.ExecuteOut);
                    if (connection != null)
                    {
                        _currentNode = connection.RightNode;
                        MoveNext();
                    }
                }

                _logger.Log<NodeGraphRunner>("Finished execution of all nodes.");
            }
        }
    }
}
