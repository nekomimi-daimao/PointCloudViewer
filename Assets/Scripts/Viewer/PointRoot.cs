using System;
using System.Collections.Generic;
using System.Linq;
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
        private GameObject[] pointDataSuppliers = null;

        public IObservable<PackedMessage.IdentifiedPointArray> PointDataSupplier { get; private set; }

        public readonly List<PointSet> PointSets = new List<PointSet>();

        private void Start()
        {
            PointDataSupplier = pointDataSuppliers
                .Select(o => o?.GetComponent<IPointDataSupplier>())
                .Where(supplier => supplier != null)
                .Select((supplier => supplier.OnReceivePointData()))
                .Merge();
            
            PointDataSupplier
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
