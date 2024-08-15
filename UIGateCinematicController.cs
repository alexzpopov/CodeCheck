using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Company.Game.Modules.Common;
using UnityEngine;

namespace Company.Game.Modules.UI.Controllers.SceneUiControllers
{
    public class UIGateCinematicController : MonoBehaviour
    {
        [SerializeField] int debugLevel;
        public event Action<int> OnDebugLevelChanged;
        [SerializeField] GameObject[] canvasElements;
        [SerializeField] GameObject[] canvasHideElements;
        [ContextMenu("Show Gate by Number")]
        public void DebugMove()
        {
            OnDebugLevelChanged?.Invoke(debugLevel);
        }

        public void ShowElements(bool state)
        {
            foreach (var element in canvasElements)
            {
                element.SetActive(state);
            }
            foreach (var element in canvasHideElements)
            {
                element.SetActive(!state);
            }
        }

        public void StartCinema(CommandBuilder commandBuilder)
        {
            StartCoroutine(commandBuilder.ExecuteCommands());
        }

        public IEnumerator InvokeGateCameraMove(List<IInteractiveObject> environmentGates, Func<Vector3?, (Vector3 position, Vector3 eulerAngles)> camPosition, int level)
        {
            int currentLevel = level + 1; // UserData.Level + 1;
            Vector3 pos = camPosition(null).position;
            var minIndexs = environmentGates
                .Select((gate, index) => new { Distance = Vector3.Distance(gate.Position(), pos), Index = index, Level = gate.RequiredPlayerLevel() })
                .Where(item => item.Level == currentLevel)
                .OrderBy(item => item.Distance)
                .Select(item => item.Index)
                .ToList();

            List<(Vector3, Action<IInteractiveObject>, IInteractiveObject)> gates = new();
            foreach (var minIndex in minIndexs)
            {
                Vector3 gatePos = environmentGates[minIndex].Position();
                gatePos = new Vector3(gatePos.x, Camera.main.transform.position.y, gatePos.z);
                var index = minIndex;
                IInteractiveObject gate = environmentGates[index];
                gates.Add((gatePos, (gate) => StartCoroutine(OpenGateAsync(gate)), gate));
            }

            yield return StartCoroutine(MoveCameraToGatesAsync(gates, pos, camPosition));
        }

        IEnumerator OpenGateAsync(IInteractiveObject gate)
        {
            gate.Open();

            yield return null;
        }

        public IEnumerator MoveCameraToGatesAsync(List<(Vector3, Action<IInteractiveObject>, IInteractiveObject)> objects, Vector3 oldPosition, Func<Vector3?, (Vector3, Vector3)> camPosition)
        {
            foreach (var obj in objects)
            {
                yield return StartCoroutine(MoveCameraToGateAsync(obj.Item1, obj.Item2, obj.Item3, false, camPosition));
            }

            yield return StartCoroutine(MoveCameraToGateAsync(oldPosition, null, null, true, camPosition));
        }

        public IEnumerator MoveCameraToGateAsync(Vector3 oldPosition, Action<IInteractiveObject> waitAction, IInteractiveObject index, bool isFinish, Func<Vector3?, (Vector3 position, Vector3 eulerAngles)> camPosition, float beforeObj = 0.5f, float waitTime = 1f, float speed = 16)
        {
            float elapsedTime = 0f;
            float smoothTime = 3;
            var camTransform = camPosition(null);
            if (!isFinish)
            {
                float angleRadians = Mathf.Deg2Rad * camTransform.eulerAngles.x;
                float forwardCamDistance = camTransform.position.y / Mathf.Tan(angleRadians);

                oldPosition = new Vector3(oldPosition.x, oldPosition.y, oldPosition.z - forwardCamDistance);
            }

            while (Mathf.Abs(Vector3.Distance(camTransform.position, oldPosition)) > 0.1f)
            {
                elapsedTime += Time.deltaTime;
                camTransform = camPosition(Vector3.MoveTowards(camTransform.position, oldPosition, speed * Time.deltaTime));
                yield return null;
            }

            if (!isFinish)
            {
                yield return new WaitForSeconds(beforeObj);
            }

            waitAction?.Invoke(index);
            if (!isFinish)
            {
                yield return new WaitForSeconds(waitTime);
            }
        }
    }
}
