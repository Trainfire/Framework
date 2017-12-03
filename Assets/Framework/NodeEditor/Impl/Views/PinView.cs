using UnityEngine;
using NodeSystem;

namespace Framework.NodeEditorViews
{
    public class NodeEditorPinViewData
    {
        public NodePin Pin { get; private set; }
        public Vector2 ScreenPosition { get; private set; }
        public Rect ScreenRect { get; private set; }
        public Rect LocalRect { get; set; }

        public NodeEditorPinViewData(NodePin pin, Rect localRect)
        {
            Pin = pin;
            LocalRect = localRect;
            ScreenPosition = LocalRect.position + pin.Node.Position;
            ScreenRect = new Rect(ScreenPosition.x, ScreenPosition.y, LocalRect.width, LocalRect.height);
        }
    }

    public class NodeEditorPinView : BaseView
    {
        const float PinSize = 10f;

        public NodeEditorPinViewData Draw(NodePin pin, bool highlighted)
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

            return new NodeEditorPinViewData(pin, pinRect);
        }

        Rect DrawPin(NodePin pin, bool highlighted)
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

            GUI.backgroundColor = startingBg;

            return GUILayoutUtility.GetLastRect();
        }

        void DrawLabel(NodePin pin)
        {
            GUILayout.Label(new GUIContent(pin.Name, pin.ToString()));
        }
    }
}
