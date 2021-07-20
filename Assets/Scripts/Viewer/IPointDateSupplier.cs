using System;
using Share;

namespace Viewer
{
    public interface IPointDateSupplier
    {
        public IObservable<PackedMessage.IdentifiedPointArray> OnReceivePointData();
    }
}
