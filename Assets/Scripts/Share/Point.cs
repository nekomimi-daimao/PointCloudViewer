using System;
using UnityEngine;

namespace Share
{
    public class Point : MonoBehaviour
    {
        [SerializeField]
        public IdentifiedPoint IdentifiedPoint;

        private Material _material = null;

        public void ChangeColor(Func<IdentifiedPoint, Color> colorRule)
        {
            if (_material == null)
            {
                _material = GetComponent<Renderer>().material;
            }
            _material.color = colorRule(IdentifiedPoint);
        }
    }
}
