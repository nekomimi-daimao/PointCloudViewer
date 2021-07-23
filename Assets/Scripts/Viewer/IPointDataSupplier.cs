using System;
using Share;

namespace Viewer
{
    public interface IPointDataSupplier
    {
        public IObservable<PackedMessage.IdentifiedPointArray> OnReceivePointData();
    }
}
