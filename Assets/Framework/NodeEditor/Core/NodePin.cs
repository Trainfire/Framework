﻿using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.NodeEditor
{
    public enum NodePinType
    {
        Input,
        Output,
    }

    public class NodePin
    {
        public Node Node { get; private set; }
        public string Name { get; private set; }
        public NodePin ConnectedPin { get; private set; }

        public Vector2 ScreenPosition { get { return LocalRect.position + Node.Position; } }
        public Rect LocalRect { get; set; }

        public NodePin(string name, Node node)
        {
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
        public NodeValuePin(string name, Node node) : base(name, node) { }

        private T _value;
        public T Value
        {
            get
            {
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

                return connectedValuePin != null ? connectedValuePin.Value : _value;
            }
            set
            {
                _value = value;
            }
        }
    }

    public class NodeExecutePin : NodePin
    {
        private Action _onExecute;

        public NodeExecutePin(string name, Node node, Action onExecute) : base(name, node)
        {
            _onExecute = onExecute;
        }

        public void Execute()
        {
            _onExecute.InvokeSafe();
        }
    }
}