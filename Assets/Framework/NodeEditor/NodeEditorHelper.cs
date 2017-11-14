using UnityEngine;
using Framework.NodeSystem;

namespace Framework.NodeEditor
{
    public static class NodeEditorHelper
    {
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
    }
}