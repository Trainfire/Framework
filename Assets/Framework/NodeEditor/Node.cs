using UnityEngine;
using System;
using System.Collections.Generic;

namespace Framework.NodeEditor
{
    [ExecuteInEditMode]
    public class Node : MonoBehaviour
    {
        public event Action<Node> Destroyed;

        public string Name { get { return name; } }
        public int ID { get { return GetInstanceID(); } }
        public Vector2 Position { get; set; }

        public Node()
        {
            
        }

        [ExecuteInEditMode]
        public void OnDestroy()
        {
            Destroyed.InvokeSafe(this);
        }
    }
}
