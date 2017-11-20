﻿using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.NodeSystem
{
    public class NodePinConnectEvent
    {
        public NodePin Pin { get; private set; }
        public NodePin OtherPin { get; private set; }

        public NodePinConnectEvent(NodePin pin, NodePin otherPin)
        {
            Pin = pin;
            OtherPin = otherPin;
        }
    }

    public abstract class NodePin
    {
        public event Action<NodePinConnectEvent> Connected;
        public event Action<NodePin> Disconnected;

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

        public void Connect(NodePin otherPin)
        {
            Connected.InvokeSafe(new NodePinConnectEvent(this, otherPin));
        }

        public void Disconnect()
        {
            Disconnected.InvokeSafe(this);
            OnDisconnect();
        }

        public bool ArePinsCompatible(NodePin pin)
        {
            return pin.WrappedType == this.WrappedType || this.WrappedType == typeof(NodePinTypeAny);
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

    public class NodePin<T> : NodePin
    {
        public NodePin(string name, int index, Node node) : base(name, index, node) { }

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
            NodePin<T> convertedPin = null;
            try
            {
                convertedPin = pin as NodePin<T>;
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
}