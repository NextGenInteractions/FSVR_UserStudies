using UnityEngine;

public class CameraBilt : MonoBehaviour
{ // Copies aTexture to rTex and displays it in all cameras.

    public Texture aTexture;
    public RenderTexture rTex;

    void Start()
    {
        if (!aTexture || !rTex)
        {
            Debug.LogError("A texture or a render texture are missing, assign them.");
        }
    }

    void Update()
    {
        Graphics.Blit(aTexture, rTex);
    }
}