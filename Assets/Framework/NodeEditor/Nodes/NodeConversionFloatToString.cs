using System;
using UnityEngine;
using UnityEditor;

namespace Framework.NodeEditor
{
    [ExecuteInEditMode]
    public class NodeConversionFloatToString : Node
    {
        private NodeValuePin<float> _in;
        private NodeValuePin<string> _out;

        [ExecuteInEditMode]
        protected override void OnEnable()
        {
            _in = AddInputPin<float>("In");
            _out = AddOutputPin<string>("Out");

            _in.Value = 64f;
            _out.OnGet += NodeConversionFloatToString_OnGet;
        }

        void NodeConversionFloatToString_OnGet()
        {
            _out.Value = _in.Value.ToString();
        }
    }
}
