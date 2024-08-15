using System;
using System.Collections.Generic;
using Company.Game.Modules.Camera;
using Company.Game.Modules.Common;
using Company.Game.Modules.UI;
using Company.Game.Modules.UI.Controllers.SceneUiControllers;
using Company.Game.Modules.UserData;
using UnityEngine;

namespace Company.Game.Modules.GateCinematic
{
    public class GateCinematic : IGateCinematic
    {
        public List<IInteractiveObject> m_EnvironmentGates;
        IUserData m_UserData;
        IUiSystem m_UISystem;
        UIGateCinematicController m_UIGateCinematicControlleriGate;
        ICameraSystem m_Camera;
        IInvincible m_WormInvincible;

        public GateCinematic(IUiSystem uiSystem, IUserData userData, ICameraSystem camera,IInvincible wormInvincible)
        {
            m_UserData = userData;
            m_UISystem = uiSystem;
            m_Camera = camera;
            m_WormInvincible = wormInvincible;
        }

        public void Init()
        {
            m_UserData.OnLevelChanged += ShowGateCinematic;
            m_UIGateCinematicControlleriGate = m_UISystem.GetUIGateCinematicController();
            m_UIGateCinematicControlleriGate.OnDebugLevelChanged += ShowGateCinematic;
        }

        public void RegisterGates(List<IInteractiveObject> gates)
        {
            m_EnvironmentGates = gates;
        }

        void ShowGateCinematic(int level)
        {
            CommandBuilder commandBuilder = new CommandBuilder();
            commandBuilder.ContinueWith(new DisableCameraCinematic(m_Camera.ActiveCamera.EnableCineMachineBrain));
            commandBuilder.ContinueWith(new DisablePlayerInput(m_UIGateCinematicControlleriGate.ShowElements));//m_UISystem.GetGameSceneUiController(). SetJoystickVisible));
            commandBuilder.ContinueWith(new DisableWormInvincible(m_WormInvincible));
            commandBuilder.ContinueWith(new FindGatesAndFollow(m_UIGateCinematicControlleriGate.InvokeGateCameraMove, m_EnvironmentGates, m_Camera.ActiveCamera.GetPosition(), level));
            commandBuilder.ContinueWith(new EnableWormInvincible(m_WormInvincible));
            commandBuilder.ContinueWith(new EnableCameraCinematic(m_Camera.ActiveCamera.EnableCineMachineBrain));
            commandBuilder.ContinueWith(new EnablePlayerInput(m_UIGateCinematicControlleriGate.ShowElements));//m_UISystem.GetGameSceneUiController().SetJoystickVisible));
            m_UIGateCinematicControlleriGate.StartCinema(commandBuilder);
        }
    }
}
