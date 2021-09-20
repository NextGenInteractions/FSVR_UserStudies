using Newtonsoft.Json;
using NextGen.VrManager.Devices;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NextGen.VrManager.ToolManagement
{
    public static class ToolManager
    {
        public static List<Tool> tools = new List<Tool>();

        public static Action<Tool> ToolAdded, ToolRemoved;

        private static string persistenceBasePath { get { return $"{Application.streamingAssetsPath}/NextGen"; } }

        private static readonly string persistenceFileName = "toolPersistence.json";
        private static string persistenceFilePath { get { return $"{persistenceBasePath}/{persistenceFileName}"; } }

        public static Dictionary<string, Dictionary<string, DeviceSlotEntry>> PersistenceMap = new Dictionary<string, Dictionary<string, DeviceSlotEntry>>();

        public class DeviceSlotEntry
        {
            public DeviceSlotEntry(string uid, string displayName)
            {
                DeviceUid = uid;
                DeviceDisplayName = displayName;
            }

            public string DeviceUid;
            public string DeviceDisplayName;
        }

        public static void AddTool(Tool t)
        {
            tools.Add(t);
            ToolAdded.Invoke(t);
            t.OnDevicesChanged += UpdatePersistenceMap;
        }

        public static void RemoveTool(Tool t)
        {
            tools.Remove(t);
            ToolRemoved.Invoke(t);
            t.OnDevicesChanged -= UpdatePersistenceMap;
        }

        [RuntimeInitializeOnLoadMethod]
        public static void Awake()
        {
            SubscribeToEvents();
            ReadAssociationsFromPersistenceFile();
        }

        private static void SubscribeToEvents()
        {
        }

        private static void ReadAssociationsFromPersistenceFile()
        {
            if (File.Exists(persistenceFilePath))
            {
                string json;
                using (StreamReader reader = new StreamReader(persistenceFilePath))
                {
                    json = reader.ReadToEnd();
                }

                Dictionary<string, Dictionary<string, DeviceSlotEntry>> map = null;
                try
                {
                    map = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, DeviceSlotEntry>>>(json);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Invalid file format for file: {persistenceFilePath}. Exception: {e.Message}");
                }

                if (map != null)
                {
                    Debug.Log("PersistenceMap has been set!");
                    PersistenceMap = map;
                }
            }
            else
            {
                Debug.LogError($"Could not read from file: {persistenceFilePath}. File does not exist. Creating empty file now...");

                if (!Directory.Exists(persistenceBasePath))
                    Directory.CreateDirectory(persistenceBasePath);

                using (StreamWriter writer = new StreamWriter(persistenceFilePath, false))
                    writer.Write("{\n}");
            }
        }

        private static void UpdatePersistenceMap(Tool t)
        {
            Dictionary<string, DeviceSlotEntry> toolEntry = new Dictionary<string, DeviceSlotEntry>();

            foreach (KeyValuePair<string, Device> kvp in t.Devices)
            {
                toolEntry[kvp.Key] = new DeviceSlotEntry(kvp.Value.Uid, kvp.Value.DisplayName);
            }

            PersistenceMap[t.PersistenceUid] = toolEntry;

            WritePersistenceMapToFile();
        }

        private static void WritePersistenceMapToFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(persistenceFilePath, false))
                {
                    writer.Write(JsonConvert.SerializeObject(PersistenceMap, Formatting.Indented));
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to write metadata to file: {persistenceFilePath}. Exception: {e}");
            }
        }
    }
}
