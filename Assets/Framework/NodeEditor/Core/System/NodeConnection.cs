using NodeSystem.Editor;

namespace NodeSystem
{
    public enum NodeConnectionType
    {
        Execute,
        Value,
    }

    public class NodeConnection
    {
        public bool Hidden { get; private set; }

        public NodePin StartPin { get; private set; }
        public NodePin EndPin { get; private set; }
        public Node StartNode { get { return StartPin.Node; } }
        public Node EndNode { get { return EndPin.Node; } }
        public NodeConnectionType Type
        {
            get
            {
                if (StartPin.IsExecutePin() && EndPin.IsExecutePin())
                {
                    return NodeConnectionType.Execute;
                }
                else
                {
                    return NodeConnectionType.Value;
                }
            }
        }

        public NodeConnection(NodePin startPin, NodePin endPin)
        {
            StartPin = startPin;
            EndPin = endPin;
        }

        public void Hide()
        {
            Hidden = true;
        }

        public void Show()
        {
            Hidden = false;
        }
    }
}