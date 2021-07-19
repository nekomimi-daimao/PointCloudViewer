using MessagePack;
using UnityEngine;

namespace Share
{
    [MessagePackObject]
    public class IdentifiedPoint
    {
        [Key(0)]
        public ulong Identify;

        [Key(1)]
        public Vector3 Position;

        public byte[] Serialize()
        {
            return MessagePackSerializer.Serialize(this);
        }

        public static IdentifiedPoint Deserialize(byte[] buffer)
        {
            return MessagePackSerializer.Deserialize<IdentifiedPoint>(buffer);
        }
    }

    [MessagePackObject]
    public class IdentifiedPointArray
    {
        [Key(0)]
        public IdentifiedPoint[] Array;

        public byte[] Serialize()
        {
            return MessagePackSerializer.Serialize(this);
        }

        public static IdentifiedPointArray Deserialize(byte[] buffer)
        {
            return MessagePackSerializer.Deserialize<IdentifiedPointArray>(buffer);
        }
    }
}
