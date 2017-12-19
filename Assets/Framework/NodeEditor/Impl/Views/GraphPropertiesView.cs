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
        public event Action<AddNodeVariableArgs> AddVariableNode;

        public GraphPropertiesView(NodeGraphHelper graphHelper) : base(graphHelper) { }

        public override void Draw()
        {
            GUILayout.Label("Variables", EditorStyles.boldLabel);

            foreach (var variable in GraphHelper.Variables)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(variable.Name, GUILayout.ExpandWidth(true));
                GUILayout.Label(variable.Type);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Add"))
                AddVariable.InvokeSafe(new AddGraphVariableEvent("Test", typeof(float)));

            if (GraphHelper.Variables.Count > 0)
            {
                if (GUILayout.Button("Remove"))
                    RemoveVariable.InvokeSafe(new RemoveGraphVariableEvent(GraphHelper.Variables[0]));
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            // BEGIN TEMP GARBAGE CODE
            if (GUILayout.Button("Get"))
                AddVariableNode.InvokeSafe(new AddNodeVariableArgs(GraphHelper.Variables[0], NodeGraphVariableAccessorType.Get));

            if (GUILayout.Button("Get Set"))
                AddVariableNode.InvokeSafe(new AddNodeVariableArgs(GraphHelper.Variables[0], NodeGraphVariableAccessorType.GetSet));

            if (GUILayout.Button("Set"))
                AddVariableNode.InvokeSafe(new AddNodeVariableArgs(GraphHelper.Variables[0], NodeGraphVariableAccessorType.Set));
            // END TEMP GARBAGE CODE

            GUILayout.EndHorizontal();
        }
    }
}
