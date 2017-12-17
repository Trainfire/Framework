using System;

namespace NodeSystem
{
    public class NodeGraphVariable
    {
        public string Name { get; private set; }
        public string ID { get; private set; }
        public string Type { get; private set; }

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