using UnityEngine;
using UnityEditor;
using Framework.NodeSystem;

namespace Framework.NodeEditor
{
    public static class NodeEditorHelper
    {
        public static Color GetPinColor(NodePin pin)
        {
            return GetPinColor(pin.Type);
        }

        public static Color GetPinColor(NodePinType pinType)
        {
            switch (pinType)
            {
                case NodePinType.Execute: return new Color(0.8f, 0f, 0f);
                case NodePinType.Object: return Color.white;
                case NodePinType.Float: return new Color(0f, 0.8f, 0f);
                case NodePinType.Int: return new Color(0.259f, 0.525f, 0.957f);
                case NodePinType.Bool: return Color.yellow;
                case NodePinType.String: return new Color(0.2f, 0.2f, 0.2f);
                default: return Color.white;
            }
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