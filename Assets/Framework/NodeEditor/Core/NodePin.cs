using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.NodeSystem
{
    public abstract class NodePin
    {
        public Node Node { get; private set; }
        public string Name { get; private set; }
        public int Index { get; private set; }
        public Type WrappedType { get { return Type.WrappedType; } }

        public Vector2 ScreenPosition { get { return LocalRect.position + Node.Position; } }
        public Rect LocalRect { get; set; }

        public abstract NodePinType Type { get; }

        public NodePin(string name, int index, Node node)
        {
            Index = index;
            Name = name;
            Node = node;
        }

        public bool ArePinsCompatible(NodePin pin)
        {
            return pin.GetType() == this.GetType();
        }

        public bool IsInput()
        {
            return Node.IsInputPin(this);
        }

        public bool IsOutput()
        {
            return Node.IsOutputPin(this);
        }

        public virtual void SetValueFromPin(NodePin pin) { }

        protected virtual void OnDisconnect() { }
    }

    public class NodeValuePin<T> : NodePin
    {
        public NodeValuePin(string name, int index, Node node) : base(name, index, node) { }

        public override NodePinType Type { get { return NodePinTypeRegistry.Get<T>(); } }

        private T _value;
        public T Value
        {
            get
            {
                if (_value == null)
                    _value = default(T);

                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public override void SetValueFromPin(NodePin pin)
        {
            NodeValuePin<T> convertedPin = null;
            try
            {
                convertedPin = pin as NodeValuePin<T>;
            }
            catch (Exception ex)
            {
                DebugEx.LogError<NodePin>(ex.Message);
            }

            _value = convertedPin.Value;
        }

        protected override void OnDisconnect()
        {
            _value = default(T);
        }

        public override string ToString()
        {
            return _value != null ? _value.ToString() : string.Empty;
        }
    }

    public class NodeExecutePin : NodePin
    {
        public NodeExecutePin(string name, int index, Node node) : base(name, index, node) { }
        public override NodePinType Type { get { return NodePinTypeRegistry.Get<NodePinTypeExecute>(); } }
        public override string ToString() { return "Execute"; }
    }
}