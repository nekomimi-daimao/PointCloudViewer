using System;
using UnityEngine;

namespace Share
{
    public class Point : MonoBehaviour
    {
        [SerializeField]
        public IdentifiedPoint IdentifiedPoint;

        private Material _material = null;

        public void ChangeColor(Func<IdentifiedPoint, Color> colorScheme)
        {
            if (_material == null)
            {
                _material = GetComponent<Renderer>().material;
            }
            _material.color = colorScheme(IdentifiedPoint);
        }

        private void OnDestroy()
        {
            if (_material != null)
            {
                Destroy(_material);
            }
        }
    }
}
