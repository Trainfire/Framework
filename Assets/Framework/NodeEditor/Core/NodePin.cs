using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.NodeSystem
{
    [Serializable]
    public class NodePin
    {
        public event Action<NodePin, NodePin> PinConnected;
        public event Action<NodePin> PinDisconnected;

        public Node Node { get; private set; }
        public string Name { get; private set; }
        public int Index { get; private set; }

        public Vector2 ScreenPosition { get { return LocalRect.position + Node.Position; } }
        public Rect LocalRect { get; set; }

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

        public void ConnectTo(NodePin targetPin)
        {
            Assert.IsTrue(ArePinsCompatible(targetPin));

            if (ArePinsCompatible(targetPin))
            {
                DebugEx.Log<NodePin>("{0} is now connected {1}", Name, targetPin.Name);
                PinConnected.InvokeSafe(this, targetPin);
            }
        }

        public void Disconnect()
        {
            DebugEx.Log<NodePin>("{0} is now disconnected.", Name);
            PinDisconnected.InvokeSafe(this);
            //ConnectedPin = null;
            OnDisconnect();
        }

        public virtual void SetValueFromPin(NodePin pin) { }

        protected virtual void OnDisconnect() { }
    }

    public class NodeValuePin<T> : NodePin
    {
        public NodeValuePin(string name, int index, Node node) : base(name, index, node) { }

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
        public NodeExecutePin(string name, int index, Node node) : base(name, index, node)
        {

        }

        //public override string ToString()
        //{
        //    if (ConnectedPin != null)
        //        return string.Format("{0} ({1})", ConnectedPin.Name, ConnectedPin.Node.Name);
        //    return string.Empty;
        //}
    }
}