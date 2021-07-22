using System;
using System.Text;
using Share;
using UniRx;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Device
{
    public sealed class DeviceState : MonoBehaviour
    {
        [SerializeField]
        private AnchorWithMemo anchorWithMemo = null;

        [SerializeField]
        private PointCloudPresenter pointCloudPresenter = default;

        [SerializeField]
        private UDPClientHolder udpClientHolder;

        private void OnEnable()
        {
            _style = new GUIStyle {fontSize = 30, normal = {textColor = Color.green}};

            var cameraTs = Camera.main?.transform;
            if (cameraTs == null)
            {
                return;
            }
            Observable.EveryUpdate()
                .TakeUntilDisable(gameObject)
                .Subscribe(_ => { anchorWithMemo.Memo = $"{cameraTs.position}{Environment.NewLine}{cameraTs.eulerAngles}"; });
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
            _stringBuilder.AppendLine($"PointCount {pointCloudPresenter.Count}");
            foreach (var ipEndPoint in udpClientHolder.SendTarget)
            {
                _stringBuilder.AppendLine($"SendTarget {ipEndPoint}");
            }

            return _stringBuilder.ToString();
        }
    }
}
