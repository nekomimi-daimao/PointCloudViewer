using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Share
{
    public class TextAnchor : MonoBehaviour
    {
        [SerializeField]
        private Text _text = null;

        public string Text
        {
            get => _text.text;
            set => _text.text = value;
        }

        private Transform _selfTs;
        public Vector3 Position => _selfTs.position;
        public Quaternion Rotation => _selfTs.rotation;

        private Transform _cameraTs = null;

        private void OnEnable()
        {
            _selfTs = this.transform;
            _cameraTs = Camera.main?.transform;

            if (_text == null || _cameraTs == null)
            {
                return;
            }

            var textParentTs = _text.transform.parent;

            Observable.EveryUpdate()
                .TakeUntilDisable(gameObject)
                .Subscribe(_ => { textParentTs.rotation = Quaternion.Euler(0f, -_cameraTs.eulerAngles.y, 0f); });
        }
    }
}
