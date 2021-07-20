using System;
using System.Linq;
using Share;
using UniRx;
using UnityEngine;

namespace Viewer
{
    public class PointDataSupplierMock : MonoBehaviour, IPointDateSupplier
    {
        private readonly Subject<PackedMessage.IdentifiedPointArray> _supply
            = new Subject<PackedMessage.IdentifiedPointArray>();

        public IObservable<PackedMessage.IdentifiedPointArray> OnReceivePointData()
        {
            return _supply;
        }

        [SerializeField]
        private int CreateCount = 20;

        [ContextMenu(nameof(Supply))]
        private void Supply()
        {
            var random = new System.Random();
            var time = DateTimeOffset.Now;

            for (var count = 0; count < CreateCount; count++)
            {
                time = time.AddSeconds(1);

                var array = new PackedMessage.IdentifiedPointArray();
                array.Array = Enumerable.Range(0, random.Next(10, 100))
                    .Select(_ => new IdentifiedPoint
                    {
                        Position = UnityEngine.Random.insideUnitSphere * 5f,
                        Identify = (ulong) random.Next(0, int.MaxValue),
                        Confidence = UnityEngine.Random.value,
                    }).ToArray();
                array.Time = time;

                _supply.OnNext(array);
            }
        }
    }
}
