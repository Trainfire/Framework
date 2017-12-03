using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using NodeSystem;

namespace Framework.NodeEditorViews
{
    public static class NodeEditorColorHelper
    {
        private static Dictionary<Type, Color> _colorRegistry = new Dictionary<Type, Color>
        {
            { typeof(NodePinTypeNone), Color.white },
            { typeof(NodePinTypeExecute), new Color(0.8f, 0f, 0f) },
            { typeof(float), new Color(0f, 0.8f, 0f) },
            { typeof(int), new Color(0.259f, 0.525f, 0.957f) },
            { typeof(bool), Color.yellow },
            { typeof(string), new Color(0.2f, 0.2f, 0.2f) }
        };

        public static Color GetPinColor(NodePin pin)
        {
            return GetPinColor(pin.Type);
        }

        public static Color GetPinColor(NodePinType pinType)
        {
            return _colorRegistry.ContainsKey(pinType.WrappedType) ? _colorRegistry[pinType.WrappedType] : Color.white;
        }
    }

    public static class NodeEditorConnectionDrawer
    {
        public static void Draw(NodeEditorPinView startPin, NodeEditorPinView endPin)
        {
            Draw(GetPinPosition(startPin), GetPinPosition(endPin), NodeEditorColorHelper.GetPinColor(startPin.Pin));
        }

        public static void Draw(NodeEditorPinView startPin, Vector2 endPosition)
        {
            Draw(GetPinPosition(startPin), endPosition, NodeEditorColorHelper.GetPinColor(startPin.Pin));
        }

        public static void Draw(Vector2 start, Vector2 end, Color color)
        {
            var startTangent = new Vector2(end.x, start.y);
            var endTangent = new Vector2(start.x, end.y);
            Handles.BeginGUI();
            Handles.DrawBezier(start, end, startTangent, endTangent, color, null, 2f);
            Handles.EndGUI();
        }

        public static Vector2 GetPinPosition(NodeEditorPinView pin)
        {
            return (pin.ScreenPosition + new Vector2(0f, pin.LocalRect.height * 0.5f));
        }
    }

    public static class NodeEditorPinDrawer
    {
        const float PinSize = 10f;

        public static NodeEditorPinView Draw(NodePin pin, bool highlighted)
        {
            GUILayout.BeginHorizontal();

            // Hack to align the element to the right.
            if (!pin.IsInput())
                GUILayout.FlexibleSpace();

            Rect pinRect = new Rect();

            if (pin.IsInput())
            {
                pinRect = DrawPin(pin, highlighted);
                DrawLabel(pin);
            }
            else
            {
                DrawLabel(pin);
                pinRect = DrawPin(pin, highlighted);
            }

            GUILayout.EndHorizontal();

            return new NodeEditorPinView(pin, pinRect);
        }

        static Rect DrawPin(NodePin pin, bool highlighted)
        {
            var startingBg = GUI.backgroundColor;

            var color = NodeEditorColorHelper.GetPinColor(pin.Type);

            if (highlighted)
            {
                var highlightAdd = 0.4f;
                color = new Color(color.r + highlightAdd, color.g + highlightAdd, color.b + highlightAdd);
            }

            GUI.backgroundColor = color;

            GUILayout.Box("", GUILayout.Width(PinSize), GUILayout.Height(PinSize));

            GUI.backgroundColor = startingBg;

            return GUILayoutUtility.GetLastRect();
        }

        static void DrawLabel(NodePin pin)
        {
            GUILayout.Label(new GUIContent(pin.Name, pin.ToString()));
        }
    }
}