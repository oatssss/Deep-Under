using UnityEngine;
using System;
using System.Collections.Generic;

public class GUIManager : UnitySingleton<GUIManager> {

    private enum TRANSITION { STACK, NOSTACK, CLOSE }
    private bool GamePaused = false;
    [SerializeField] private Menu PauseMenu;
    private Menu CurrentMenu;
    private Stack<Menu> History = new Stack<Menu>();

    [Header("Fading")]
    private Coroutine Fading = null;
    [SerializeField] private CanvasRenderer MajorFadeRenderer;
    public static readonly float FadeDuration = 1f;
    [Space(10)]


    [Header("Tooltips")]
    public List<Tooltip> Tooltips = new List<Tooltip>();
    public Canvas Canvas;
    [SerializeField] private float TooltipDuration = 5f;
    public Tooltip TooltipPrefab;
    public enum TOOL_TIP_DURATION { DEFAULT, INSTANTANEOUS }

    void Start()
    {
        GUIManager.Instance.FadeToClear(null);
        GUIManager.Instance.ShowTooltip("Test Tooltip");
    }

    private void OpenMenu(Menu menu, TRANSITION transition)
    {
        menu.ResetTriggers();

        if (Instance.CurrentMenu != null)
        {
            if (transition == TRANSITION.CLOSE)
                { Instance.CurrentMenu.Close(); }
            else
            {
                if (transition == TRANSITION.STACK)
                    { Instance.History.Push(Instance.CurrentMenu); }

                Instance.CurrentMenu.Hide();
            }
        }

        Instance.CurrentMenu = menu;
        Instance.CurrentMenu.Open();
    }

    public void OpenMenu(Menu menu, bool useHistory)
    {
        if (useHistory)
            { Instance.OpenMenu(menu, TRANSITION.STACK); }
        else
            { Instance.OpenMenu(menu, TRANSITION.NOSTACK); }
    }

	public void OpenMenu(Menu menu)
    {
        Instance.OpenMenu(menu, true);
    }

    public void BackFromCurrentMenu()
    {
        Menu previous = (Instance.History.Count > 0) ? Instance.History.Pop() : null;

        if (previous != null)
        {
            Instance.OpenMenu(previous, TRANSITION.CLOSE);
        }
        else
        {
            Instance.ResumeGame();
        }
    }

    public void PauseGame()
    {
        Instance.GamePaused = true;
        GameManager.Instance.PauseTime();
        Instance.SetMenuFocus();
        Instance.OpenMenu(GUIManager.Instance.PauseMenu);
    }

    public void ResumeGame()
    {
        Instance.SetGameFocus();
        Instance.CurrentMenu.Close();
        Instance.CurrentMenu = null;
        foreach (Menu menu in Instance.History)
            { menu.Reset(); }
        Instance.History.Clear();
        // Possibly wait for the close animation to finish
        GameManager.Instance.ResumeTime();
        Instance.GamePaused = false;
    }

    public Menu PreviousMenu()
    {
        return Instance.History.Peek();
    }

    private void SetMenuFocus()
    {
        // TODO
    }

    private void SetGameFocus()
    {
        // TODO
    }

    /// <summary>Cause the screen to fade to black, the fade covers everything including UI elements.</summary>
    /// <param name="callback">A callback method to run after the fade.</param>
    public void FadeToBlack(Action callback)
    {
        if (Instance.Fading != null)
            { Instance.StopCoroutine(Instance.Fading); }

        Instance.Fading = Instance.StartCoroutine(FadeUtility.UIAlphaFade(Instance.MajorFadeRenderer, Instance.MajorFadeRenderer.GetAlpha(), 1f, FadeDuration, FadeUtility.EaseType.InOut, () => {
                Instance.Fading = null;
                if (callback != null)
                    { callback(); }
            }));
    }

    /// <summary>Cause the screen to fade to clear, the fade covers everything including UI elements.</summary>
    /// <param name="callback">A callback method to run after the fade.</param>
    public void FadeToClear(Action callback)
    {
        if (Instance.Fading != null)
            { Instance.StopCoroutine(Instance.Fading); }

        Instance.Fading = Instance.StartCoroutine(FadeUtility.UIAlphaFade(Instance.MajorFadeRenderer, Instance.MajorFadeRenderer.GetAlpha(), 0f, FadeDuration, FadeUtility.EaseType.InOut, () => {
                Instance.Fading = null;
                if (callback != null)
                    { callback(); }
            }));
    }

    public void ShowTooltip(string tooltip, float duration)
    {
        Tooltip duplicate = this.Tooltips.Find(tip => tip.Text.text.Equals(tooltip));

        if (duplicate)
            { duplicate.Show(tooltip, duration); }
        else
        {
            Tooltip newTip = Instantiate<Tooltip>(this.TooltipPrefab);
            newTip.transform.SetParent(this.Canvas.transform, false);
            newTip.Show(tooltip, duration);
        }
    }

    public void ShowTooltip(string tooltip)
    {
        this.ShowTooltip(tooltip, this.TooltipDuration);
    }

    public void ShowTooltip(string tooltip, TOOL_TIP_DURATION duration)
    {
        switch (duration)
        {
            case TOOL_TIP_DURATION.DEFAULT:         this.ShowTooltip(tooltip, this.TooltipDuration); break;
            case TOOL_TIP_DURATION.INSTANTANEOUS:   this.ShowTooltip(tooltip, 0.25f); break;
        }
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (Instance.GamePaused)
                { Instance.ResumeGame(); }
            else
                { Instance.PauseGame(); }
        }
    }
}
