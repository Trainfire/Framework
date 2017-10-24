using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.NodeEditor
{
    [ExecuteInEditMode]
    public abstract class Node : MonoBehaviour
    {
        public event Action<Node> Destroyed;

        public List<NodePin> Pins { get { return InputPins.Concat(OutputPins).ToList(); } }
        public List<NodePin> InputPins { get; private set; }
        public List<NodePin> OutputPins { get; private set; }

        public string Name { get { return name; } }
        public int ID { get { return GetInstanceID(); } }
        public Vector2 Position { get; set; }

        public Node()
        {
            InputPins = new List<NodePin>();
            OutputPins = new List<NodePin>();   
        }

        [ExecuteInEditMode]
        public void OnDestroy()
        {
            Destroyed.InvokeSafe(this);
        }

        protected void AddInputPin(string name, NodePinValueType valueType)
        {
            // TODO: Spawn from factory.
            var pin = gameObject.AddComponent<NodePin>();
            pin.Initialize(this, name, NodePinType.Input, valueType);
            InputPins.Add(pin);
        }

        protected void AddOutputPin(string name, NodePinValueType valueType)
        {
            // TODO: Spawn from factory.
            var pin = gameObject.AddComponent<NodePin>();
            pin.Initialize(this, name, NodePinType.Output, valueType);
            OutputPins.Add(pin);
        }

        public void Execute()
        {
            OnExecute();
        }

        protected abstract void OnExecute();
    }
}
