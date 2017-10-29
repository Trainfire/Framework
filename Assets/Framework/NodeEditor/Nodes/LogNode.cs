using System;
using UnityEngine;
using UnityEditor;

namespace Framework.NodeEditor
{
    [ExecuteInEditMode]
    public class DebugNodeLog : Node
    {
        NodeValuePin<string> _inLog;

        [ExecuteInEditMode]
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
