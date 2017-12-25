using System;
using NodeSystem.Editor;

namespace NodeSystem
{
    public class NodeConstant : Node
    {
        public NodeValueWrapper ValueWrapper { get; private set; }

        protected NodePin Out { get; private set; }

        NodePinValueWrapper _wrappedOutPin;

        protected override void OnInitialize()
        {
            ValueWrapper = new NodeValueWrapper<NodePinTypeNone>();
            UpdateOutPin();
        }

        public void Set(NodeConstantData nodeConstantData)
        {
            var constantType = Type.GetType(nodeConstantData.ConstantType);
            SetType(Type.GetType(nodeConstantData.ConstantType));
            ValueWrapper.SetFromString(nodeConstantData.Value);
        }

        public void SetType(Type type)
        {
            if (type == ValueWrapper.ValueType)
                return;

            NodeEditor.Assertions.IsNotNull(type, "Type cannot be null");

            ValueWrapper = Activator.CreateInstance(typeof(NodeValueWrapper<>).MakeGenericType(type)) as NodeValueWrapper;
            UpdateOutPin();
        }

        void UpdateOutPin()
        {
            RemoveAllPins();
            Out = AddPin("Out", ValueWrapper.ValueType, true);
            _wrappedOutPin = NodePinValueWrapper.Instantiate(Out, ValueWrapper);
        }

        public override void Calculate()
        {
            _wrappedOutPin.Calculate();
        }
    }
}
