namespace Framework.NodeSystem
{
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
                    // Early out when hitting an execute node and take the value from it's pin.
                    if (CurrentConnection.EndNode != null && CurrentConnection.EndNode.HasExecutePins())
                    {
                        CurrentConnection.StartPin.SetValueFromPin(CurrentConnection.EndPin);
                    }
                    else
                    {
                        SubGroup = new NodeExecutionGroup(Depth + 1, CurrentConnection.EndNode, _graphHelper);
                    }
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
