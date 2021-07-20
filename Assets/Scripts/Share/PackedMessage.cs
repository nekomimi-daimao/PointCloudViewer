using System;
using MessagePack;
using UnityEngine;

namespace Share
{
    public class PackedMessage
    {
        [Union(0, typeof(IdentifiedPointArray))]
        [Union(1, typeof(DevicePose))]
        public interface IPackedMessage
        {
            // NOP
        }

        [MessagePackObject]
        public class IdentifiedPointArray : IPackedMessage
        {
            [Key(0)]
            public IdentifiedPoint[] Array;

            [Key(1)]
            public DateTimeOffset Time;

            public byte[] Serialize()
            {
                return MessagePackSerializer.Serialize(this);
            }

            public static IdentifiedPointArray Deserialize(byte[] buffer)
            {
                return MessagePackSerializer.Deserialize<IdentifiedPointArray>(buffer);
            }
        }

        [MessagePackObject]
        public class DevicePose : IPackedMessage
        {
            [Key(0)]
            public Vector3 Position;

            [Key(1)]
            public Quaternion Rotation;

            public byte[] Serialize()
            {
                return MessagePackSerializer.Serialize(this);
            }

            public static DevicePose Deserialize(byte[] buffer)
            {
                return MessagePackSerializer.Deserialize<DevicePose>(buffer);
            }
        }
    }
}
