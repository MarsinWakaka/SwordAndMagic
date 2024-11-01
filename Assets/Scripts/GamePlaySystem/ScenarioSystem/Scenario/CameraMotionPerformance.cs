using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace GamePlaySystem.ScenarioSystem.Scenario
{
    public class CameraMotionPerformance : BasePerformance
    {
        [SerializeField] private Camera motionCamera;
        private Transform _cameraTransform;

        [SerializeField] private Vector3 startPos;
        [SerializeField] private Vector3 endPos;
        [SerializeField] private float duration = 2f;
        [SerializeField] private bool shouldReturn;

        private void Awake()
        {
            _cameraTransform = motionCamera.transform;
        }

        public override IEnumerator StartPerformance()
        {
            var wait = new WaitForSeconds(duration);
            _cameraTransform.position = startPos;
            // Camera motion
            _cameraTransform.DOMove(endPos, duration);
            yield return wait;
            if (shouldReturn)
            {
                _cameraTransform.DOMove(startPos, duration);
                yield return wait;
            }
        }
    }
}