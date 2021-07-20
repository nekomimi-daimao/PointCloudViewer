using System;
using System.Text;
using Share;
using UniRx;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Device
{
    public class DeviceState : MonoBehaviour
    {
        [SerializeField]
        private AnchorWithMemo _anchorWithMemo = null;

        [SerializeField]
        private PointCloudPresenter _pointCloudPresenter = default;

        private void OnEnable()
        {
            _style = new GUIStyle {fontSize = 20, normal = {textColor = Color.green}};

            var cameraTs = Camera.main?.transform;
            if (cameraTs == null)
            {
                return;
            }
            Observable.EveryUpdate()
                .TakeUntilDisable(gameObject)
                .Subscribe(_ => { _anchorWithMemo.Memo = $"{cameraTs.position}{Environment.NewLine}{cameraTs.eulerAngles}"; });
        }

        private readonly Rect _rect = new Rect(40, 100, Screen.width, Screen.height);
        private GUIStyle _style;

        private void OnGUI()
        {
            GUI.Label(_rect, ToLog(), _style);
        }

        private readonly StringBuilder _stringBuilder = new StringBuilder();

        private string ToLog()
        {
            _stringBuilder.Clear();

            _stringBuilder.AppendLine($"SessionState {ARSession.state}");
            _stringBuilder.AppendLine($"PointCount {_pointCloudPresenter.Count}");

            return _stringBuilder.ToString();
        }
    }
}
