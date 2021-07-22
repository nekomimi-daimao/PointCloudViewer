using System;
using Share;
using UnityEngine;

namespace Viewer
{
    public sealed class PointerDataSupplier : MonoBehaviour, IPointDateSupplier
    {
        [SerializeField]
        private PackedMessageSupplier packedMessageSupplier;

        public IObservable<PackedMessage.IdentifiedPointArray> OnReceivePointData()
        {
            return packedMessageSupplier.ObservablePointArray;
        }
    }
}
