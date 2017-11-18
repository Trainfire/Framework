using UnityEngine.Assertions;

namespace Framework.NodeSystem
{
    public class NodeGraphRunner
    {
        private const int MaxExecutions = 256;

        private NodeGraph _graph;
        private NodeRunner _runner;
        private NodeExecute _currentNode;
        private int _executions;

        public void Run(NodeGraph graph)
        {
            if (graph == null)
            {
                DebugEx.LogError<NodeGraphRunner>("Cannot run graph as it is null.");
                return;
            }

            _graph = graph;
            _runner = new NodeRunner(_graph.Helper, true);

            var startNode = graph.Helper.GetNode<CoreStart>();

            if (startNode == null)
            {
                DebugEx.LogError<NodeGraphRunner>("Cannot run graph as no start node was found.");
                return;
            }

            DebugEx.Log<NodeGraphRunner>("Executing...");

            _currentNode = startNode;
            MoveNext();
        }

        void MoveNext()
        {
            Assert.IsNotNull(_currentNode);

            DebugEx.Log<NodeGraphRunner>("Move next: {0} ({1})", _currentNode.Name, _currentNode.ID);

            // Run through all the nodes connected to the current node to prepare it for execution.
            _runner.StartFrom(_currentNode);

            _currentNode.Execute();

            _executions++;

            if (_executions == MaxExecutions)
            {
                DebugEx.LogWarning<NodeGraphRunner>("Max executions have been reached!");
                return;
            }
            else
            {
                var connection = _graph.Helper.GetConnectionFromStartPin(_currentNode.ExecuteOut);
                if (connection != null)
                {
                    _currentNode = connection.EndNode as NodeExecute;
                    MoveNext();
                }
                else
                {
                    DebugEx.Log<NodeGraphRunner>("Finished execution of all nodes.");
                }
            }
        }
    }
}
