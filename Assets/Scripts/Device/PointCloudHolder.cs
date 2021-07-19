using System;
using System.Collections.Generic;
using Share;
using UniRx;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Device
{
    public class PointCloudHolder : MonoBehaviour
    {
        private const int IntervalMsec = 100;

        [SerializeField]
        private ARPointCloudManager pointCloudManager = default;

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

            var trackable = pointCloudManager.trackables;
            foreach (var pointCloud in trackable)
            {
                if (!pointCloud.identifiers.HasValue || !pointCloud.positions.HasValue)
                {
                    continue;
                }
                var identifiers = pointCloud.identifiers.Value;
                var pos = pointCloud.positions.Value;

                for (var count = 0; count < identifiers.Length; count++)
                {
                    _cacheIdentifiedPoint.Add(new IdentifiedPoint
                    {
                        Identify = identifiers[count],
                        Position = pos[count]
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