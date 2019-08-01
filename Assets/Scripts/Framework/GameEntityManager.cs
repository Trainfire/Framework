using System.Collections.Generic;

namespace Framework
{
    class GameEntityManager
    {
        private GameBase _game;
        private List<GameEntity> _entities;

        public GameEntityManager(GameBase game)
        {
            _game = game;
            _game.ZoneListener.ZoneChanged += ZoneListener_ZoneChanged;
        }

        private void ZoneListener_ZoneChanged(GameZone gameZone)
        {
            foreach (var entity in InterfaceHelper.FindObjects<IGameEntity>())
            {
                if (!entity.Initialized)
                    entity.Initialize(_game);
            }
        }
    }
}
