using System;
using UnityEngine;
using UnityEditor;

namespace Framework.NodeSystem
{
    public class DebugLog : Node
    {
        NodeValuePin<string> _inLog;

        protected override void OnInitialize()
        {
            AddExecuteInPin(OnExecute);
            _inLog = AddInputPin<string>("Log");
        }

        protected override void OnExecute()
        {
            var log = _inLog.Value;
            Debug.Log(log);
        }
    }
}
