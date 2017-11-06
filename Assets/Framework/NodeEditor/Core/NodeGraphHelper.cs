﻿using System.Collections.Generic;
using System.Linq;

namespace Framework.NodeSystem
{
    /// <summary>
    /// Wrapper to expose graph info safely.
    /// </summary>
    public class NodeGraphHelper
    {
        public int NodeCount { get { return _graph == null ? 0 : _graph.Nodes.Count; } }
        public List<NodeConnection> Connections { get { return _graph != null ? _graph.Connections.ToList() : new List<NodeConnection>(); } }

        private NodeGraph _graph;

        public NodeGraphHelper(NodeGraph graph)
        {
            if (graph == null)
                return;

            _graph = graph;
        }

        public bool IsPinConnected(NodePin pin)
        {
            return Connections.Any(connection => connection.StartPin == pin || connection.EndPin == pin);
        }

        public Node GetStartNode()
        {
            return _graph.Nodes.Find(node => node.GetType() == typeof(EventOnStart));
        }

        public Node GetNode(string nodeId)
        {
            return _graph.Nodes.Find(x => x.ID == nodeId);
        }

        public NodePin GetPin(string nodeId, int pinId)
        {
            var node = GetNode(nodeId);
            if (node != null)
            {
                if (node.HasPin(pinId))
                    return node.Pins[pinId];
            }
            return null;
        }

        public List<T> GetNodes<T>() where T : Node
        {
            return _graph.Nodes.OfType<T>().ToList();
        }

        public static NodeGraphData GetGraphData(NodeGraph graph)
        {
            DebugEx.Log<NodeGraphState>("Serializing graph state...");

            var outGraphData = new NodeGraphData();

            // TODO: Find a nicer way to do this...
            graph.Nodes.ForEach(node =>
            {
                if (node.GetType() == typeof(NodeConstant))
                {
                    outGraphData.Constants.Add(NodeConstantData.Convert(node as NodeConstant));
                }
                else
                {
                    outGraphData.Nodes.Add(NodeData.Convert(node));
                }
            });

            graph.Connections.ForEach(connection =>
            {
                outGraphData.Connections.Add(new NodeConnectionData(connection.StartPin, connection.EndPin));
            });

            return outGraphData;
        }
    }
}
