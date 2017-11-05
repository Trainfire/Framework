using UnityEngine;

namespace Framework.NodeEditor
{
    public class NodeGraphRoot : MonoBehaviour
    {
        [SerializeField]
        public NodeGraphData GraphData;

        public Node Selection { get; set; }
    }
}
