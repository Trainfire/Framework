using UnityEngine;
using Framework.NodeSystem;
using UnityEditor;

namespace Framework.NodeEditor.Views
{
    public class NodeEditorPropertiesView : BaseView
    {
        protected override void OnDraw()
        {
            var style = new GUIStyle();
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, new Color(0f, 0f, 0f, 1f));
            tex.Apply();
            style.normal.background = tex;

            GUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins, GUILayout.ExpandHeight(true));

            GUILayout.Label("Contextual Properties", EditorStyles.boldLabel);

            if (GraphHelper.SelectedNode != null)
            {
                EditorGUILayout.LabelField(GraphHelper.SelectedNode.Name);

                if (GraphHelper.SelectedNode.GetType() == typeof(NodeConstant))
                    DrawConstantInspector();
            }

            GUILayout.EndVertical();
        }

        void DrawConstantInspector()
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