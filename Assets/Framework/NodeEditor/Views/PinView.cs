using UnityEngine;
using UnityEditor;
using Framework.NodeSystem;

namespace Framework.NodeEditor.Views
{
    public class NodeEditorPinView : BaseView
    {
        const float PinSize = 10f;

        public void Draw(NodePin pin, bool highlighted)
        {
            GUILayout.BeginHorizontal();

            // Hack to align the element to the right.
            if (!pin.IsInput())
                GUILayout.FlexibleSpace();

            if (pin.IsInput())
            {
                DrawPin(pin, highlighted);
                DrawLabel(pin);
            }
            else
            {
                DrawLabel(pin);
                DrawPin(pin, highlighted);
            }

            GUILayout.EndHorizontal();
        }

        void DrawPin(NodePin pin, bool highlighted)
        {
            var startingBg = GUI.backgroundColor;

            var color = NodeEditorHelper.GetPinColor(pin.Type);

            if (highlighted)
            {
                var highlightAdd = 0.4f;
                color = new Color(color.r + highlightAdd, color.g + highlightAdd, color.b + highlightAdd);
            }

            GUI.backgroundColor = color;

            GUILayout.Box("", GUILayout.Width(PinSize), GUILayout.Height(PinSize));

            // Only cache Rect on Repaint event.
            if (Event.current.type == EventType.Repaint)
                pin.LocalRect = GUILayoutUtility.GetLastRect();

            GUI.backgroundColor = startingBg;
        }

        void DrawLabel(NodePin pin)
        {
            GUILayout.Label(new GUIContent(pin.Name, pin.ToString()));
        }
    }
}
