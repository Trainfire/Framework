using UnityEngine;
using UnityEngine.Assertions;
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
        public Vector2 Position { get; set; }

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

        protected NodePin<T> AddInputPin<T>(string name)
        {
            var pin = new NodePin<T>(name, Pins.Count, this);
            RegisterPin(pin);
            InputPins.Add(pin);
            return pin;
        }

        protected NodePin<T> AddOutputPin<T>(string name)
        {
            var pin = new NodePin<T>(name, Pins.Count, this);
            RegisterPin(pin);
            OutputPins.Add(pin);
            return pin;
        }

        protected NodePin<NodePinTypeExecute> AddExecuteInPin()
        {
            var pin = new NodePin<NodePinTypeExecute>("In", Pins.Count, this);
            RegisterPin(pin);
            InputPins.Add(pin);
            return pin;
        }

        protected NodePin<NodePinTypeExecute> AddExecuteOutPin()
        {
            var pin = new NodePin<NodePinTypeExecute>("Out", Pins.Count, this);
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

        public NodePin<T> ChangePinType<T>(NodePin pin)
        {
            Assert.IsTrue(Pins.Contains(pin), string.Format("'{0}' does not contains pin '{1}'.", Name, pin.Name));

            if (Pins.Contains(pin))
            {
                var replacementPin = new NodePin<T>(pin.Name, pin.Index, this);
                Pins.Remove(pin);

                var targetList = pin.IsInput() ? InputPins : OutputPins;
                targetList.Insert(pin.Index, replacementPin);
                targetList.Remove(pin);

                Pins.Add(replacementPin);

                DebugEx.Log<Node>("Swapped pin '{0}' of type '{1}' for type '{2}'", replacementPin.Name, pin.WrappedType, replacementPin.WrappedType);

                return replacementPin;
            }

            return null;
        }

        protected void TriggerChange()
        {
            Changed.InvokeSafe(this);
        }

        protected virtual void OnPinConnected(NodePinConnectEvent pinConnectEvent) { }

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
            return Pins.Any(x => x.WrappedType == typeof(NodePinTypeExecute));
        }

        /// <summary>
        /// Triggers a change to tell the graph state that this node has moved positions.
        /// </summary>
        public void TriggerPositionChanged()
        {
            //TriggerChange();
        }

        void RegisterPin(NodePin pin)
        {
            Pins.Add(pin);
            PinAdded.InvokeSafe(pin);
            pin.Connected += OnPinConnected;
        }

        void RemovePin(NodePin pin)
        {
            Pins.Remove(pin);
            PinRemoved.InvokeSafe(pin);
            pin.Connected -= OnPinConnected;
            TriggerChange();
        }
    }
}