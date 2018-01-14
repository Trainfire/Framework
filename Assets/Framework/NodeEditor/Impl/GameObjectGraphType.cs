using UnityEngine;
using NodeSystem;
using System;

namespace Framework
{
    public class GameObjectGraphType : NodeGraphType
    {
        public GameObjectGraphType() : base()
        {
            const string events = "Events";
            RegisterNodeType<NodeGraphEvent>("Awake", events);
            RegisterNodeType<NodeGraphEvent>("Start", events);
            RegisterNodeType<NodeGraphEvent>("Update", events);

            const string transform = "Transform";
            RegisterNodeType<GameObjectGetPosition>("Get Position", transform);
            RegisterNodeType<GameObjectSetPosition>("Set Position", transform);
        }
    }

    public class GameObjectNode : Node
    {
        public GameObject GameObject { protected get; set; }
    }

    public class GameObjectGetPosition : GameObjectNode
    {
        private NodePin<Vector3> _out;

        protected override void OnInitialize()
        {
            AddOutputPin<Vector3>("Out");
        }

        public override void Calculate()
        {
            Write(_out, GameObject.transform.position);
        }
    }

    public class GameObjectSetPosition : GameObjectNode
    {
        private NodePin<Vector3> _in;

        protected override void OnInitialize()
        {
            _in = AddInputPin<Vector3>("In");
        }

        public override void Calculate()
        {
            var inPosition = Read<Vector3>(_in);
            GameObject.transform.position = inPosition;
        }
    }
}
