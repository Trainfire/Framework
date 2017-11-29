﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Framework.NodeSystem
{
    /// <summary>
    /// Wrapper to expose graph info safely.
    /// </summary>
    public class NodeGraphHelper
    {
        public event Action<Node> NodeSelected;
        public event Action<Node> NodeAdded;
        public event Action<Node> NodeRemoved;

        public int NodeCount { get { return _graph == null ? 0 : _graph.Nodes.Count; } }
        public List<NodeConnection> Connections { get { return _graph != null ? _graph.Connections.ToList() : new List<NodeConnection>(); } }

        private NodeGraph _graph;

        public NodeGraphHelper(NodeGraph graph)
        {
            if (graph == null)
                return;

            _graph = graph;
            _graph.NodeAdded += Graph_NodeAdded;
            _graph.NodeRemoved += Graph_NodeRemoved;
            _graph.NodeSelected += Graph_NodeSelected;
        }

        void Graph_NodeAdded(Node node) { NodeAdded.InvokeSafe(node); }
        void Graph_NodeRemoved(Node node) { NodeRemoved.InvokeSafe(node); }
        void Graph_NodeSelected(Node node) { NodeSelected.InvokeSafe(node); }

        public bool IsPinConnected(NodePin pin)
        {
            return Connections.Any(connection => connection.StartPin == pin || connection.EndPin == pin);
        }

        public T GetNode<T>() where T : Node
        {
            return _graph.Nodes.Find(x => x.GetType() == typeof(T)) as T;
        }

        public List<T> GetNodes<T>() where T : Node
        {
            return _graph.Nodes.OfType<T>().ToList();
        }

        public Node GetNode(string nodeId)
        {
            return _graph.Nodes.Find(x => x.ID == nodeId);
        }

        public NodeConnection GetConnection(NodePin pin)
        {
            return _graph.Connections.ToList().Where(x => x.StartPin == pin || x.EndPin == pin).FirstOrDefault();
        }

        public List<NodeConnection> GetConnections(NodePin pin)
        {
            return _graph.Connections.ToList().Where(x => x.StartPin == pin || x.EndPin == pin).ToList();
        }

        public NodeConnection GetConnectionFromStartPin(NodePin startPin)
        {
            return _graph.Connections.ToList().Where(x => x.StartPin == startPin).FirstOrDefault();
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
