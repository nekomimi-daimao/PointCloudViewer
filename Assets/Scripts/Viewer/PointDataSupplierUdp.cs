using System;
using Share;
using UnityEngine;

namespace Viewer
{
    public sealed class PointDataSupplierUdp : MonoBehaviour, IPointDataSupplier
    {
        [SerializeField]
        private PackedMessageSupplier packedMessageSupplier;

        public IObservable<PackedMessage.IdentifiedPointArray> OnReceivePointData()
        {
            return packedMessageSupplier.ObservablePointArray;
        }
    }
}
