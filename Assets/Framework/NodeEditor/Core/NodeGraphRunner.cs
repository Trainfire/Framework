using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Assertions;

namespace Framework.NodeSystem
{
    public class NodeGraphRunner
    {
        private const int MaxExecutions = 256;

        private NodeGraph _graph;
        private NodeExecution _execution;
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

            DebugEx.Log<NodeGraphRunner>("Executing...");

            MoveNext(startNode);
        }

        void Prepare(Node node)
        {
            DebugEx.Log<NodeGraphRunner>("Preparing node '{0}'...", node.Name);

            foreach (var inputPin in node.InputPins)
            {
                var graphConnection = _graph.Helper.GetConnectionFromStartPin(inputPin);
                if (graphConnection != null)
                {
                    Prepare(graphConnection.EndNode);
                    inputPin.SetValueFromPin(graphConnection.EndPin);
                }
            }

            DebugEx.Log<NodeGraphRunner>("Calculating node '{0}'...", node.Name);
            node.Calculate();
        }

        void MoveNext(NodeExecute nodeExecute)
        {
            DebugEx.Log<NodeGraphRunner>("Move next: {0} ({1})", nodeExecute.Name, nodeExecute.ID);

            if (_execution == null)
                _execution = new NodeExecution(_graph.Helper, true);

            _execution.Start(nodeExecute);

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
