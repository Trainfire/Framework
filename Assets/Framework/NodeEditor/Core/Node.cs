using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.NodeSystem
{
    public abstract class Node
    {
        public event Action<Node> Changed;
        public event Action<NodePin> PinAdded;
        public event Action<NodePin> PinRemoved;
        public event Action<Node> Destroyed;

        public List<NodePin> Pins { get; private set; }
        public List<NodePin> InputPins { get; private set; }
        public List<NodePin> OutputPins { get; private set; }

        public string Name { get; private set; }
        public string ID { get; private set; }

        private Vector2 _position;
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                // Don't trigger if position is the same.
                if (value != Position)
                {
                    _position = value;

                    // TODO: Only trigger after the mouse has been released?
                    //TriggerChange();
                }
            }
        }

        public Node()
        {
            Pins = new List<NodePin>();
            InputPins = new List<NodePin>();
            OutputPins = new List<NodePin>();   
        }

        public void Initialize(NodeData data)
        {
            Name = data.Name;
            ID = data.ID;
            Position = data.Position;

            InputPins.Clear(); // TEMP!
            OutputPins.Clear(); // TEMP!
            OnInitialize();
        }

        protected virtual void OnInitialize() { }

        public virtual void Calculate() { }

        protected NodeValuePin<T> AddInputPin<T>(string name)
        {
            var pin = new NodeValuePin<T>(name, Pins.Count, this);
            RegisterPin(pin);
            InputPins.Add(pin);
            return pin;
        }

        protected NodeValuePin<T> AddOutputPin<T>(string name)
        {
            var pin = new NodeValuePin<T>(name, Pins.Count, this);
            RegisterPin(pin);
            OutputPins.Add(pin);
            return pin;
        }

        protected NodeExecutePin AddExecuteInPin()
        {
            var pin = new NodeExecutePin("In", Pins.Count, this);
            RegisterPin(pin);
            InputPins.Add(pin);
            return pin;
        }

        protected NodeExecutePin AddExecuteOutPin()
        {
            var pin = new NodeExecutePin("Out", Pins.Count, this);
            RegisterPin(pin);
            OutputPins.Add(pin);
            return pin;
        }

        protected void RemoveInputPin(int pinIndex)
        {
            if (pinIndex <= InputPins.Count && InputPins.Count > 0)
            {
                DebugEx.Log<Node>("Remove input pin.");
                var pin = InputPins[pinIndex];
                RemovePin(pin);
                InputPins.Remove(pin);
            }
        }

        protected void RemoveOutputPin(int pinIndex)
        {
            if (pinIndex <= OutputPins.Count && OutputPins.Count > 0)
            {
                DebugEx.Log<Node>("Remove output pin.");
                var pin = OutputPins[pinIndex];
                RemovePin(pin);
                OutputPins.Remove(pin);
            }
        }

        protected void TriggerChange()
        {
            Changed.InvokeSafe(this);
        }

        public bool IsInputPin(NodePin pin)
        {
            return InputPins.Contains(pin);
        }

        public bool IsOutputPin(NodePin pin)
        {
            return OutputPins.Contains(pin);
        }

        public bool HasPin(int pinId)
        {
            return pinId < Pins.Count;
        }

        public bool HasExecutePins()
        {
            return Pins.Any(x => x.GetType() == typeof(NodeExecutePin));
        }

        void RegisterPin(NodePin pin)
        {
            Pins.Add(pin);
            PinAdded.InvokeSafe(pin);
        }

        void RemovePin(NodePin pin)
        {
            Pins.Remove(pin);
            PinRemoved.InvokeSafe(pin);
            TriggerChange();
        }
    }
}