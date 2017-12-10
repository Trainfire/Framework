using UnityEngine;
using UnityEditor;
using NodeSystem;

namespace Framework.NodeEditorViews
{
    public class ContextualPropertiesView : NodeEditorPropertiesPanel
    {
        public ContextualPropertiesView(NodeGraphHelper graphHelper) : base(graphHelper) { }

        public override void Draw()
        {
            if (GraphHelper.SelectedNode != null)
            {
                EditorGUILayout.LabelField(GraphHelper.SelectedNode.Name);

                if (GraphHelper.SelectedNode.GetType() == typeof(NodeConstant))
                    DrawInspector();
            }
            else
            {
                GUILayout.Label("Nothing selected.", EditorStyles.boldLabel);
            }

            GUILayout.EndVertical();
        }

        void DrawInspector()
        {
            var constant = GraphHelper.SelectedNode as NodeConstant;
            constant.PinType = (NodeConstantType)EditorGUILayout.EnumPopup("Type", constant.PinType);

            const string prefix = "Value";
            switch (constant.PinType)
            {
                case NodeConstantType.None: EditorGUILayout.LabelField(prefix, "No Type"); break;
                case NodeConstantType.Int: constant.SetInt(EditorGUILayout.DelayedIntField(prefix, constant.GetInt())); break;
                case NodeConstantType.Float: constant.SetFloat(EditorGUILayout.DelayedFloatField(prefix, constant.GetFloat())); break;
                case NodeConstantType.String: constant.SetString(EditorGUILayout.DelayedTextField(prefix, constant.GetString())); break;
                case NodeConstantType.Bool: constant.SetBool(EditorGUILayout.Toggle(prefix, constant.GetBool())); break;
            }
        }
    }
}
