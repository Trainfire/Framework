using System.Collections.Generic;

namespace Framework.NodeSystem
{
    public abstract class NodeExecute : Node
    {
        public NodePin<NodePinTypeExecute> ExecuteOut { get; private set; }

        protected override void OnInitialize()
        {
            AddExecuteInPin();
            ExecuteOut = AddExecuteOutPin();
        }

        public abstract void Execute();
    }

    public abstract class NodeExecute1In<T> : NodeExecute
    {
        protected NodePin<T> In { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            In = AddInputPin<T>("In");
        }

        public override abstract void Execute();
    }

    public abstract class NodeExecute1In1Out<TIn, TOut> : NodeExecute
    {
        protected NodePin<TIn> In { get; private set; }
        protected NodePin<TOut> Out { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            In = AddInputPin<TIn>("In");
            Out = AddOutputPin<TOut>("Out");
        }

        public override abstract void Execute();
    }

    public abstract class Node1In1Out<TIn, TOut> : Node
    {
        protected NodePin<TIn> In { get; private set; }
        protected NodePin<TOut> Out { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            In = AddInputPin<TIn>("In");
            Out = AddOutputPin<TOut>("Out");
        }
    }

    public abstract class Node2In1Out<TIn1, TIn2, TOut> : Node
    {
        protected NodePin<TIn1> In1 { get; private set; }
        protected NodePin<TIn2> In2 { get; private set; }
        protected NodePin<TOut> Out { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            In1 = AddInputPin<TIn1>("In 1");
            In2 = AddInputPin<TIn2>("In 2");
            Out = AddOutputPin<TOut>("Out");
        }
    }

    public abstract class Node3In1Out<TIn1, TIn2, TIn3, TOut> : Node
    {
        protected NodePin<TIn1> In1 { get; private set; }
        protected NodePin<TIn2> In2 { get; private set; }
        protected NodePin<TIn3> In3 { get; private set; }
        protected NodePin<TOut> Out { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            In1 = AddInputPin<TIn1>("In 1");
            In2 = AddInputPin<TIn2>("In 2");
            In2 = AddInputPin<TIn2>("In 3");
            Out = AddOutputPin<TOut>("Out");
        }
    }
}