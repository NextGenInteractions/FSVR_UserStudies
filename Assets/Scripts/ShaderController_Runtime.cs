using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Runtime Shader Controller
/// contains feature to flush with texture swapping and fade in/out with color lerp
/// </summary>
[RequireComponent(typeof (MeshRenderer))]
public class ShaderController_Runtime : MonoBehaviour
{
    public string seletedMatName;
    public Material mat;
    public TextureHolder texHolder = new TextureHolder();
    public ColorHolder colorHolder = new ColorHolder();

    /// <summary>
    /// initialize texture holder and color
    /// assign the material instance upon start
    /// </summary>
    /// <param name="material"></param>
    void Start()
    {
        Material[] materials = GetComponent<MeshRenderer>().materials;
        for (int i = 0; i < materials.Length; ++i)
        {
            if (materials[i].name.Contains(seletedMatName))
            {
                seletedMatName = materials[i].name;
                mat = materials[i];
                texHolder.setContainer(this);
                colorHolder.setContainer(this);
                if (texHolder.startOnLoad)
                    StartRoutine("swap");
                if (colorHolder.startOnLoad)
                    StartRoutine("fade");
                break;
            }
        }
    }

    /// <summary>
    /// set texture in material
    /// </summary>
    /// <param name="field"></param>
    /// <param name="textureMap"></param>
    public void SetTexture(string field, Texture2D textureMap)
    {
        mat.SetTexture(field, textureMap);
    }

    // Update is called once per frame
    void Update()
    {
        colorHolder.Update();
    }

    /// <summary>
    /// start routine in texture holder and color holder with certain commands
    /// leave potential feature with cmd input
    /// </summary>
    /// <param name="cmd"></param>
    public void StartRoutine (string cmd)
    {
        switch(cmd)
        {
            case "swap":
                if (texHolder.swapRoutine != null)
                    StopCoroutine(texHolder.swapRoutine);
                texHolder.swapRoutine = StartCoroutine(texHolder.StartSwapRoutine());
                break;
            case "fade":
                if (colorHolder.fadeRoutine != null)
                    StopCoroutine(colorHolder.fadeRoutine);
                colorHolder.fadeRoutine = StartCoroutine(colorHolder.StartFadeRoutine());
                break;
        }
    }

    /// <summary>
    /// stop routine in texture holder and color holder
    /// </summary>
    /// <param name="cmd"></param>
    public void StopRoutine (string cmd)
    {
        switch (cmd)
        {
            case "swap":
                if (texHolder.swapRoutine != null)
                    StopCoroutine(texHolder.swapRoutine);
                break;
            case "fade":
                if (colorHolder.fadeRoutine != null)
                    StopCoroutine(colorHolder.fadeRoutine);
                break;
        }
    }
}

[System.Serializable]
public class TextureHolder
{
    public List<Texture2D> textureList = new List<Texture2D>();
    public bool startOnLoad = true;
    [Tooltip("If set to 0, it runs infinitely")]
    public int maxRun = 0;
    [Tooltip("Time that it waits to start in seconds")]
    public float delayOnStart = 0.1f;
    public float intervalTime = 0.1f;
    public ShaderPropertyMap.TextureMap textureMap = ShaderPropertyMap.TextureMap.Albedo;
    public Coroutine swapRoutine;

    private ShaderController_Runtime controller;
    private int currentRun = 1;
    private int currentIndex = 0;
    private bool isStart = false;

    /// <summary>
    /// assign shader controller holds all information about shader control
    /// </summary>
    /// <param name="container"></param>
    public void setContainer (ShaderController_Runtime container)
    {
        controller = container;
    }

