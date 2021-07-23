using System;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Share;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace Viewer
{
    public class CsvConfigurator : MonoBehaviour, IPointDataSupplier
    {
        private readonly Subject<PackedMessage.IdentifiedPointArray> _supply
            = new Subject<PackedMessage.IdentifiedPointArray>();

        public IObservable<PackedMessage.IdentifiedPointArray> OnReceivePointData()
        {
            return _supply;
        }

        [SerializeField]
        protected internal PointRoot pointRoot;
    }

    [CustomEditor(typeof(CsvConfigurator))]
    public class CsvConfiguratorEditor : UnityEditor.Editor
    {
        private CsvConfigurator configurator = null;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUI.BeginDisabledGroup(!EditorApplication.isPlaying);

            if (GUILayout.Button(nameof(Save)))
            {
                if (configurator == null)
                {
                    configurator = target as CsvConfigurator;
                }
                Save().Forget();
            }
            if (GUILayout.Button(nameof(Load)))
            {
                if (configurator == null)
                {
                    configurator = target as CsvConfigurator;
                }
                Load();
            }

            EditorGUI.EndDisabledGroup();
        }

        private const string ExtensionCsv = "csv";

        private async UniTaskVoid Save()
        {
            var dirPath = EditorUtility.SaveFolderPanel(nameof(Save), Path.Combine(Application.dataPath, ExtensionCsv), ExtensionCsv);
            if (string.IsNullOrEmpty(dirPath))
            {
                return;
            }
            var dirInfo = new DirectoryInfo(dirPath);
            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            await UniTask.SwitchToThreadPool();

            var array = configurator.pointRoot.PointSets
                .Select(set => set.IdentifiedPointArray)
                .ToArray();

            foreach (var pointSet in array)
            {
                try
                {
                    var path = Path.Combine(dirPath, $"{pointSet.Time.ToString(PointSet.DateFormat)}.{ExtensionCsv}");
                    using var writer = new StreamWriter(path, false);
                    await writer.WriteAsync(CSVParser.ToCsv(pointSet).Aggregate((a, b) => $"{a}{Environment.NewLine}{b}"));
                    await writer.WriteLineAsync();
                    await writer.FlushAsync();
                    writer.Close();
                }
                catch (Exception e)
                {
                    Debug.LogError($"{pointSet.Time.ToString(PointSet.DateFormat)}");
                    Debug.LogException(e);
                }
            }
        }

        private async UniTaskVoid Load()
        {
        }
    }
}
