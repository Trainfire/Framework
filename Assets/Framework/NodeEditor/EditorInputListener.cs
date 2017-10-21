using UnityEngine;
using UnityEngine.Assertions;
using System;
using System.Collections.Generic;

namespace Framework
{
    public class EditorMouseEvent : EventArgs
    {
        public Vector2 Position { get; private set; }

        public EditorMouseEvent()
        {
            Position = Event.current.mousePosition;
        }
    }

    public class EditorInputListener
    {
        public Vector2 MousePosition { get; private set; }

        public event Action<EditorMouseEvent> MouseLeftClicked;
        public event Action<EditorMouseEvent> MouseLeftReleased;
        public event Action<EditorMouseEvent> MouseDragged;
        public event Action<EditorMouseEvent> MouseMoved;

        public event Action ContextClicked;

        public event Action DeletePressed;
        public event Action<KeyCode> KeyPressed;
        public event Action<KeyCode> KeyReleased;

        private Dictionary<EventType, List<Action>> _mouseEventMap;

        public EditorInputListener()
        {
            _mouseEventMap = new Dictionary<EventType, List<Action>>();

            AddMouseHandler(EventType.MouseDown, () => MouseLeftClicked.InvokeSafe(new EditorMouseEvent()));
            AddMouseHandler(EventType.MouseUp, () => MouseLeftReleased.InvokeSafe(new EditorMouseEvent()));
            AddMouseHandler(EventType.MouseDrag, () => MouseDragged.InvokeSafe(new EditorMouseEvent()));
            AddMouseHandler(EventType.MouseMove, () => MouseMoved.InvokeSafe(new EditorMouseEvent()));
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
            {
                KeyPressed.InvokeSafe(Event.current.keyCode);

                // TODO: Replace with delete command. There is one...somewhere...apparently...*shrug*
                if (Event.current.keyCode == KeyCode.Backspace)
                    DeletePressed.InvokeSafe();
            }

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