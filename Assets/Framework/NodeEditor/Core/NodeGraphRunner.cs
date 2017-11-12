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

    public class NodeExecution
    {
        public bool Completed { get; private set; }

        private NodeGraphHelper _graphHelper;
        private bool _autoIterate;

        private Stack<NodeExecutionGroup> _stack;

        public NodeExecution(NodeGraphHelper graphHelper, bool autoIterate = false)
        {
            _graphHelper = graphHelper;
            _autoIterate = autoIterate;
        }

        public void Start(Node startNode)
        {
            _stack = new Stack<NodeExecutionGroup>();

            // Auto-iterate on start node.
            _stack.Push(new NodeExecutionGroup(startNode, _graphHelper));

            if (_autoIterate)
                Iterate();
        }

        public void Iterate()
        {
            if (_stack.Count == 0)
            {
                Completed = true;
                return;
            }

            var currentGroup = _stack.Peek();
            currentGroup.Iterate();

            if (currentGroup.SubGroup != null)
            {
                _stack.Push(currentGroup.SubGroup);
            }
            else if (currentGroup.Finished)
            {
                // Node is prepared. Calculate it.
                currentGroup.Node.Calculate();

                // Pop from stack.
                _stack.Pop();
            }

            if (_autoIterate)
                Iterate();
        }
    }

    public class NodeExecutionGroup
    {
        public Node Node { get; private set; }
        public NodeExecutionGroup SubGroup { get; private set; }
        public bool Finished { get; private set; }

        private NodeGraphHelper _graphHelper;
        private int _currentPin;
        private int _pinCount;

        public NodeExecutionGroup(Node node, NodeGraphHelper graphHelper)
        {
            Node = node;

            _graphHelper = graphHelper;
            _pinCount = node.InputPins.Count;
        }

        public void Iterate()
        {
            // NB: Nodes such as constants have no input pins so no sub-groups will ever be created for them.
            if (_pinCount != 0)
            {
                var pin = Node.InputPins[_currentPin];
                var connection = _graphHelper.GetConnectionFromStartPin(pin);
                if (connection != null)
                {
                    SubGroup = new NodeExecutionGroup(connection.EndNode, _graphHelper);
                }
                else
                {
                    SubGroup = null;
                }

                _currentPin++;
            }

            if (_currentPin == _pinCount || _pinCount == 0)
            {
                SubGroup = null;
                Finished = true;
            }
        }
    }
}
