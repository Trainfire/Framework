using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Framework.NodeSystem;
using System;

namespace Framework.NodeEditor
{
    public static class NodeEditorHelper
    {
        private static Dictionary<Type, Color> _colorRegistry;

        static NodeEditorHelper()
        {
            _colorRegistry = new Dictionary<Type, Color>();
            _colorRegistry.Add(typeof(NodePinTypeNone), Color.white);
            _colorRegistry.Add(typeof(NodePinTypeExecute), new Color(0.8f, 0f, 0f));
            _colorRegistry.Add(typeof(float), new Color(0f, 0.8f, 0f));
            _colorRegistry.Add(typeof(int), new Color(0.259f, 0.525f, 0.957f));
            _colorRegistry.Add(typeof(bool), Color.yellow);
            _colorRegistry.Add(typeof(string), new Color(0.2f, 0.2f, 0.2f));
        }

        public static Color GetPinColor(NodePin pin)
        {
            return GetPinColor(pin.Type);
        }

        public static Color GetPinColor(NodePinType pinType)
        {
            return _colorRegistry.ContainsKey(pinType.WrappedType) ? _colorRegistry[pinType.WrappedType] : Color.white;
        }

        public static void DrawConnection(NodeConnection connection)
        {
            if (connection.Hidden)
                return;

            var start = GetPinPosition(connection.StartPin);
            var end = GetPinPosition(connection.EndPin);
            var color = NodeEditorHelper.GetPinColor(connection.StartPin.Type);
            DrawConnection(start, end, color);
        }

        public static void DrawConnection(NodePin startPin, Vector2 screenPosition)
        {
            DrawConnection(GetPinPosition(startPin), screenPosition, GetPinColor(startPin));
        }

        public static void DrawConnection(Vector2 start, Vector2 end, Color color)
        {
            var startTangent = new Vector2(end.x, start.y);
            var endTangent = new Vector2(start.x, end.y);
            Handles.BeginGUI();
            Handles.DrawBezier(start, end, startTangent, endTangent, color, null, 2f);
            Handles.EndGUI();
        }

        public static Vector2 GetPinPosition(NodePin pin)
        {
            return (pin.ScreenPosition + new Vector2(0f, pin.LocalRect.height * 0.5f));
        }
    }
}