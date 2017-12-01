using System.Collections.Generic;

namespace NodeSystem
{
    public class NodeRunner
    {
        public Node StartNode { get; private set; }
        public bool Completed { get; private set; }

        private NodeGraphHelper _graphHelper;
        private bool _autoIterate;

        private Stack<NodeExecutionGroup> _stack;

        public NodeRunner(NodeGraphHelper graphHelper, bool autoIterate = false)
        {
            _graphHelper = graphHelper;
            _autoIterate = autoIterate;
        }

        public void StartFrom(Node startNode)
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
}