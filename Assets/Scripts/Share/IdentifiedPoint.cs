using MessagePack;

namespace Share
{
    [System.Serializable]
    [MessagePackObject]
    public class IdentifiedPoint
    {
        [Key(0)]
        public ulong Identify;

        [Key(1)]
        public UnityEngine.Vector3 Position;

        [Key(2)]
        public float Confidence;

        [Key(4)]
        public UnityEngine.Vector3 CameraPosition;

        [Key(5)]
        public UnityEngine.Quaternion CameraRotation;

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
