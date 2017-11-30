using Framework.NodeSystem;

namespace Framework.NodeEditor.Views
{
    public abstract class BaseView
    {
        protected EditorInputListener InputListener { get; private set; }

        public BaseView()
        {
            InputListener = new EditorInputListener();
        }

        public void Draw()
        {
            InputListener.ProcessEvents();
            OnDraw();
        }

        protected abstract void OnDraw();

        public void Destroy()
        {
            OnDestroy();
        }

        protected virtual void OnDestroy()
        {

        }
    }
}
