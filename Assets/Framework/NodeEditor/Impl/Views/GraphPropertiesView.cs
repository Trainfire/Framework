using UnityEngine;
using UnityEditor;
using NodeSystem;
using System;
using NodeSystem.Editor;

namespace Framework.NodeEditorViews
{
    public class GraphPropertiesView : NodeEditorPropertiesPanel
    {
        public event Action<AddGraphVariableEvent> AddVariable;
        public event Action<RemoveGraphVariableEvent> RemoveVariable;

        public GraphPropertiesView(NodeGraphHelper graphHelper) : base(graphHelper) { }

        public override void Draw()
        {
            GUILayout.Label("Variables", EditorStyles.boldLabel);

            foreach (var variable in GraphHelper.Variables)
            {
                GUILayout.Label(variable.Name);
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add"))
                AddVariable.InvokeSafe(new AddGraphVariableEvent("Test"));

            if (GUILayout.Button("Remove"))
                RemoveVariable.InvokeSafe(new RemoveGraphVariableEvent("Todo"));

            GUILayout.EndHorizontal();
        }
    }
}
