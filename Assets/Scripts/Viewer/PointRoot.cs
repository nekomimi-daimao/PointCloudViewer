using Share;
using UniRx;
using UnityEngine;

namespace Viewer
{
    public class PointRoot : MonoBehaviour
    {
        [SerializeField]
        private PointSet pointSet;

        [SerializeField]
        private GameObject _pointDataSupplier = null;

        private void Start()
        {
            var pointDateSupplier = _pointDataSupplier.GetComponent<IPointDateSupplier>();
            pointDateSupplier?.OnReceivePointData()
                .TakeUntilDisable(this)
                .Subscribe(OnReceived);
        }

        private void OnReceived(PackedMessage.IdentifiedPointArray array)
        {
            var set = GameObject.Instantiate(pointSet, transform);
            set.Init(array);
        }
    }
}
