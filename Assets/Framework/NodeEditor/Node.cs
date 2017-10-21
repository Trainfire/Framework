using UnityEngine;
using System.Collections.Generic;

namespace Framework.NodeEditor
{
    public class Node
    {
        public string Name { get; private set; }
        public int ID { get; private set; }
        public Vector2 Position { get; set; }

        public Node()
        {
            Name = "Untitled Node";
        }

        public void SetID(int id)
        {
            ID = id;
        }
    }
}
