using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.NodeEditor
{
    public class NodeGraphRunner
    {
        public NodeGraphRunner() { }

        public void Run(NodeGraph graph)
        {
            if (graph == null)
            {
                DebugEx.LogError<NodeGraphRunner>("Cannot run graph as it is null.");
                return;
            }

            var startNode = graph.GetStartNode();

            if (startNode == null)
            {
                DebugEx.LogError<NodeGraphRunner>("Cannot run graph as no start node was found.");
                return;
            }

            // TODO: Weird logic here.Just call StartNode.Execute() ?
            var executePin = startNode.OutputPins[0] as NodeExecutePin;
            if (executePin.ConnectedPin == null)
            {
                DebugEx.LogError<NodeGraphRunner>("Cannot run graph as start node is not connected.");
                return;
            }

            DebugEx.Log<NodeGraphRunner>("Executing...");

            executePin.ConnectedPin.Node.Execute();
        }
    }
}
