using System;

namespace NodeSystem
{
    public enum NodeGraphVariableAccessorType
    {
        Get,
        GetSet,
        Set,
    }

    public abstract class NodeGraphVariable
    {
        public string Name { get; private set; }
        public string ID { get; private set; }
        public string Type { get; protected set; }

        public Type ActualType { get { return System.Type.GetType(Type); } }

        public NodeGraphVariable(NodeGraphVariableData data)
        {
            Name = data.Name;
            ID = data.ID;
            Type = data.VariableType;
        }

        public void Parse(string value)
        {
            SetIfType(typeof(float), () =>
            {
                float outValue;
                float.TryParse(value, out outValue);
                Set(outValue);
            });

            SetIfType(typeof(int), () =>
            {
                int outValue;
                int.TryParse(value, out outValue);
                Set(outValue);
            });

            SetIfType(typeof(bool), () =>
            {
                bool outValue;
                bool.TryParse(value, out outValue);
                Set(outValue);
            });

            SetIfType(typeof(string), () => Set(value));
        }

        void SetIfType(Type type, Action onIsType)
        {
            if (ActualType == type)
                onIsType();
        }

        protected virtual void Set(float value) { (this as NodeGraphVariable<float>).Value = value; }
        protected virtual void Set(int value) { (this as NodeGraphVariable<int>).Value = value; }
        protected virtual void Set(bool value) { (this as NodeGraphVariable<bool>).Value = value; }
        protected virtual void Set(string value) { (this as NodeGraphVariable<string>).Value = value; }
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