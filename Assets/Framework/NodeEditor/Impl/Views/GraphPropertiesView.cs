using UnityEngine;
using UnityEditor;
using NodeSystem;
using System;
using System.Linq;
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

            GraphHelper.Variables.ForEach(variable => DrawVariable(variable));

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
            GUILayout.BeginHorizontal();

            GUILayout.Label(variable.Name, GUILayout.MinWidth(100f));

            // Draw type dropdown.
            var pinTypeData = NodePinTypeRegistry.Get(variable.WrappedType);

            if (pinTypeData != null)
            {
                var selectedIndex = NodePinTypeRegistry.AllPinTypes.IndexOf(pinTypeData);
                var dropdownOptions = NodePinTypeRegistry.AllPinTypeNames.ToArray();

                selectedIndex = EditorGUILayout.Popup(selectedIndex, dropdownOptions);

                var selectedPinType = NodePinTypeRegistry.AllPinTypes[selectedIndex];
                variable.SetValueWrapperFromType(selectedPinType.WrappedType);
            }

            // Draw field editor.
            var fieldValue = variable.WrappedValue.ToString();

            if (variable.WrappedType == typeof(float))
            {
                var variableAsType = variable.WrappedValue as NodeValueWrapper<float>;
                variableAsType.Set(EditorGUILayout.FloatField(variableAsType.Value));
            }
            else if (variable.WrappedType == typeof(int))
            {
                var variableAsType = variable.WrappedValue as NodeValueWrapper<int>;
                variableAsType.Set(EditorGUILayout.IntField(variableAsType.Value));
            }
            else if (variable.WrappedType == typeof(bool))
            {
                var variableAsType = variable.WrappedValue as NodeValueWrapper<bool>;
                variableAsType.Set(EditorGUILayout.Toggle(variableAsType.Value));
            }
            else if (variable.WrappedType == typeof(string))
            {
                var variableAsType = variable.WrappedValue as NodeValueWrapper<string>;
                variableAsType.Set(EditorGUILayout.TextField(variableAsType.Value));
            }

            GUILayout.EndHorizontal();
        }
    }
}
