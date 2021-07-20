using System;
using System.Text;
using Share;
using UniRx;
using UnityEngine;

public class DeviceViewer : MonoBehaviour
{
    [SerializeField]
    private AnchorWithMemo anchorWithMemo;

    [SerializeField]
    private PackedMessageSupplier packedMessageSupplier;

    private void Start()
    {
        packedMessageSupplier.ObservableDevicePose
            .TakeUntilDestroy(this)
            .ObserveOn(Scheduler.MainThread)
            .Subscribe(pose =>
            {
                anchorWithMemo.SelfTs.SetPositionAndRotation(pose.Position, pose.Rotation);
                anchorWithMemo.Memo = $"{pose.Position}{Environment.NewLine}{pose.Rotation.eulerAngles}";
            });
    }

    private readonly Rect _rect = new Rect(40, 100, Screen.width, Screen.height);
    private GUIStyle _style;

    private void OnGUI()
    {
        // GUI.Label(_rect, ToLog(), _style);
    }

    private readonly StringBuilder _stringBuilder = new StringBuilder();

    private string ToLog()
    {
        _stringBuilder.Clear();


        return _stringBuilder.ToString();
    }
}
