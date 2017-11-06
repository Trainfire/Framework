﻿using UnityEngine;
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
            constant.PinType = (NodePinType)EditorGUILayout.EnumPopup("Type", constant.PinType);

            const string prefix = "Value";
            switch (constant.PinType)
            {
                case NodePinType.None: EditorGUILayout.LabelField(prefix, "No Type"); break;
                case NodePinType.Int: constant.SetInt(EditorGUILayout.DelayedIntField(prefix, constant.GetInt())); break;
                case NodePinType.Float: constant.SetFloat(EditorGUILayout.DelayedFloatField(prefix, constant.GetFloat())); break;
                case NodePinType.String: constant.SetString(EditorGUILayout.DelayedTextField(prefix, constant.GetString())); break;
                case NodePinType.Bool: constant.SetBool(EditorGUILayout.Toggle(prefix, constant.GetBool())); break;
            }
        }
    }
}
