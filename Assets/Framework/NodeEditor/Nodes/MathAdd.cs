using System;
using UnityEngine;
using UnityEditor;

namespace Framework.NodeSystem
{
    public class MathAdd : Node
    {
        protected override void OnInitialize()
        {
            AddInputPin<float>("In 1");
            AddInputPin<float>("In 2");
            AddOutputPin<float>("Result");
        }
    }
}
