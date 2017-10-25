using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.NodeEditor
{
    public class NodeGraphInfo
    {
        public int NodeCount { get; private set; }

        public NodeGraphInfo(NodeGraph graph)
        {
            if (graph == null)
                return;

            // NB: Might be expanded. Use this for debug info.
            NodeCount = graph.Nodes.Count;
        }
    }
}
