using System;
using System.Collections.Generic;
using Share;
using UniRx;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Device
{
    public sealed class PointCloudHolder : MonoBehaviour
    {
        private const int IntervalMsec = 100;

        [SerializeField]
        private ARPointCloudManager pointCloudManager = default;

        private Transform _cameraTs = null;

        public IObservable<ARPointCloudChangedEventArgs> PointChanged =>
            Observable.FromEvent<ARPointCloudChangedEventArgs>(
                    h => pointCloudManager.pointCloudsChanged += h,
                    h => pointCloudManager.pointCloudsChanged -= h
                )
                .ThrottleFirst(TimeSpan.FromMilliseconds(IntervalMsec));

        private readonly List<IdentifiedPoint> _cacheIdentifiedPoint = new List<IdentifiedPoint>();

        public IdentifiedPoint[] CurrentPoints()
        {
            _cacheIdentifiedPoint.Clear();

            if (_cameraTs == null)
            {
                _cameraTs = Camera.main?.transform;
            }
            var cameraPos = _cameraTs?.position ?? Vector3.zero;
            var cameraRot = _cameraTs?.rotation ?? Quaternion.identity;

            var trackable = pointCloudManager.trackables;
            foreach (var pointCloud in trackable)
            {
                if (!pointCloud.identifiers.HasValue || !pointCloud.positions.HasValue || !pointCloud.confidenceValues.HasValue)
                {
                    continue;
                }
                var identifiers = pointCloud.identifiers.Value;
                var position = pointCloud.positions.Value;
                var confidence = pointCloud.confidenceValues.Value;

                for (var count = 0; count < identifiers.Length; count++)
                {
                    _cacheIdentifiedPoint.Add(new IdentifiedPoint
                    {
                        Identify = identifiers[count],
                        Position = position[count],
                        Confidence = confidence[count],
                        CameraPosition = cameraPos,
                        CameraRotation = cameraRot,
                    });
                }
            }
            return _cacheIdentifiedPoint.ToArray();
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (pointCloudManager == null)
            {
                pointCloudManager = FindObjectOfType<ARPointCloudManager>();
            }
        }
#endif
    }
}
