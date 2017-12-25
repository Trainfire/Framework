using UnityEngine;
using UnityEditor;
using NodeSystem;
using System;
using System.Collections.Generic;
using NodeSystem.Editor;

namespace Framework.NodeEditorViews
{
    public class GraphPropertiesView : NodeEditorPropertiesPanel
    {
        public event Action<AddGraphVariableEvent> AddVariable;
        public event Action<RemoveGraphVariableEvent> RemoveVariable;
        public event Action<AddNodeVariableArgs> AddVariableNode;

        class VariableViewState
        {
            public bool Foldout { get; set; }

            public VariableViewState() { }
        }

        Dictionary<NodeGraphVariable, VariableViewState> _variableViewStates;

        public GraphPropertiesView(NodeGraphHelper graphHelper) : base(graphHelper)
        {
            _variableViewStates = new Dictionary<NodeGraphVariable, VariableViewState>();
        }

        public override void Draw()
        {
            GUILayout.Label("Variables", EditorStyles.boldLabel);

            GraphHelper.Variables.ForEach(x => DrawVariable(x));

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

        void DrawVariable(NodeGraphVariable variable)
        {
            if (!_variableViewStates.ContainsKey(variable))
                _variableViewStates.Add(variable, new VariableViewState());

            // Todo: Close the UI here.

            var foldOut = _variableViewStates[variable].Foldout;
            _variableViewStates[variable].Foldout = EditorGUILayout.Foldout(foldOut, variable.Name);

            if (foldOut)
            {
                GUILayout.BeginVertical();

                NodeEditorPropertiesHelper.DrawTypeField(variable);
                NodeEditorPropertiesHelper.DrawValueWrapperField(variable.WrappedValue);

                GUILayout.EndVertical();
            }
        }
    }
}
