using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour {

    private static float FadeDuration = 0.25f;
    public Text Text;
    [SerializeField] private CanvasRenderer Renderer;
    [SerializeField] private float Duration;
    private Coroutine Fading;
    private bool FadingIn = false;

    void Awake()
    {
        this.Renderer.SetAlpha(0);
    }

    public void Show(string tooltip, float duration)
    {
        if (!GUIManager.Instance.Tooltips.Contains(this))
            { GUIManager.Instance.Tooltips.Add(this); }

        this.Duration = duration;
        this.Text.text = tooltip;

        if (!this.FadingIn && this.Renderer.GetAlpha() != 1)
        {
            if (this.Fading != null)
                { StopCoroutine(this.Fading); }

            this.Fading = StartCoroutine(FadeUtility.UIAlphaFade(this.Renderer, this.Renderer.GetAlpha(), 1, FadeDuration, FadeUtility.EaseType.InOut, () => { this.Fading = null; this.FadingIn = false; }));
            this.FadingIn = true;
        }
    }

    private void Remove()
    {
        GUIManager.Instance.Tooltips.Remove(this);
        Destroy(gameObject);
    }

    void Update()
    {
        this.Duration -= Time.deltaTime;

        if (this.Duration <= 0 && this.Fading == null)
        {
            this.Fading = StartCoroutine(FadeUtility.UIAlphaFade(this.Renderer, this.Renderer.GetAlpha(), 0, FadeDuration, FadeUtility.EaseType.InOut, () => { this.Fading = null; this.Remove(); }));
        }
    }
}
