using UnityEngine;
using UnityEditor;

namespace Framework.NodeEditor
{
    class NodeConnectionView : View
    {
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }

        protected override void OnDraw()
        {
            Handles.BeginGUI();
            Handles.DrawAAPolyLine(5f, Start, End);
            Handles.EndGUI();
        }
    }
}
