using System;
using UnityEngine;
using UnityEditor;

namespace Framework.NodeEditor
{
    [ExecuteInEditMode]
    public class DebugNodeLog : Node
    {
        [ExecuteInEditMode]
        void OnEnable()
        {
            Debug.Log("Awake");
            AddExecuteInPin(OnExecute);
            AddInputPin<string>("Log");
        }

        void OnExecute()
        {
            var log = (InputPins[0] as NodeValuePin<string>).Value;
            Debug.Log(log);
        }
    }
}
