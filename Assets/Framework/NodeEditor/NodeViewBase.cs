using UnityEngine;
using UnityEditor;

namespace Framework.NodeEditor
{
    public abstract class View
    {
        protected EditorInputListener InputListener { get; private set; }

        public View()
        {
            InputListener = new EditorInputListener();
        }

        public void Draw()
        {
            InputListener.ProcessEvents();
            OnDraw();
        }

        protected abstract void OnDraw();
    }
}
