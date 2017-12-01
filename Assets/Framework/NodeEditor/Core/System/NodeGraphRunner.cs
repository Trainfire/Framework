using UnityEngine.Assertions;
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

            var startNode = graph.Helper.GetNode<CoreStart>();

            if (startNode == null)
            {
                NodeEditor.Logger.LogError<NodeGraphRunner>("Cannot run graph as no start node was found.");
                return;
            }

            NodeEditor.Logger.Log<NodeGraphRunner>("Executing...");

            _currentNode = startNode;
            MoveNext();
        }

        void MoveNext()
        {
            Assert.IsNotNull(_currentNode);

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
                        _currentNode = connection.EndNode;
                        MoveNext();
                    }
                }

                NodeEditor.Logger.Log<NodeGraphRunner>("Finished execution of all nodes.");
            }
        }
    }
}
