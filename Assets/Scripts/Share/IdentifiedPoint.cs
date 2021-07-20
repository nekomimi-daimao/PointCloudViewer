using MessagePack;
using UnityEngine;

namespace Share
{
    [System.Serializable]
    [MessagePackObject]
    public class IdentifiedPoint
    {
        [Key(0)]
        public ulong Identify;

        [Key(1)]
        public Vector3 Position;

        [Key(2)]
        public float Confidence;

        public byte[] Serialize()
        {
            return MessagePackSerializer.Serialize(this);
        }

        public static IdentifiedPoint Deserialize(byte[] buffer)
        {
            return MessagePackSerializer.Deserialize<IdentifiedPoint>(buffer);
        }
    }
}
