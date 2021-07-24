using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Share;
using UniRx;
using UnityEngine;

namespace Viewer
{
    public class CsvConfigurator : MonoBehaviour, IPointDataSupplier
    {
        protected internal readonly Subject<PackedMessage.IdentifiedPointArray> Supply
            = new Subject<PackedMessage.IdentifiedPointArray>();

        public IObservable<PackedMessage.IdentifiedPointArray> OnReceivePointData()
        {
            return Supply;
        }

        [SerializeField]
        protected internal PointRoot pointRoot;
    }

#if UNITY_EDITOR

    [UnityEditor.CustomEditor(typeof(CsvConfigurator))]
    public class CsvConfiguratorEditor : UnityEditor.Editor
    {
        private CsvConfigurator configurator = null;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            UnityEditor.EditorGUILayout.Space();

            UnityEditor.EditorGUILayout.HelpBox("Save PointData to csv", UnityEditor.MessageType.Info);

            UnityEditor.EditorGUI.BeginDisabledGroup(!UnityEditor.EditorApplication.isPlaying);
            if (GUILayout.Button(nameof(Save)))
            {
                if (configurator == null)
                {
                    configurator = target as CsvConfigurator;
                }
                Save().Forget();
            }
            UnityEditor.EditorGUI.EndDisabledGroup();

            UnityEditor.EditorGUILayout.Space();

            UnityEditor.EditorGUILayout.HelpBox("Load PointData from csv", UnityEditor.MessageType.Info);

            UnityEditor.EditorGUI.BeginDisabledGroup(!UnityEditor.EditorApplication.isPlaying);
            if (GUILayout.Button(nameof(Load)))
            {
                if (configurator == null)
                {
                    configurator = target as CsvConfigurator;
                }
                Load().Forget();
            }
            UnityEditor.EditorGUI.EndDisabledGroup();
        }

        private const string ExtensionCsv = "csv";

        private async UniTaskVoid Save()
        {
            var dirPath = UnityEditor.EditorUtility.SaveFolderPanel(
                nameof(Save), Path.Combine(Application.dataPath, ExtensionCsv), ExtensionCsv);
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
                    return;
                }
            }

            Debug.Log($"Save {array.Length} to {dirPath}");
        }

        private async UniTaskVoid Load()
        {
            var dirPath = UnityEditor.EditorUtility.OpenFolderPanel(
                nameof(Load), Path.Combine(Application.dataPath, ExtensionCsv), ExtensionCsv);
            if (string.IsNullOrEmpty(dirPath))
            {
                return;
            }
            var dirInfo = new DirectoryInfo(dirPath);
            if (!dirInfo.Exists)
            {
                return;
            }

            await UniTask.SwitchToThreadPool();

            var lineCache = new List<string>();

            foreach (var fileInfo in dirInfo.EnumerateFiles())
            {
                if (!fileInfo.Exists || !string.Equals(fileInfo.Extension, $".{ExtensionCsv}"))
                {
                    continue;
                }

                using var reader = fileInfo.OpenText();
                // ignore header
                await reader.ReadLineAsync();

                lineCache.Clear();
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    lineCache.Add(line);
                }
                var info = CSVParser.FromCsv(lineCache.ToArray());
                configurator.Supply.OnNext(info);
            }

            Debug.Log($"Load from {dirPath}");
        }
    }
#endif
}
