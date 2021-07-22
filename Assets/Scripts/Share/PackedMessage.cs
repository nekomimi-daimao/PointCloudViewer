using System;
using MessagePack;
using MessagePack.Resolvers;

namespace Share
{
    public static class PackedMessage
    {
        [UnityEngine.RuntimeInitializeOnLoadMethod]
        private static void Register()
        {
            StaticCompositeResolver.Instance.Register(
                MessagePack.Resolvers.GeneratedResolver.Instance,
                MessagePack.Resolvers.StandardResolver.Instance
            );

            var option = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);

            MessagePackSerializer.DefaultOptions = option;
        }


        [Union(0, typeof(IdentifiedPointArray))]
        [Union(1, typeof(DevicePose))]
        public interface IPackedMessage
        {
            // NOP
        }

        [MessagePackObject]
        public sealed class IdentifiedPointArray : IPackedMessage
        {
            [Key(0)]
            public IdentifiedPoint[] Array;

            [Key(1)]
            public DateTimeOffset Time;

            public byte[] Serialize()
            {
                return MessagePackSerializer.Serialize<IPackedMessage>(this);
            }

            public static IdentifiedPointArray Deserialize(byte[] buffer)
            {
                return MessagePackSerializer.Deserialize<IdentifiedPointArray>(buffer);
            }
        }

        [MessagePackObject]
        public sealed class DevicePose : IPackedMessage
        {
            [Key(0)]
            public UnityEngine.Vector3 Position;

            [Key(1)]
            public UnityEngine.Quaternion Rotation;

            public byte[] Serialize()
            {
                return MessagePackSerializer.Serialize<IPackedMessage>(this);
            }

            public static DevicePose Deserialize(byte[] buffer)
            {
                return MessagePackSerializer.Deserialize<DevicePose>(buffer);
            }
        }
    }
}
