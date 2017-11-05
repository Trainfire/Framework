using System;
using UnityEngine;
using UnityEditor;

namespace Framework.NodeSystem
{
    public class ConversionFloatToString : Node
    {
        private NodeValuePin<float> _in;
        private NodeValuePin<string> _out;

        protected override void OnInitialize()
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
