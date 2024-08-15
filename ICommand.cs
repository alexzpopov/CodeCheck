using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace Company.Game.Modules.Common
{
    public interface ICommand
    {
        IEnumerator Execute();
    }

    public class CommandBuilder
    {
        Queue<ICommand> commandQueue = new Queue<ICommand>();

        public void ContinueWith(ICommand command)
        {
            commandQueue.Enqueue(command);
        }

        public IEnumerator ExecuteCommands()
        {
            int i = 0;
            while (commandQueue.Count > 0)
            {
                Debug.Log("Command is" + i + " " + commandQueue.Peek().ToString());
                i++;
                var command = commandQueue.Dequeue();

                yield return command.Execute();
            }
        }
    }

    public class EnablePlayerInput : ICommand
    {
        Action<bool> m_Input;

        public EnablePlayerInput(Action<bool> input)
        {
            m_Input = input;
        }

        public virtual IEnumerator Execute()
        {
            m_Input.Invoke(true);
            yield return null;
        }
    }

    public class DisablePlayerInput : ICommand
    {
        Action<bool> m_Input;

        public DisablePlayerInput(Action<bool> input)
        {
            m_Input = input;
        }

        public IEnumerator Execute()
        {
            m_Input.Invoke(false);
            yield return null;
        }
    }

    public class EnableCameraCinematic : ICommand
    {
        Action<bool> m_Input;

        public EnableCameraCinematic(Action<bool> input)
        {
            m_Input = input;
        }

        public IEnumerator Execute()
        {
            m_Input.Invoke(true);
            yield return null;
        }
    }

    public class DisableCameraCinematic : ICommand
    {
        Action<bool> m_Input;

        public DisableCameraCinematic(Action<bool> input)
        {
            m_Input = input;
        }

        public IEnumerator Execute()
        {
            m_Input.Invoke(false);
            yield return null;
        }
    }

    public class DisableWormInvincible : ICommand
    {
        IInvincible m_Input;

        public DisableWormInvincible(IInvincible input)
        {
            m_Input = input;
        }

        public IEnumerator Execute()
        {
            m_Input.ChangeInvincibleState(true);
            yield return null;
        }
    }

    public class EnableWormInvincible : ICommand
    {
        IInvincible m_Input;

        public EnableWormInvincible(IInvincible input)
        {
            m_Input = input;
        }

        public IEnumerator Execute()
        {
            m_Input.ChangeInvincibleState(false);
            yield return null;
        }
    }

    public class FindGatesAndFollow : ICommand
    {
        bool onFinished;
        Func<List<IInteractiveObject>, Func<Vector3?, (Vector3, Vector3)>, int, IEnumerator> m_MoveOpenGateFunction;
        List<IInteractiveObject> m_Gates;
        int m_Level;
        Func<Vector3?, (Vector3 position, Vector3 eulerAngles)> m_CamPosition;
        Action<Vector3> GetStartPosition;

        public FindGatesAndFollow(Func<List<IInteractiveObject>, Func<Vector3?, (Vector3, Vector3)>, int, IEnumerator> moveOpenGateFunction, List<IInteractiveObject> gates, Func<Vector3?, (Vector3 position, Vector3 eulerAngles)> camPosition, int level)
        {
            m_MoveOpenGateFunction = moveOpenGateFunction;
            m_Level = level;
            m_CamPosition = camPosition;
            m_Gates = gates;
            onFinished = false;
        }

        public IEnumerator Execute()
        {
            yield return m_MoveOpenGateFunction.Invoke(m_Gates, m_CamPosition, m_Level);
        }

        public void OnFinished()
        {
            onFinished = false;
        }
    }
}
