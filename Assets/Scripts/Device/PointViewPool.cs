using UniRx.Toolkit;
using UnityEngine;

namespace Device
{
    public sealed class PointViewPool : ObjectPool<Transform>
    {
        private readonly Transform _prefab = default;
        private readonly Transform _parentTs = default;

        public PointViewPool(Transform prefab, Transform parentTs)
        {
            _prefab = prefab;
            _parentTs = parentTs;
        }

        protected override Transform CreateInstance()
        {
            return GameObject.Instantiate(_prefab, _parentTs);
        }
    }
}
