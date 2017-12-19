using System;

namespace NodeSystem
{
    public enum NodeGraphVariableAccessorType
    {
        Get,
        GetSet,
        Set,
    }

    public class NodeGraphVariable
    {
        public string Name { get; private set; }
        public string ID { get; private set; }
        public string Type { get; private set; }

        public Type ActualType { get { return System.Type.GetType(Type); } }

        public NodeGraphVariable(NodeGraphVariableData data)
        {
            Name = data.Name;
            ID = data.ID;
            Type = data.VariableType;
        }
    }

    public class NodeGraphVariable<T> : NodeGraphVariable
    {
        public T Value { get; set; }

        public NodeGraphVariable(NodeGraphVariableData data) : base(data)
        {
            Value = default(T);
        }
    }
}