    /// <summary>
    /// start routine with swapping texture feature
    /// </summary>
    /// <returns></returns>
    public IEnumerator StartSwapRoutine()
    {
        isStart = true;
        yield return new WaitForSeconds(delayOnStart);
        if (textureList.Count > 0)
        {
            bool isEnd = false;
            while (!isEnd)
            {
                yield return new WaitForSeconds(intervalTime);
                controller.SetTexture(ShaderPropertyMap.GetTextureString(textureMap), textureList[currentIndex++]);
                if (textureList.Count > 0 && currentIndex >= textureList.Count)
                {
                    if (maxRun == 0 || ++currentRun <= maxRun)
                    {
                        currentIndex = 0;
                    }
                    else
                    {
                        isEnd = true;
                    }
                }
            }
        }
    }
}

[System.Serializable]
public class ColorHolder
{
    public List<Color> colorList = new List<Color>();
    public bool startOnLoad = true;
    public float totalTime = 0;
    public float onerunTime = 1f;
    public ShaderPropertyMap.ColorMap colorMap = ShaderPropertyMap.ColorMap.Albedo;
    public Coroutine fadeRoutine;

    private ShaderController_Runtime controller;
    private int currentIndex = 0;
    private Color start, end;
    private bool isStart = false;

    /// <summary>
    /// assign shader controller holds all information about shader control
    /// </summary>
    /// <param name="container"></param>
    public void setContainer(ShaderController_Runtime container)
    {
        controller = container;
    }

    /// <summary>
    /// start routine with fade in/out feature
    /// </summary>
    /// <returns></returns>
    public IEnumerator StartFadeRoutine()
    {
        isStart = true;
        int count = colorList.Count;
        float currentTime = Time.time;
        int sign = 1;
        if (count > 1)
        {
            bool isEnd = false;
            while (!isEnd)
            {
                float eachLerpTime = onerunTime / ((count - 1) * 2);
                yield return new WaitForSeconds(eachLerpTime);
                start = colorList[currentIndex];
                end = colorList[currentIndex + sign];
                currentIndex += sign;
                if (currentIndex >= count - 1 || currentIndex == 0)
                {
                    if (totalTime == 0 || Time.time - currentTime < totalTime)
                    {
                        sign *= -1;
                    }
                    else
                    {
                        isEnd = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Update each frame
    /// called by shader controller
    /// </summary>
    public void Update()
    {
        if (isStart && colorList.Count > 1)
            controller.mat.SetColor(ShaderPropertyMap.GetColorString(colorMap), Color.Lerp(start, end, Mathf.PingPong(Time.time, onerunTime / ((colorList.Count - 1) * 2))));
    }

}

public class ShaderPropertyMap
{
    public enum TextureMap
    {
        Albedo = 0,
        Specular,
        Normal,
        Height,
        Occlusion,
        Emission
    }

    public enum ColorMap
    {
        Albedo,
        Specular,
        Emission
    }

    private static string[] textureStringList = new string[] { "_BaseMap", "_SpecGlossMap", "_BumpMap", "_ParallaxMap", "_OcclusionMap", "_EmissionMap" };
    private static string[] colorStringList = new string[] { "_BaseColor", "_SpecColor", "_EmissionColor" };

    public static string GetTextureString (TextureMap tex)
    {
        int index = (int)tex;
        return (index < textureStringList.Length) ? textureStringList[index] : "";
    }

    public static string GetColorString(ColorMap color)
    {
        int index = (int)color;
        return (index < colorStringList.Length) ? colorStringList[index] : "";
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(ShaderController_Runtime))]
public class ShaderControllerEditor: Editor
{
    int selectedIndex = 0;
    string[] matList;

    string[] GetMatList (ShaderController_Runtime scr)
    {
        List<string> list = new List<string>();
        foreach (Material mat in scr.GetComponent<MeshRenderer>().sharedMaterials)
            list.Add(mat.name);
        return list.ToArray();
        
    }

    public override void OnInspectorGUI()
    {
        GUILayout.Label("Select the mateiral");
        var instance = (ShaderController_Runtime)this.target;
        matList = GetMatList(instance);
        selectedIndex = EditorGUILayout.Popup(selectedIndex, matList);
        instance.seletedMatName = matList[selectedIndex];

        DrawDefaultInspector();


    }
}
#endif