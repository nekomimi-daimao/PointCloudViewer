using System;
using MessagePack;
using UniRx;
using UnityEngine;

namespace Share
{
    public class PackedMessageSupplier : MonoBehaviour
    {
        [SerializeField]
        private UDPClientHolder clientHolder;

        private readonly Subject<PackedMessage.IdentifiedPointArray> subjectPointArray
            = new Subject<PackedMessage.IdentifiedPointArray>();

        public IObservable<PackedMessage.IdentifiedPointArray> ObservablePointArray => subjectPointArray;

        private readonly Subject<PackedMessage.DevicePose> subjectDevicePose = new Subject<PackedMessage.DevicePose>();
        public IObservable<PackedMessage.DevicePose> ObservableDevicePose => subjectDevicePose;

        private void Start()
        {
            clientHolder.ObservableReceive
                .TakeUntilDestroy(this)
                .Subscribe(OnReceive);
        }

        private void OnReceive(byte[] buffer)
        {
            var deserialized = MessagePackSerializer.Deserialize<PackedMessage.IPackedMessage>(buffer);
            switch (deserialized)
            {
                case PackedMessage.IdentifiedPointArray identifiedPointArray:
                    subjectPointArray.OnNext(identifiedPointArray);
                    break;
                case PackedMessage.DevicePose devicePose:
                    subjectDevicePose.OnNext(devicePose);
                    break;
                default:
                    return;
            }
        }
    }
}
