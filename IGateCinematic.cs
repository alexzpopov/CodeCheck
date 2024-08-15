using System;
using System.Collections.Generic;
using Company.Game.Modules.Environment.TollBarriers;

namespace Company.Game.Modules.GateCinematic
{
    public interface IGateCinematic
    {
        void Init();
        void RegisterGates(List<IInteractiveObject> gates);
    }
}
