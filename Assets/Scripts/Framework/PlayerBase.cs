using UnityEngine;

namespace Framework
{
    /// <summary>
    /// An object that can be controlled.
    /// </summary>
    public interface IPawn
    {
        void HandleInput(InputUpdateEvent inputUpdateEvent);
    }

    public abstract class PlayerBase : GameEntity
    {
        public IPawn Pawn { get; private set; }

        public void Possess(IPawn pawn)
        {
            if (pawn == null)
            {
                DebugEx.LogError<PlayerBase>("Cannot posses a pawn that is null.");
                return;
            }

            Pawn = pawn;

            OnPossess(pawn);
        }

        protected virtual void OnPossess(IPawn pawn) { }

        public override void HandleUpdate(InputUpdateEvent handlerEvent)
        {
            base.HandleUpdate(handlerEvent);

            if (Pawn != null)
                Pawn.HandleInput(handlerEvent);
        }
    }
}
