using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;

namespace Framework
{
    public class EditorInputListener
    {
        public Vector2 MousePosition { get; private set; }

        public event Action MouseLeftClicked;
        public event Action MouseLeftReleased;
        public event Action<Vector2> MouseDragged;
        public event Action<Vector2> MouseMoved;

        public event Action ContextClicked;

        public event Action<KeyCode> KeyPressed;
        public event Action<KeyCode> KeyReleased;

        private Dictionary<EventType, List<Action>> _mouseEventMap;

        public EditorInputListener()
        {
            _mouseEventMap = new Dictionary<EventType, List<Action>>();

            AddMouseHandler(EventType.MouseDown, () => MouseLeftClicked.InvokeSafe());
            AddMouseHandler(EventType.MouseUp, () => MouseLeftReleased.InvokeSafe());
            AddMouseHandler(EventType.MouseDrag, () => MouseDragged.InvokeSafe(Event.current.mousePosition));
            AddMouseHandler(EventType.MouseMove, () => MouseMoved.InvokeSafe(Event.current.mousePosition));
            AddMouseHandler(EventType.ContextClick, () => ContextClicked.InvokeSafe());
        }

        void AddMouseHandler(EventType eventType, Action callback)
        {
            if (!_mouseEventMap.ContainsKey(eventType))
                _mouseEventMap.Add(eventType, new List<Action>());

            _mouseEventMap[eventType].Add(callback);
        }

        /// <summary>
        /// Call this inside the OnGUI function of the class to check for any events.
        /// </summary>
        public void ProcessEvents()
        {
            var eventType = Event.current.type;

            if (_mouseEventMap.ContainsKey(eventType))
                _mouseEventMap[eventType].ForEach(x => x.InvokeSafe());

            if (eventType == EventType.KeyDown)
                KeyPressed.InvokeSafe(Event.current.keyCode);

            if (eventType == EventType.KeyUp)
                KeyReleased.InvokeSafe(Event.current.keyCode);

            MousePosition = Event.current.mousePosition;
        }

        public void Destroy()
        {
            _mouseEventMap = null;
            ContextClicked = null;
            MouseLeftClicked = null;
            KeyReleased = null;
            KeyPressed = null;
        }
    }
}