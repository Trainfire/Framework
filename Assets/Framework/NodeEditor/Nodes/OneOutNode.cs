using UnityEngine;

namespace Framework.NodeEditor
{
    [ExecuteInEditMode]
    public class OneOutNode : Node
    {
        [ExecuteInEditMode]
        void Awake()
        {
            AddOutputPin<float>("Out");
        }
    }
}
