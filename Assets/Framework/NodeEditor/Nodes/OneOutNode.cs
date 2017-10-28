using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Framework.NodeEditor
{
    public class OneOutNode : Node
    {
        void Awake()
        {
            AddOutputPin<float>("Out");
        }
    }
}
