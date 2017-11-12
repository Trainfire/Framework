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
        public Node StartNode { get; private set; }
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
            StartNode = startNode;

            _stack = new Stack<NodeExecutionGroup>();
            _stack.Push(new NodeExecutionGroup(0, startNode, _graphHelper)); // Auto-iterate on start node.

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
                currentGroup.Node.Calculate(); // Node is prepared. Calculate it.
                _stack.Pop();
            }

            if (_autoIterate)
                Iterate();
        }
    }

    public class NodeExecutionGroup
    {
        public Node Node { get; private set; }
        public NodeConnection CurrentConnection { get; private set; }
        public NodeExecutionGroup SubGroup { get; private set; }
        public int Depth { get; private set; }
        public bool Finished { get; private set; }

        private NodeGraphHelper _graphHelper;
        private int _currentPin;
        private int _pinCount;

        public NodeExecutionGroup(int depth, Node node, NodeGraphHelper graphHelper)
        {
            Depth = depth;
            Node = node;

            _graphHelper = graphHelper;
            _pinCount = node.InputPins.Count;

            // Early out.
            if (_pinCount == 0)
                Finished = true;
        }

        public void Iterate()
        {
            if (_currentPin == _pinCount || _pinCount == 0)
            {
                Log("Finished iterating on node '{0}'.", Node.Name);
                SubGroup = null;
                Finished = true;
                return;
            }

            Log("Iterate on pin '{0}' on '{1}'...", Node.InputPins[_currentPin].Name, Node.Name);

            if (SubGroup != null)
            {
                if (SubGroup.Finished)
                {
                    Log("Subgroup '{0}' has finished. Setting pin value for '{1}' on '{2}'...",
                        SubGroup.Node.Name, 
                        CurrentConnection.StartNode.InputPins[_currentPin].Name,
                        CurrentConnection.StartNode.Name);
                    CurrentConnection.StartPin.SetValueFromPin(CurrentConnection.EndPin);
                    SubGroup = null;
                }
            }
            else
            {
                var pin = Node.InputPins[_currentPin];
                CurrentConnection = _graphHelper.GetConnectionFromStartPin(pin);
                if (CurrentConnection != null)
                {
                    SubGroup = new NodeExecutionGroup(Depth + 1, CurrentConnection.EndNode, _graphHelper);
                }
                else
                {
                    SubGroup = null;
                }
            }

            // Not waiting for anything, so iterate pin.
            if (SubGroup == null)
                _currentPin++;
        }

        void Log(string message, params object[] args)
        {
            var prefix = string.Format("Depth: {0} - ", Depth);
            DebugEx.Log<NodeExecutionGroup>(prefix + message, args);
        }
    }
}
