using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.NodeEditor
{
    [Serializable]
    public class NodePin
    {
        public event Action<NodePin, NodePin> PinConnected;

        public Node Node { get; private set; }
        public string Name { get; private set; }
        public int Index { get; private set; }
        public NodePin ConnectedPin { get; private set; }

        public Vector2 ScreenPosition { get { return LocalRect.position + Node.Position; } }
        public Rect LocalRect { get; set; }

        public NodePin(string name, int index, Node node)
        {
            Index = index;
            Name = name;
            Node = node;
        }

        public bool WillPinConnectionCreateCircularDependency(NodePin targetPin)
        {
            // Pin cannot be connected to a node that has connections to this pin's node.
            return Node.InputPins
                .Where(pin => pin.ConnectedPin != null)
                .Any(pin => pin.ConnectedPin.Node == targetPin.Node);
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
                ConnectedPin = targetPin;

                PinConnected.InvokeSafe(this, targetPin);
            }
        }

        public void Disconnect()
        {
            DebugEx.Log<NodePin>("{0} is now disconnected.", Name);
            ConnectedPin = null;
        }
    }

    public class NodeValuePin<T> : NodePin
    {
        public NodeValuePin(string name, int index, Node node) : base(name, index, node) { }

        public event Action<T> OnSet;
        public event Action OnGet;

        private T _value;
        public T Value
        {
            get
            {
                OnGet.InvokeSafe();

                if (_value == null)
                    _value = default(T);

                if (this.ConnectedPin == null)
                    return _value;

                NodeValuePin<T> connectedValuePin = null;
                try
                {
                    connectedValuePin = this.ConnectedPin as NodeValuePin<T>;
                }
                catch (Exception ex)
                {
                    DebugEx.Log<NodeValuePin<T>>(ex.Message);
                }

                _value = connectedValuePin.Value;

                //return connectedValuePin != null ? connectedValuePin.Value : _value;
                return _value;
            }
            set
            {
                _value = value;
                OnSet.InvokeSafe(_value);
            }
        }

        public override string ToString()
        {
            return _value != null ? _value.ToString() : string.Empty;
        }
    }

    public class NodeExecutePin : NodePin
    {
        private Action _onExecute;

        public NodeExecutePin(string name, int index, Node node, Action onExecute) : base(name, index, node)
        {
            _onExecute = onExecute;
        }

        public void Execute()
        {
            if (_onExecute == null)
                DebugEx.LogWarning<NodeExecutePin>("Cannot execute pin '{0}' on node '{1}'.", Name, Node.Name);

            _onExecute.InvokeSafe();
        }

        public override string ToString()
        {
            if (ConnectedPin != null)
                return string.Format("{0} ({1})", ConnectedPin.Name, ConnectedPin.Node.Name);
            return string.Empty;
        }
    }
}