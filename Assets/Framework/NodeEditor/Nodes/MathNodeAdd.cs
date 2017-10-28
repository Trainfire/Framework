using System;
using UnityEngine;
using UnityEditor;

namespace Framework.NodeEditor
{
    [ExecuteInEditMode]
    public class MathNodeAdd : Node
    {
        [ExecuteInEditMode]
        void OnEnable()
        {
            Debug.Log("Awake");
            AddInputPin<float>("In 1");
            AddInputPin<float>("In 2");
            AddOutputPin<float>("Result");
        }
    }
}
