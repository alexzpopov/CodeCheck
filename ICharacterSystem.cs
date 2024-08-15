using System;
using Company.Game.Modules.Character.Controllers;
using Company.Game.Modules.Character.Enums;
using Company.Game.Modules.Character.Models;
using Company.Game.Modules.Common;

namespace Company.Game.Modules.Character.Interfaces
{
    public interface ICharacterSystem
    {
        event Action OnPlayerDeath;
        IEntityState<WormState> PlayerEntity { get; }
        PlayerManager Player { get; }
        void SpawnPlayer(int skinIndex, Action<SpawnPlayerResult> completeEvent);
        void RevivePlayer();
    }
}
