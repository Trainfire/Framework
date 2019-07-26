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
        void HandleInput(InputButtonEvent action);
    }

    public interface IInputUpdateHandler
    {
        void HandleInputUpdate(InputMapUpdateEvent updateEvent);
    }

    public interface IInputContextHandler
    {
        void HandleContextChange(InputContext context);
    }

    public class InputButtonEvent : EventArgs
    {
        public InputEvent ID { get; private set; }
        public InputActionType Type { get; private set; }
        public float Delta { get; private set; }

        public InputButtonEvent(InputEvent id, InputActionType type)
        {
            ID = id;
            Type = type;
        }
    }

    public class InputAxisEvent<TValue> : EventArgs
    {
        public InputEvent ID { get; private set; }
        public TValue Delta { get; private set; }

        public InputAxisEvent(InputEvent id, TValue delta)
        {
            ID = id;
            Delta = delta;
        }
    }

    public class InputSingleAxisEvent : InputAxisEvent<float>
    {
        public InputSingleAxisEvent(InputEvent axis, float delta) : base(axis, delta) { }
    }

    public class InputTwinAxisEvent : InputAxisEvent<Vector2>
    {
        public InputTwinAxisEvent(InputEvent axis, Vector2 delta) : base(axis, delta) { }
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
        private static List<IInputUpdateHandler> inputUpdateHandlers;
        private static List<IInputHandler> inputHandlers;
        private static List<IInputContextHandler> contextHandlers;
        private static List<IInputMap> maps;
        private static InputContext context;

        static InputManager()
        {
            inputUpdateHandlers = new List<IInputUpdateHandler>();
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

        public static void RegisterHandler(IInputUpdateHandler handler)
        {
            if (!inputUpdateHandlers.Contains(handler))
                inputUpdateHandlers.Add(handler);
        }

        public static void UnregisterHandler(IInputUpdateHandler handler)
        {
            if (inputUpdateHandlers.Contains(handler))
                inputUpdateHandlers.Remove(handler);
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

        private static void Relay(object sender, InputMapUpdateEvent updateEvent)
        {
            inputUpdateHandlers.ForEach(x => x.HandleInputUpdate(updateEvent));
        }
    }
}
