using System;
using System.Linq;
using UnityEngine;

namespace Share
{
    public static class CSVParser
    {
        private const char Separator = ',';

        private static readonly string Header =
            $"Time{Separator}Identify{Separator}Confidence{Separator}PositionX{Separator}PositionY{Separator}PositionZ{Separator}CameraPositionX{Separator}CameraPositionY{Separator}CameraPositionZ{Separator}CameraRotationX{Separator}CameraRotationY{Separator}CameraRotationZ";

        public static string[] ToCsv(PackedMessage.IdentifiedPointArray array)
        {
            var time = array.Time.ToUnixTimeMilliseconds();
            return new[] {Header}.Concat(array.Array.Select(identifiedPoint => ToCsvLine(time, identifiedPoint))).ToArray();
        }

        private static string ToCsvLine(in long time, in IdentifiedPoint point)
        {
            return
                $"{time}{Separator}{point.Identify}{Separator}{point.Confidence}{Separator}{point.Position.x}{Separator}{point.Position.y}{Separator}{point.Position.z}{Separator}{point.CameraPosition.x}{Separator}{point.CameraPosition.y}{Separator}{point.CameraPosition.z}{Separator}{point.CameraRotation.eulerAngles.x}{Separator}{point.CameraRotation.eulerAngles.y}{Separator}{point.CameraRotation.eulerAngles.z}";
        }

        public static PackedMessage.IdentifiedPointArray FromCsv(string[] csv)
        {
            var result = new PackedMessage.IdentifiedPointArray();

            var sample = csv[1].Split(Separator)[0];
            var time = DateTimeOffset.MinValue;
            if (long.TryParse(sample, out var rawTime))
            {
                time = DateTimeOffset.FromUnixTimeMilliseconds(rawTime);
            }
            result.Time = time;
            // skip header
            result.Array = csv.Skip(1).Select(FromCsvLine).ToArray();
            return result;
        }

        // time, identify, confidence, posX, posY, posZ, cameraPosX, cameraPosY, cameraPosZ, cameraRotX, cameraRotY, cameraRotZ
        private static IdentifiedPoint FromCsvLine(string line)
        {
            var data = line.Split(Separator);
            var result = new IdentifiedPoint();

            if (ulong.TryParse(data[1], out var identify))
            {
                result.Identify = identify;
            }

            if (float.TryParse(data[2], out var confidence))
            {
                result.Confidence = confidence;
            }

            if (float.TryParse(data[3], out var x) && float.TryParse(data[4], out var y) && float.TryParse(data[5], out var z))
            {
                result.Position = new Vector3(x, y, z);
            }

            if (float.TryParse(data[6], out var cpx) && float.TryParse(data[7], out var cpy) && float.TryParse(data[8], out var cpz))
            {
                result.CameraPosition = new Vector3(cpx, cpy, cpz);
            }
            if (float.TryParse(data[9], out var crx) && float.TryParse(data[10], out var cry) && float.TryParse(data[11], out var crz))
            {
                result.CameraRotation = Quaternion.Euler(crx, cry, crz);
            }

            return result;
        }
    }
}
