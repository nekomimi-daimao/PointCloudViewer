using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Device
{
    public class PointCloudPresenter : MonoBehaviour
    {
        [SerializeField]
        private Transform prefab;

        [SerializeField]
        private PointCloudHolder pointCloudHolder;

        private PointViewPool pointViewPool;
        private readonly Dictionary<ulong, Transform> _points = new Dictionary<ulong, Transform>();
        public int Count => _points.Count;

        private void Start()
        {
            pointViewPool = new PointViewPool(prefab, this.transform);
            pointViewPool.PreloadAsync(100, 10).Subscribe();
            pointCloudHolder.PointChanged
                .TakeUntilDestroy(this).Subscribe(OnChanged);
        }

        private void OnChanged(ARPointCloudChangedEventArgs arg)
        {
            var reserved = _points.Keys;
            var identifiedPoints = pointCloudHolder.CurrentPoints();

            foreach (var l in reserved)
            {
                if (identifiedPoints.Any(point => point.Identify == l))
                {
                    continue;
                }
                pointViewPool.Return(_points[l]);
                _points.Remove(l);
            }

            foreach (var identifiedPoint in identifiedPoints)
            {
                if (_points.TryGetValue(identifiedPoint.Identify, out var target))
                {
                    target.position = identifiedPoint.Position;
                    continue;
                }
                var ts = pointViewPool.Rent();
                ts.position = identifiedPoint.Position;
                _points[identifiedPoint.Identify] = ts;
            }
        }

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (pointCloudHolder == null)
            {
                pointCloudHolder = FindObjectOfType<PointCloudHolder>();
            }
        }
#endif
    }
}
