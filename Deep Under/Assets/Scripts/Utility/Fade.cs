using UnityEngine;
using System;
using System.Collections;

public class FadeUtility {

	public enum EaseType {None, In, Out, InOut};

	public static IEnumerator ColorFade(Renderer renderer, Color start, Color end, float duration, EaseType easeType) {

		float t = 0f;
		while (t < 1f) {
			t += Time.deltaTime * (1f / duration);
			renderer.material.color = Color.Lerp (start, end, Ease (t, easeType));
			yield return null;
		}
	}

	public static IEnumerator AlphaFade (Renderer renderer, float start, float end, float duration, EaseType easeType) {

		float t = 0f;
		while (t < 1f) {
			t += Time.deltaTime * (1f / duration);
			float newAlpha = Mathf.Lerp (start, end, Ease(t, easeType));
			renderer.material.color = new Color (renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, newAlpha);
			yield return null;
		}
	}

    public static IEnumerator UIColorFade(CanvasRenderer renderer, Color start, Color end, float duration, EaseType easeType, Action callback) {

		float t = 0f;
		while (t < 1f) {
			t += Time.unscaledDeltaTime * (1f / duration);
			renderer.SetColor (Color.Lerp (start, end, Ease (t, easeType)));
			yield return null;
		}

        if (callback != null)
        {
            callback();
        }
	}

	public static IEnumerator UIColorFade(CanvasRenderer renderer, Color start, Color end, float duration, EaseType easeType) {

		yield return UIColorFade(renderer, start, end, duration, easeType, null);
	}

	public static IEnumerator UIAlphaFade (CanvasGroup group, float start, float end, float duration, EaseType easeType, Action callback) {

		float t = 0f;
		while (t < 1f) {
			t += Time.unscaledDeltaTime * (1f / duration);
			float newAlpha = Mathf.Lerp (start, end, Ease(t, easeType));
			// renderer.SetColor ( new Color (renderer.GetColor().r, renderer.GetColor().g, renderer.GetColor().b, newAlpha));
            group.alpha = newAlpha;
			yield return null;
		}

        if (callback != null)
        {
            callback();
        }
	}

    public static IEnumerator UIAlphaFade (CanvasGroup group, float start, float end, float duration, EaseType easeType) {

		yield return UIAlphaFade(group, start, end, duration, easeType, null);
	}

	public static float Ease (float t, EaseType easeType) {

		switch (easeType) {
			case EaseType.None:
				return t;
			case EaseType.In:
				return Mathf.Lerp(0f, 1f, 1f - Mathf.Cos(t * Mathf.PI * 0.5f));
			case EaseType.Out:
				return Mathf.Lerp(0f, 1f, Mathf.Sin(t * Mathf.PI * 0.5f));
			default:
				return Mathf.SmoothStep(0f, 1f, t);
		}
	}
}
