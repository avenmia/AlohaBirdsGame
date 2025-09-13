using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class fadetest : MonoBehaviour
{
    public Image uiImage;             // Assign in Inspector
    public float fadeDuration = 1f;   // Duration for fade in/out
    public float visibleDuration = 2f; // How long the image stays fully visible

    private void Start()
    {
        if (uiImage != null)
            StartCoroutine(FadeRoutine());
        else
            Debug.LogWarning("UIFader: No Image assigned.");
    }

    IEnumerator FadeRoutine()
    {
        yield return StartCoroutine(Fade(0f, 1f));           // Fade In
        yield return new WaitForSeconds(visibleDuration);    // Wait
        yield return StartCoroutine(Fade(1f, 0f));           // Fade Out
    }

    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float timer = 0f;
        Color color = uiImage.color;

        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, timer / fadeDuration);
            uiImage.color = new Color(color.r, color.g, color.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }

        // Ensure final alpha is exact
        uiImage.color = new Color(color.r, color.g, color.b, endAlpha);
    }
}
