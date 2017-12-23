using System;
using NodeSystem.Editor;

namespace NodeSystem
{
    public enum NodeGraphVariableAccessorType
    {
        Get,
        GetSet,
        Set,
    }

    public class NodeGraphVariableTypeChangeEvent
    {
        public NodeGraphVariable Variable { get; private set; }
        public Type OldType { get; private set; }
        public Type NewType { get; private set; }

        public NodeGraphVariableTypeChangeEvent(NodeGraphVariable variable, Type oldType, Type newType)
        {
            Variable = variable;
            OldType = oldType;
            NewType = newType;
        }
    }

    public class NodeGraphVariable
    {
        public event Action<NodeGraphVariableTypeChangeEvent> PreTypeChanged;
        public event Action<NodeGraphVariableTypeChangeEvent> PostTypeChanged;

        public string Name { get; private set; }
        public string ID { get; private set; }

        public Type WrappedType { get { return WrappedValue != null ? WrappedValue.ValueType : null; } }
        public NodeValueWrapper WrappedValue { get; private set; }

        public NodeGraphVariable(NodeGraphVariableData data)
        {
            Name = data.Name;
            ID = data.ID;

            SetValueWrapper(Type.GetType(data.VariableType));
            WrappedValue.SetFromString(data.Value);
        }

        /// <summary>
        /// Sets the wrapped value based on the specified type.
        /// </summary>
        public void SetValueWrapper(Type wrappedValueType)
        {
            if (wrappedValueType == WrappedType)
                return;

            PreTypeChanged.InvokeSafe(new NodeGraphVariableTypeChangeEvent(this, WrappedType, wrappedValueType));

            NodeEditor.Assertions.IsNotNull(wrappedValueType);
            var classType = typeof(NodeValueWrapper<>).MakeGenericType(wrappedValueType);
            WrappedValue = Activator.CreateInstance(classType) as NodeValueWrapper;

            PostTypeChanged.InvokeSafe(new NodeGraphVariableTypeChangeEvent(this, WrappedType, wrappedValueType));
        }
    }
}