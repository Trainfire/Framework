using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.NodeEditor
{
    public abstract class Node
    {
        public event Action<NodePin> PinAdded;
        public event Action<NodePin> PinRemoved;

        public event Action<Node> Destroyed;

        public List<NodePin> Pins { get { return InputPins.Concat(OutputPins).ToList(); } }
        public List<NodePin> InputPins { get; private set; }
        public List<NodePin> OutputPins { get; private set; }

        public string Name { get; private set; }
        public string ID { get; private set; }
        public Vector2 Position { get; set; }

        public Node()
        {
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

        [ExecuteInEditMode]
        public void OnDestroy()
        {
            Destroyed.InvokeSafe(this);
        }

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

        protected void AddExecuteInPin(Action onExecute)
        {
            var pin = new NodeExecutePin("In", Pins.Count, this, onExecute);
            RegisterPin(pin);
            InputPins.Add(pin);
        }

        protected void AddExecuteOutPin()
        {
            var pin = new NodeExecutePin("Out", Pins.Count, this, null);
            RegisterPin(pin);
            OutputPins.Add(pin);
        }

        public bool IsInputPin(NodePin pin)
        {
            return InputPins.Contains(pin);
        }

        public bool IsOutputPin(NodePin pin)
        {
            return OutputPins.Contains(pin);
        }

        public void Execute()
        {
            OnExecute();
        }

        protected virtual void OnExecute() { }

        void RegisterPin(NodePin pin)
        {
            PinAdded.InvokeSafe(pin);
        }
    }
}