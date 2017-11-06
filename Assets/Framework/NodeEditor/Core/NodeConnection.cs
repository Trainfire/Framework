using UnityEngine;
using UnityEditor;

namespace Framework.NodeSystem
{
    public class NodeConnection
    {
        public NodePin StartPin { get; private set; }
        public NodePin EndPin { get; private set; }
        public Node StartNode { get { return StartPin.Node; } }
        public Node EndNode { get { return EndPin.Node; } }

        public NodeConnection(NodePin startPin, NodePin endPin)
        {
            StartPin = startPin;
            EndPin = endPin;
        }
    }
}