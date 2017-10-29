using UnityEngine;

namespace Framework.NodeEditor
{
    [ExecuteInEditMode]
    public class OneOutNode : Node
    {
        [ExecuteInEditMode]
        protected override void OnInitialize()
        {
            AddOutputPin<float>("Out");
        }
    }
}
