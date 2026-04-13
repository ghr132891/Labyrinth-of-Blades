using System.Collections;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;

public class UI_FadeScreen : MonoBehaviour
{
    private Image fadeImage;
    public Coroutine fadeEffectCo { get; private set; }

    private void Awake()
    {
        fadeImage = GetComponent<Image>();
        fadeImage.color = new Color(0,0,0,1);
    }

    public void DoFadeIn(float duration = 1)
    {
        fadeImage.color = new Color(0,0,0,1);
        FadeEffect(0f, duration);
    }

    public void DoFadeOut(float duration = 1)
    {
        fadeImage.color = new Color(0, 0, 0, 0);
        FadeEffect(1f, duration);
    }

    private void FadeEffect(float targetAlpha,float duration)
    {
        if (fadeEffectCo != null)
            StopCoroutine(fadeEffectCo);

        fadeEffectCo = StartCoroutine(FadeEffectCo(targetAlpha, duration));
    }

    private IEnumerator FadeEffectCo(float targetAlpha,float duration)
    {
        float startAlpha = fadeImage.color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            var color = fadeImage.color;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, time / duration);

            fadeImage.color = color;

            yield return null;
        }

        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, targetAlpha);
    }
    

    

}
