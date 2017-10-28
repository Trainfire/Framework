using UnityEngine;
using System;
using System.Collections.Generic;

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
        //public Rect LocalRect { get; set; }

        public NodePin(string name, Node node)
        {
            Name = name;
            Node = node;
        }

        public bool IsCompatible(NodePin pin)
        {
            return pin.GetType() == this.GetType();
        }

        public void ConnectTo(NodePin pin)
        {
            DebugEx.Log<NodePin>("{0} is now connected {1}", Name, pin.Name);
            if (IsCompatible(pin))
                ConnectedPin = pin;
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

        public NodeExecutePin(string name, Node node, Action _onExecute) : base(name, node) { }

        public void Execute()
        {
            _onExecute.InvokeSafe();
        }
    }

    public class NodeTests
    {
        public NodeTests()
        {
            //var aPin = new NodeValuePin<int>("A");
            //aPin.Value = 2;

            //var bPin = new NodeValuePin<int>("B");
            //bPin.Value = 3;

            //var nodeAdd = new NodeMathAdd();
            //nodeAdd.Pins[0].ConnectTo(aPin);
            //nodeAdd.Pins[1].ConnectTo(bPin);

            //nodeAdd.Execute();
        }
    }
}