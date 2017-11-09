using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.NodeSystem
{
    public class NodeGraphRunner
    {
        private const int MaxExecutions = 256;

        private NodeGraph _graph;
        private int _executions;

        public NodeGraphRunner() { }

        public void Run(NodeGraph graph)
        {
            if (graph == null)
            {
                DebugEx.LogError<NodeGraphRunner>("Cannot run graph as it is null.");
                return;
            }

            _graph = graph;

            var startNode = graph.Helper.GetNode<CoreStart>();

            if (startNode == null)
            {
                DebugEx.LogError<NodeGraphRunner>("Cannot run graph as no start node was found.");
                return;
            }

            // TODO: Weird logic here.Just call StartNode.Execute() ?
            var executePin = startNode.OutputPins[0] as NodeExecutePin;
            if (executePin.ConnectedPin == null)
            {
                DebugEx.LogError<NodeGraphRunner>("Cannot run graph as start node is not connected.");
                return;
            }

            DebugEx.Log<NodeGraphRunner>("Executing...");

            MoveNext(startNode as NodeExecute);
        }

        void MoveNext(NodeExecute nodeExecute)
        {
            DebugEx.Log<NodeGraphRunner>("Move next: {0} ({1})", nodeExecute.Name, nodeExecute.ID);

            nodeExecute.Execute(new NodeExecuteParameters());

            _executions++;

            if (_executions == MaxExecutions)
            {
                DebugEx.LogWarning<NodeGraphRunner>("Max executions have been reached!");
                return;
            }
            else if (nodeExecute.OutputPins.Count > 0)
            {
                var connection = _graph.Helper.GetConnectionFromStartPin(nodeExecute.OutputPins[0]);
                if (connection != null)
                {
                    MoveNext(connection.EndNode as NodeExecute);
                }
                else
                {
                    DebugEx.Log<NodeGraphRunner>("Finished execution of all nodes.");
                }
            }
        }
    }
}
