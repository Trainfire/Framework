﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.NodeEditor
{
    public class NodeFactory
    {
        private Dictionary<string, Action<NodeGraph>> _nodeRegistry;

        public List<string> Registry { get { return _nodeRegistry.Keys.ToList(); } }

        public NodeFactory()
        {
            _nodeRegistry = new Dictionary<string, Action<NodeGraph>>();
            _nodeRegistry.Add("Math/Add", (graph) => graph.AddNode<MathNodeAdd>("Add"));
            _nodeRegistry.Add("Debug/Log", (graph) => graph.AddNode<DebugNodeLog>("Debug Log"));
            _nodeRegistry.Add("One Out", (graph) => graph.AddNode<OneOutNode>("One Out"));
            _nodeRegistry.Add("Event/On Start", (graph) => graph.AddNode<NodeEventOnStart>("On Start"));
            _nodeRegistry.Add("Conversion/Float to String", (graph) => graph.AddNode<NodeConversionFloatToString>("Float to String"));
        }

        public void Instantiate(string id, NodeGraph graph)
        {
            var containsID = _nodeRegistry.ContainsKey(id);

            if (containsID)
            {
                DebugEx.Log<NodeFactory>("Spawn node '" + id + "'");
                _nodeRegistry[id](graph);
            }
        }
    }
}
