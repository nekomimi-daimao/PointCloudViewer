using System.Collections.Generic;
using Share;
using UniRx;
using UnityEngine;

namespace Viewer
{
    public sealed class PointRoot : MonoBehaviour
    {
        [SerializeField]
        private PointSet pointSetPrefab;

        [SerializeField]
        private GameObject _pointDataSupplier = null;

        public IPointDataSupplier PointDataSupplier { get; private set; }

        public readonly List<PointSet> PointSets = new List<PointSet>();

        private void Start()
        {
            PointDataSupplier = _pointDataSupplier.GetComponent<IPointDataSupplier>();
            PointDataSupplier?.OnReceivePointData()
                .TakeUntilDisable(this)
                .ObserveOn(Scheduler.MainThread)
                .Subscribe(OnReceived);
        }

        private void OnReceived(PackedMessage.IdentifiedPointArray array)
        {
            var set = GameObject.Instantiate(pointSetPrefab, transform);
            set.Init(array);
            PointSets.Add(set);
        }
    }
}
