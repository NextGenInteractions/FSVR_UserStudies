using Newtonsoft.Json;
using NextGen.VrManager.ToolManagement;
using NextGen.VrManager.Ui;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [System.Serializable]
    public class SettingsFile
    {
        public int settingsMode;
        public SettingsConfiguration settingsConfiguration;

        public SettingsFile()
        {
            settingsMode = 0;
            settingsConfiguration = new SettingsConfiguration(0);
        }

        public SettingsFile(int settingsMode, SettingsConfiguration settingsConfiguration)
        {
            this.settingsMode = settingsMode;
            this.settingsConfiguration = settingsConfiguration;
        }
    }

    [System.Serializable]
    public class SettingsConfiguration
    {
        public bool overlayVisible;
        public bool visualizeDevices;
        public bool visualizeFloorGrid;
        public bool deviceManagerWindowVisible;
        public bool toolManagerWindowVisible;
        public bool pivotManagerWindowVisible;
        public bool metricManagerWindowVisible;
        public bool nextgenInspectorWindowVisible;
        public string defaultCameraFocusTarget;

        public SettingsConfiguration(int mode)
        {
            switch(mode)
            {
                case 0: //Remember last session mode.
                    break;

                case 1: //Developer mode.
                    overlayVisible = true;
                    visualizeDevices = true;
                    visualizeFloorGrid = true;
                    deviceManagerWindowVisible = true;
                    nextgenInspectorWindowVisible = true;
                    break;

                case 2: //Demo mode.
                    break;

                case 3: //User study mode.
                    overlayVisible = true;
                    metricManagerWindowVisible = true;
                    break;
            }
        }
    }

    private static readonly string fileName = "ngvrmSettings.json";
    private static string basePath { get { return $"{Application.streamingAssetsPath}/NextGen"; } }
    private static string filePath { get { return $"{basePath}/{fileName}"; } }

    public SettingsFile loadedFile;
    public SettingsConfiguration settings;

    [Header("Component References")]
    [SerializeField] private Canvas overlay;
    [SerializeField] private Togglebox visualizeDevicesTogglebox;
    [SerializeField] private GameObject floorGrid;
    [SerializeField] private CanvasGroup deviceManagerWindow;
    [SerializeField] private CanvasGroup toolManagerWindow;
    [SerializeField] private CanvasGroup pivotManagerWindow;
    [SerializeField] private CanvasGroup metricManagerWindow;
    [SerializeField] private CanvasGroup nextgenInspectorWindow;

    private void OnEnable()
    {
        Application.wantsToQuit += SessionEndWrite;
    }

    private void OnDisable()
    {
        Application.wantsToQuit -= SessionEndWrite;
    }

    void Awake()
    {
        ReadSettingsFromFile();
    }

    private void Start()
    {
        ApplySettings();
    }

    // Update is called once per frame
    void Update()
    {
        bool lShift = UnityEngine.InputSystem.Keyboard.current.leftShiftKey.isPressed;
        bool f1 = UnityEngine.InputSystem.Keyboard.current.f1Key.wasPressedThisFrame;
        bool f2 = UnityEngine.InputSystem.Keyboard.current.f2Key.wasPressedThisFrame;

        if (lShift)
        {
            if(f1)
                overlay.enabled = !overlay.enabled;
            if (f2)
                floorGrid.SetActive(!floorGrid.activeInHierarchy);
        }
    }

    private void ApplySettings()
    {
        overlay.enabled = settings.overlayVisible;

        visualizeDevicesTogglebox.state = settings.visualizeDevices;
        floorGrid.SetActive(settings.visualizeFloorGrid);

        ApplySettingForWindow(deviceManagerWindow, settings.deviceManagerWindowVisible);
        ApplySettingForWindow(toolManagerWindow, settings.toolManagerWindowVisible);
        ApplySettingForWindow(pivotManagerWindow, settings.pivotManagerWindowVisible);
        ApplySettingForWindow(metricManagerWindow, settings.metricManagerWindowVisible);
        ApplySettingForWindow(nextgenInspectorWindow, settings.nextgenInspectorWindowVisible);

        ApplyDefaultCameraFocusTarget();
    }

    private void ApplySettingForWindow(CanvasGroup window, bool setting)
    {
        window.alpha = setting ? 1 : 0;
        window.interactable = setting;
        window.blocksRaycasts = setting;
    }

    private void ApplyDefaultCameraFocusTarget()
    {
        if(!string.IsNullOrEmpty(settings.defaultCameraFocusTarget))
        {
            List<Tool> tools = new List<Tool>(FindObjectsOfType<Tool>());
            Tool foundTool = tools.FirstOrDefault(tool => tool.name == settings.defaultCameraFocusTarget);
            if(foundTool != null)
                SmartCamera.SetFocus(foundTool.transform);
        }
    }

    private void ReadSettingsFromFile()
    {
        Debug.Log("Reading file...");
        if (File.Exists(filePath))
        {
            string json;
            using (StreamReader reader = new StreamReader(filePath))
            {
                json = reader.ReadToEnd();
            }

            loadedFile = null;
            try
            {
                loadedFile = JsonConvert.DeserializeObject<SettingsFile>(json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Invalid file format for file: {filePath}. Exception: {e.Message}");
            }

            if (loadedFile != null)
            {
                Debug.Log($"Settings file loaded. Settings mode determined to be {loadedFile.settingsMode}.");



                settings = loadedFile.settingsMode == 0 ? loadedFile.settingsConfiguration : new SettingsConfiguration(loadedFile.settingsMode);
            }
        }
        else
        {
            Debug.LogError($"Could not read from file: {filePath}. File does not exist. Creating empty file now...");

            using (StreamWriter writer = new StreamWriter(filePath, false))
                writer.Write("{\n}");
        }
    }

    private void WriteSettingsToFile()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                SettingsFile toWrite = new SettingsFile(loadedFile.settingsMode, settings);
                writer.Write(JsonConvert.SerializeObject(toWrite, Formatting.Indented));
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write settings to file: {filePath}. Exception: {e}");
        }
    }

    private void CreateDefaultSettingsFile()
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(filePath, false))
            {
                SettingsFile toWrite = new SettingsFile();
                writer.Write(JsonConvert.SerializeObject(toWrite, Formatting.Indented));
            }
            Debug.Log("Successfully created default file");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write settings to file: {filePath}. Exception: {e}");
        }
    }

    private bool SessionEndWrite()
    {
        if (loadedFile.settingsMode == 0)
        {
            settings.overlayVisible = overlay.enabled;
            settings.visualizeDevices = visualizeDevicesTogglebox.state;
            settings.visualizeFloorGrid = floorGrid.activeInHierarchy;
            settings.deviceManagerWindowVisible = deviceManagerWindow.alpha == 1;
            settings.toolManagerWindowVisible = toolManagerWindow.alpha == 1;
            settings.pivotManagerWindowVisible = pivotManagerWindow.alpha == 1;
            settings.metricManagerWindowVisible = metricManagerWindow.alpha == 1;
            settings.nextgenInspectorWindowVisible = nextgenInspectorWindow.alpha == 1;
            settings.defaultCameraFocusTarget = SmartCamera.Instance.focusTarget != null ? SmartCamera.Instance.focusTarget.name : null;
        }

        WriteSettingsToFile();

        return true;
    }


}
