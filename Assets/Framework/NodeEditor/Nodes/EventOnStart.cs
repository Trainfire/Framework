using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.NodeSystem
{
    public class EventOnStart : Node
    {
        protected override void OnInitialize()
        {
            AddExecuteOutPin();
        }
    }
}
