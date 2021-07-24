using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Share;
using UniRx;
using UnityEngine;

namespace Viewer
{
    public sealed class PointViewScheme : MonoBehaviour
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
        public ReactiveProperty<ColorScheme> Scheme = new ReactiveProperty<ColorScheme>(ColorScheme.White);

        [SerializeField]
        public FloatReactiveProperty Distance = new FloatReactiveProperty(5f);

        [SerializeField]
        public FloatReactiveProperty Confidence = new FloatReactiveProperty(0.5f);

        private async void OnEnable()
        {
            Scheme.TakeUntilDisable(this)
                .Subscribe(OnSchemeChanged);

            Distance.Merge(Confidence)
                .TakeUntilDisable(this)
                .Select(f => Scheme.Value)
                .Subscribe(OnSchemeChanged);

            ForceChangeSubject.TakeUntilDisable(this)
                .Throttle(TimeSpan.FromMilliseconds(2000))
                .Subscribe(_ => ForceChangeScheme());

            await UniTask.Yield();

            pointRoot.PointDataSupplier?.TakeUntilDisable(this)
                .Subscribe(_ => ForceChangeSubject.OnNext(Unit.Default));
        }

        private readonly Subject<Unit> ForceChangeSubject = new Subject<Unit>();

        [ContextMenu(nameof(ForceChangeScheme))]
        private void ForceChangeScheme()
        {
            Scheme.SetValueAndForceNotify(Scheme.Value);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ForceChangeSubject.OnNext(Unit.Default);
        }
#endif

        private void OnSchemeChanged(ColorScheme s)
        {
            var colorScheme = SchemeTable[s];

            var points = pointRoot.PointSets.SelectMany(set => set.Points).ToArray();
            foreach (var p in points)
            {
                p.ChangeFilter(FilterScheme);
                p.ChangeColor(colorScheme);
            }
        }

        private bool FilterScheme(IdentifiedPoint arg)
        {
            return arg.Confidence > Confidence.Value
                   && Vector3.SqrMagnitude(arg.Position - arg.CameraPosition) <= Mathf.Pow(Distance.Value, 2);
        }

        private readonly Dictionary<ColorScheme, Func<IdentifiedPoint, Color>> SchemeTable
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
