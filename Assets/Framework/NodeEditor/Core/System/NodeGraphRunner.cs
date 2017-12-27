using System;
using NodeSystem.Editor;

namespace NodeSystem
{
    public class NodeGraphRunner
    {
        private const int MaxExecutions = 256;

        private NodeGraph _graph;
        private NodeRunner _runner;
        private Node _currentNode;
        private int _executions;

        public void Run(NodeGraph graph)
        {
            if (graph == null)
            {
                NodeEditor.Logger.LogError<NodeGraphRunner>("Cannot run graph as it is null.");
                return;
            }

            _graph = graph;
            _runner = new NodeRunner(_graph.Helper, true);

            var startNode = graph.Helper.GetNodes<NodeGraphEvent>("Start");

            if (startNode.Count == 0)
            {
                NodeEditor.Logger.LogError<NodeGraphRunner>("Cannot run graph as no start node was found.");
                return;
            }

            if (startNode.Count > 0)
                NodeEditor.Logger.LogWarning<NodeGraphRunner>("Found multiple 'Start' nodes. Using the first found node...");

            NodeEditor.Logger.Log<NodeGraphRunner>("Executing...");

            _currentNode = startNode[0];
            MoveNext();
        }

        void MoveNext()
        {
            NodeEditor.Assertions.IsNotNull(_currentNode);

            NodeEditor.Logger.Log<NodeGraphRunner>("Move next: {0} ({1})", _currentNode.Name, _currentNode.ID);

            // Run through all the nodes connected to the current node to prepare it for execution.
            _runner.StartFrom(_currentNode);

            var executeHandler = _currentNode as INodeExecuteHandler;
            if (executeHandler != null)
                executeHandler.Execute();

            _executions++;

            if (_executions == MaxExecutions)
            {
                NodeEditor.Logger.LogWarning<NodeGraphRunner>("Max executions have been reached!");
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

                NodeEditor.Logger.Log<NodeGraphRunner>("Finished execution of all nodes.");
            }
        }
    }
}
