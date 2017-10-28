using UnityEngine;

namespace Framework.NodeEditor
{
    [ExecuteInEditMode]
    public class OneOutNode : Node
    {
        [ExecuteInEditMode]
        void OnEnable()
        {
            AddOutputPin<float>("Out");
        }
    }
}
