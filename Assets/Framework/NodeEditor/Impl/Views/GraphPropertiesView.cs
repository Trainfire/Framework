using UnityEngine;
using UnityEditor;
using NodeSystem;

namespace Framework.NodeEditorViews
{
    public class GraphPropertiesView : NodeEditorPropertiesPanel
    {
        public GraphPropertiesView(NodeGraphHelper graphHelper) : base(graphHelper) { }

        public override void Draw()
        {
            GUILayout.Label("Variables", EditorStyles.boldLabel);

            foreach (var variable in GraphHelper.Variables)
            {
                GUILayout.Label(variable.Name);
            }

            GUILayout.BeginHorizontal();
            GUILayout.Button("Add");
            GUILayout.Button("Remove");
            GUILayout.EndHorizontal();
        }
    }
}
