using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Framework
{
    public enum InputContext
    {
        PC,
        Xbox,
        PS4,
    }

    // The type of action
    public enum InputActionType
    {
        None,
        Down,
        Up,
        Held,
        Axis,
    }

    public interface IInputHandler
    {
        void HandleUpdate(InputUpdateEvent handlerEvent);
    }

    public interface IInputContextHandler
    {
        void HandleContextChange(InputContext context);
    }

    public class InputAction
    {
        public string Name { get; }

        public InputAction(string name) => Name = name;

        public override bool Equals(object obj)
        {
            return (obj as InputAction).Name == this.Name;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class InputButtonEvent : EventArgs
    {
        public InputAction Action { get; private set; }
        public InputActionType Type { get; private set; }
        public float Delta { get; private set; }

        public InputButtonEvent(InputAction action, InputActionType type)
        {
            Action = action;
            Type = type;
        }
    }

    public class InputAxisEvent<TValue> : EventArgs
    {
        public InputAction Action { get; private set; }
        public TValue Delta { get; private set; }

        public InputAxisEvent(InputAction action, TValue delta)
        {
            Action = action;
            Delta = delta;
        }
    }

    public class InputSingleAxisEvent : InputAxisEvent<float>
    {
        public InputSingleAxisEvent(InputAction action, float delta) : base(action, delta) { }
    }

    public class InputTwinAxisEvent : InputAxisEvent<Vector2>
    {
        public InputTwinAxisEvent(InputAction action, Vector2 delta) : base(action, delta) { }
    }

    /// <summary>
    /// Helper class that creates instance of Input Maps on the specified GameObject.
    /// </summary>
    public class InputHelper
    {
        public InputMapPC PC { get; private set; }
        public InputMapXbox Xbox { get; private set; }
        public InputMapPS4 PS4 { get; private set; }

        public List<IInputMap> Maps { get; } = new List<IInputMap>();

        public InputHelper(GameObject gameObject)
        {
            PC = gameObject.GetOrAddComponent<InputMapPC>();
            Xbox = gameObject.GetOrAddComponent<InputMapXbox>();
            PS4 = gameObject.GetOrAddComponent<InputMapPS4>();

            AddMap(PC);
            AddMap(Xbox);
            AddMap(PS4);
        }

        private void AddMap<TInputMap>(TInputMap inputMap) where TInputMap : IInputMap
        {
            Maps.Add(inputMap);
        }
    }

    // Handles input from an input map and relays to a handler
    public static class InputManager
    {
        private static List<IInputHandler> inputHandlers;
        private static List<IInputContextHandler> contextHandlers;
        private static List<IInputMap> maps;
        private static InputContext context;

        static InputManager()
        {
            inputHandlers = new List<IInputHandler>();
            contextHandlers = new List<IInputContextHandler>();
            maps = new List<IInputMap>();
        }

        public static void RegisterHandler(IInputHandler handler)
        {
            if (!inputHandlers.Contains(handler))
                inputHandlers.Add(handler);
        }

        public static void UnregisterHandler(IInputHandler handler)
        {
            if (inputHandlers.Contains(handler))
                inputHandlers.Remove(handler);
        }

        public static void RegisterHandler(IInputContextHandler handler)
        {
            if (!contextHandlers.Contains(handler))
                contextHandlers.Add(handler);
        }

        public static void UnregisterHandler(IInputContextHandler handler)
        {
            if (contextHandlers.Contains(handler))
                contextHandlers.Remove(handler);
        }

        public static void RegisterMap(IInputMap inputMap)
        {
            if (!maps.Contains(inputMap))
            {
                maps.Add(inputMap);
                inputMap.OnUpdate += Relay;
            }
        }

        public static void RegisterMaps(List<IInputMap> inputMaps)
        {
            inputMaps.ForEach(x => RegisterMap(x));
        }

        public static void UnregisterMap(IInputMap inputMap)
        {
            if (maps.Contains(inputMap))
            {
                maps.Remove(inputMap);
                inputMap.OnUpdate -= Relay;
            }
        }

        private static void Relay(object sender, InputUpdateEvent updateEvent)
        {
            inputHandlers.ForEach(x => x.HandleUpdate(updateEvent));
        }
    }
}
