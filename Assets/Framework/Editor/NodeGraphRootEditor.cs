using UnityEngine;
using UnityEditor;

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
            constant.PinType = (NodePinType)EditorGUILayout.EnumPopup("Type", constant.PinType);

            const string prefix = "Value";
            switch (constant.PinType)
            {
                case NodePinType.None: EditorGUILayout.LabelField(prefix, "No Type"); break;
                case NodePinType.Int: constant.SetInt(EditorGUILayout.IntField(prefix, constant.GetInt())); break;
                case NodePinType.Float: constant.SetFloat(EditorGUILayout.FloatField(prefix, constant.GetFloat())); break;
                case NodePinType.String: constant.SetString(EditorGUILayout.TextField(prefix, constant.GetString())); break;
                case NodePinType.Bool: constant.SetBool(EditorGUILayout.Toggle(prefix, constant.GetBool())); break;
            }
        }
    }
}
