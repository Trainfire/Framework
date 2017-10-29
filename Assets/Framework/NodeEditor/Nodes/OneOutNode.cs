using UnityEngine;

namespace Framework.NodeEditor
{
    [ExecuteInEditMode]
    public class OneOutNode : Node
    {
        [ExecuteInEditMode]
        protected override void OnEnable()
        {
            AddOutputPin<float>("Out");
        }
    }
}
