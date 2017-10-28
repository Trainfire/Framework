using System;
using UnityEngine;
using UnityEditor;

namespace Framework.NodeEditor
{
    [ExecuteInEditMode]
    public class MathNodeAdd : Node
    {
        [ExecuteInEditMode]
        void Awake()
        {
            AddInputPin("In 1", NodePinValueType.Float);
            AddInputPin("In 2", NodePinValueType.Float);
            AddOutputPin("Out", NodePinValueType.Float);
        }

        protected override void OnExecute()
        {
            throw new NotImplementedException();
        }
    }
}
