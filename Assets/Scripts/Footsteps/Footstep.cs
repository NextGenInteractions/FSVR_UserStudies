using System.Collections;
using UnityEngine;

public class Footstep : MonoBehaviour
{
    Renderer _footstepRenderer;
    Renderer footstepRenderer { get { if (_footstepRenderer == null) _footstepRenderer = GetComponentInChildren<Renderer>(); return _footstepRenderer; } }

    public void Show()
    {
        var originalColor = footstepRenderer.material.color;
        var alphaColor = originalColor;
        alphaColor.a = 1;
        footstepRenderer.material.color = alphaColor;
    }

    public void Hide()
    {
        var originalColor = footstepRenderer.material.color;
        var alphaColor = originalColor;
        alphaColor.a = 0;
        footstepRenderer.material.color = alphaColor;
    }

    public void Animate(float fadeInTime, float stayTime, float fadeOutTime)
    {
        StartCoroutine(AnimateCoroutine(fadeInTime, stayTime, fadeOutTime));
    }

    private IEnumerator AnimateCoroutine(float fadeInTime, float stayTime, float fadeOutTime)
    {
        var originalColor = footstepRenderer.material.color;

        float elapsedTime = 0f;
        while (elapsedTime < fadeInTime)
        {
            var alphaColor = originalColor;
            alphaColor.a = elapsedTime / fadeInTime;
            footstepRenderer.material.color = alphaColor;

            yield return null;
            elapsedTime += Time.deltaTime;
        }

        elapsedTime = 0f;
        while (elapsedTime < stayTime)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
        }

        elapsedTime = 0f;
        while (elapsedTime < fadeOutTime)
        {
            var alphaColor = originalColor;
            alphaColor.a = 1 - (elapsedTime / fadeOutTime);
            footstepRenderer.material.color = alphaColor;

            yield return null;
            elapsedTime += Time.deltaTime;
        }

        Hide();
    }
}
