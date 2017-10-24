using UnityEngine;

namespace Framework.NodeEditor
{
    public enum NodePinValueType
    {
        Float,
        Int,
        Bool,
        String,
    }

    public enum NodePinType
    {
        Input,
        Output,
    }

    public class NodePin : MonoBehaviour
    {
        public Node Node { get; private set; }
        public string Name { get; private set; }
        public NodePinType PinType { get; private set; }

        public void Initialize(Node node, string name, NodePinType pinType, NodePinValueType valueType)
        {
            Node = node;
            Name = name;
            PinType = pinType;
            // ???
        }
    }
}
