using System;
using Share;
using UniRx;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

namespace Device
{
    public class PackedMessageSender : MonoBehaviour
    {
        [SerializeField]
        private UDPClientHolder udpClientHolder;

        [SerializeField]
        private PointCloudHolder pointCloudHolder;

        [SerializeField]
        private Transform device;

        private void Awake()
        {
            pointCloudHolder.PointChanged
                .TakeUntilDestroy(this).Subscribe(OnPointChanged);

            Observable.EveryFixedUpdate()
                .TakeUntilDestroy(this)
                .Subscribe(OnDevicePose);
        }

        private void OnPointChanged(ARPointCloudChangedEventArgs _)
        {
            var identifiedPoints = pointCloudHolder.CurrentPoints();
            var array = new PackedMessage.IdentifiedPointArray
            {
                Array = identifiedPoints,
                Time = DateTimeOffset.Now,
            };
            udpClientHolder.Send(array.Serialize());
        }

        private void OnDevicePose(long _)
        {
            var devicePose = new PackedMessage.DevicePose
            {
                Position = device.position,
                Rotation = device.rotation,
            };
            udpClientHolder.Send(devicePose.Serialize());
        }
    }
}
