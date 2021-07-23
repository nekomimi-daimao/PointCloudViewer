using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Share;
using UniRx;
using UnityEngine;

namespace Viewer
{
    public sealed class PointColorScheme : MonoBehaviour
    {
        [SerializeField]
        private PointRoot pointRoot;

        public enum ColorScheme
        {
            White,
            Distance,
            Confidence,
        }

        [SerializeField]
        public ReactiveProperty<ColorScheme> scheme = new ReactiveProperty<ColorScheme>(ColorScheme.White);

        private async void OnEnable()
        {
            scheme.TakeUntilDisable(this)
                .Subscribe(OnSchemeChanged);

            await UniTask.Yield();

            pointRoot.PointDataSupplier?.TakeUntilDisable(this)
                .Throttle(TimeSpan.FromMilliseconds(100))
                .Subscribe(_ => ForceChangeScheme());
        }

        [ContextMenu(nameof(ForceChangeScheme))]
        private void ForceChangeScheme()
        {
            scheme.SetValueAndForceNotify(scheme.Value);
        }

        private void OnSchemeChanged(ColorScheme s)
        {
            var func = Scheme[s];

            var points = pointRoot.PointSets.SelectMany(set => set.Points).ToArray();
            foreach (var p in points)
            {
                p.ChangeColor(func);
            }
        }

        private readonly Dictionary<ColorScheme, Func<IdentifiedPoint, Color>> Scheme
            = new Dictionary<ColorScheme, Func<IdentifiedPoint, Color>>
            {
                {ColorScheme.White, SchemeWhite},
                {ColorScheme.Distance, SchemeDistance},
                {ColorScheme.Confidence, SchemeConfidence},
            };

        private static Color SchemeWhite(IdentifiedPoint point)
        {
            return Color.white;
        }

        private const float HSVs = 0.5f;
        private const float HSVv = 1f;

        private const float DistanceLimit = 5f;

        private static Color SchemeDistance(IdentifiedPoint point)
        {
            var distance = Vector3.Distance(point.Position, point.CameraPosition);
            return Color.HSVToRGB(Mathf.Clamp01(distance / DistanceLimit), HSVs, HSVv);
        }

        private static Color SchemeConfidence(IdentifiedPoint point)
        {
            return Color.HSVToRGB(Mathf.Clamp01(point.Confidence), HSVs, HSVv);
        }
    }
}
