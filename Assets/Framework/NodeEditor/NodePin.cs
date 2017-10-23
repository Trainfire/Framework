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
        public string Name { get; private set; }
        public NodePinType PinType { get; private set; }

        public void Initialize(string name, NodePinType pinType, NodePinValueType valueType)
        {
            Name = name;
            PinType = pinType;
            // ???
        }
    }
}
