using Share;
using UnityEngine;

namespace Viewer
{
    public class PointSet : MonoBehaviour
    {
        private const string DateFormat = "yyyyMMdd_HHmmssff";

        [SerializeField]
        private Point pointPrefab;

        [SerializeField]
        private IdentifiedPoint[] IdentifiedPoints;

        public PackedMessage.IdentifiedPointArray IdentifiedPointArray { get; private set; }

        public void Init(PackedMessage.IdentifiedPointArray identifiedPointsArray)
        {
            this.IdentifiedPointArray = identifiedPointsArray;
            this.IdentifiedPoints = identifiedPointsArray.Array;
            this.gameObject.name = identifiedPointsArray.Time.ToString(DateFormat);
            var parentTs = this.transform;
            foreach (var identifiedPoint in identifiedPointsArray.Array)
            {
                GameObject.Instantiate(point, identifiedPoint.Position, Quaternion.identity, parentTs);
            }
        }
    }
}
