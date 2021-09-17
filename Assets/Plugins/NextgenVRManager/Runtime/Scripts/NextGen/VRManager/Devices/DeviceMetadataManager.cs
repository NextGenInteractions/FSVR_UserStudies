using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using NextGen.VrManager.Ui;
using System;

namespace NextGen.VrManager.Devices
{
    /// <summary>
    /// A manager for all user-defined data about a device,. using the devices UID (unique identifier).
    /// Reads to/from a deviceMetadata file to store this data across sessions.
    /// </summary>
    public static class DeviceMetadataManager
    {
        private static string basePath { get { return $"{Application.streamingAssetsPath}/NextGen"; } }
        private static readonly string fileName = "deviceMetadata.json";
        private static string filePath { get { return $"{basePath}/{fileName}"; } }

        private static IDictionary<string, DeviceMetadata> DeviceMetadataMap = new Dictionary<string, DeviceMetadata>();

        public static Action<string, DeviceMetadata> DeviceMetadataChanged;

        public static DeviceMetadata? GetMetadata(Device device)
        {
            return GetMetadata(device.Uid);
        }

        public static DeviceMetadata? GetMetadata(string serialNumber)
        {
            if (DeviceMetadataMap.ContainsKey(serialNumber))
                return DeviceMetadataMap[serialNumber];
            else
                return null;
        }

        public static void SetMetadata(Device device, DeviceMetadata metadata)
        {
            SetMetadata(device.Uid, metadata);
        }

        public static void SetMetadata(string serialNumber, DeviceMetadata metadata)
        {
            if (!DeviceMetadataMap.ContainsKey(serialNumber) || !DeviceMetadataMap[serialNumber].Equals(metadata))
            {
                DeviceMetadataMap[serialNumber] = metadata;

                DeviceMetadataChanged?.Invoke(serialNumber, metadata);

                WriteMetadataToFile();
            }
        }

        [RuntimeInitializeOnLoadMethod]
        private static void ReadMetadataFromFile()
        {
            if (File.Exists(filePath))
            {
                string json;
                using (StreamReader reader = new StreamReader(filePath))
                {
                    json = reader.ReadToEnd();
                }

                IDictionary<string, DeviceMetadata> map = null;
                try
                {
                    map = JsonConvert.DeserializeObject<IDictionary<string, DeviceMetadata>>(json);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Invalid file format for file: {filePath}. Exception: {e.Message}");
                }

                if (map != null)
                {
                    DeviceMetadataMap = map;
                }
            }
            else
            {
                Debug.LogError($"Could not read from file: {filePath}. File does not exist. Creating empty file now...");

                if (!Directory.Exists(basePath))
                    Directory.CreateDirectory(basePath);

                using (StreamWriter writer = new StreamWriter(filePath, false))
                    writer.Write("{\n}");
            }
        }

        private static void WriteMetadataToFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(filePath, false))
                    writer.Write(JsonConvert.SerializeObject(DeviceMetadataMap, Formatting.Indented));
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to write metadata to file: {filePath}. Exception: {e}");
            }
        }

        [RuntimeInitializeOnLoadMethod]
        private static void SubscribeToEvents()
        {
            DeviceManager.DeviceAdded += CheckForMetadata;
        }

        private static void CheckForMetadata(Device d)
        {
            if (d.Metadata == null)
            {
                DialogManager.SetMetadataDialog(d);
            }
        }
    }
}
