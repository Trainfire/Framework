using UnityEngine;
using UnityEditor;
using Framework.NodeSystem;

namespace Framework.NodeEditor
{
    [CustomEditor(typeof(NodeGraphRoot))]
    public class NodeGraphRootEditor : Editor
    {
        private NodeGraphRoot _root;

        public override void OnInspectorGUI()
        {
            _root = target as NodeGraphRoot;

            if (_root.Selection != null)
            {
                EditorGUILayout.LabelField(_root.Selection.Name, EditorStyles.boldLabel);

                if (_root.Selection.GetType() == typeof(NodeConstant))
                    DrawConstantInspector();
            }
        }

        void DrawConstantInspector()
        {
            var constant = _root.Selection as NodeConstant;
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
