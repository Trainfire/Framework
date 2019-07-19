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
        public string Action { get; private set; }
        public InputActionType Type { get; private set; }
        public float Delta { get; private set; }

        public InputButtonEvent(string action, InputActionType type)
        {
            Action = action;
            Type = type;
        }
    }

    public class InputAxisEvent<TValue> : EventArgs
    {
        public string Axis { get; private set; }
        public TValue Delta { get; private set; }

        public InputAxisEvent(string axis, TValue delta)
        {
            Axis = axis;
            Delta = delta;
        }
    }

    public class InputSingleAxisEvent : InputAxisEvent<float>
    {
        public InputSingleAxisEvent(string axis, float delta) : base(axis, delta) { }
    }

    public class InputTwinAxisEvent : InputAxisEvent<Vector2>
    {
        public InputTwinAxisEvent(string axis, Vector2 delta) : base(axis, delta) { }
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
                //inputMap.ActionTriggered += Relay;
                inputMap.OnUpdate += Relay;
            }
        }

        public static void UnregisterMap(IInputMap inputMap)
        {
            if (maps.Contains(inputMap))
            {
                maps.Remove(inputMap);
                //inputMap.ActionTriggered -= Relay;
                inputMap.OnUpdate -= Relay;
            }
        }

        private static void Relay(object sender, InputButtonEvent action)
        {
            //if (action.Context != context)
            //{
            //    context = action.Context;
            //    contextHandlers.ForEach(x => x.HandleContextChange(context));
            //}

            //inputHandlers.ForEach(x => x.HandleInput(action));
        }

        private static void Relay(object sender, InputMapUpdateEvent updateEvent)
        {
            inputUpdateHandlers.ForEach(x => x.HandleInputUpdate(updateEvent));
            //inputHandlers.ForEach(x => x.HandleInput(action));
        }
    }
}
