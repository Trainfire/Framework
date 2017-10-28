﻿using UnityEngine;
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

        protected NodeValuePin<T> AddInputPin<T>(string name)
        {
            var pin = new NodeValuePin<T>(name, this);
            InputPins.Add(pin);
            return pin;
        }

        protected NodeValuePin<T> AddOutputPin<T>(string name)
        {
            var pin = new NodeValuePin<T>(name, this);
            OutputPins.Add(pin);
            return pin;
        }

        protected void AddExecuteInPin(Action onExecute)
        {
            var pin = new NodeExecutePin("In", this, onExecute);
            InputPins.Add(pin);
        }

        protected void AddExecuteOutPin()
        {
            var pin = new NodeExecutePin("Out", this, null);
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
    }
}