using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Share
{
    public class AnchorWithMemo : MonoBehaviour
    {
        [SerializeField]
        private Text _text = null;

        public string Memo
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
                .Subscribe(_ =>
                {
                    var look = _cameraTs.position - Position;
                    if (look != Vector3.zero)
                    {
                        textParentTs.rotation = Quaternion.LookRotation(look, Vector3.up);
                    }
                });
        }
    }
}
