using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.NodeEditor
{
    public class NodeEventOnStart : Node
    {
        protected override void OnEnable()
        {
            AddExecuteOutPin();
        }
    }
}
