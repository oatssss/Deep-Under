﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

public class GUIManager : UnitySingletonPersistent<GUIManager> {

    public class Ref<T> where T : struct
    {
        public T Value {get; set;}
    }

    private enum TRANSITION { STACK, NOSTACK, CLOSE }
    public bool GamePaused = false;
    [SerializeField] private Menu PauseMenu;
    private Menu CurrentMenu;
    private Stack<Menu> History = new Stack<Menu>();

    [Header("Fading")]
    private List<KeyValuePair<CanvasGroup,Coroutine>> FadingRenderers = new List<KeyValuePair<CanvasGroup,Coroutine>>();
    [SerializeField] private List<CanvasGroup> CurrentOverlays = new List<CanvasGroup>();
    public CanvasGroup FadeOverlay;
	[SerializeField] private CanvasGroup EatenOverlay;
	[SerializeField] private CanvasGroup BatteryOverlay;

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
        GUIManager.Instance.ShowTooltip("Collect the fish souls to complete the level. Recharge energy at the pods.");
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
        FadeToRenderer(Instance.FadeOverlay, callback);
    }

    public void FadeToEaten(Action callback)
    {
        FadeToRenderer(Instance.EatenOverlay, callback);
        this.FadeToBlack(null);
    }

    // public void FadeFromEaten(Action callback)
    // {
    //     FadeRendererToClear(Instance.EatenOverlay, callback);
    // }

    public void FadeToBattery(Action callback)
    {
        FadeToRenderer(Instance.BatteryOverlay, callback);
        this.FadeToBlack(null);
    }

    // public void FadeFromBattery(Action callback)
    // {
    //     FadeRendererToClear(Instance.BatteryOverlay, callback);
    // }

    /// <summary>Cause the screen to fade to clear, the fade covers everything including UI elements.</summary>
    /// <param name="callback">A callback method to run after the fade.</param>
    public void FadeToClear(Action callback)
    {
        foreach (CanvasGroup overlay in Instance.CurrentOverlays)
            { FadeRendererToClear(overlay, callback); }
    }

    public void FadeToClearExclusive(CanvasGroup[] excludedGroups, Action callback)
    {
        IEnumerable<CanvasGroup> removing =
            from active in Instance.CurrentOverlays
            from keep in excludedGroups
            where active != keep
            select active;

        IEnumerator<CanvasGroup> enumerator = removing.GetEnumerator();
        CanvasGroup last = removing.Last();
        while (enumerator.MoveNext() && enumerator.Current != last)
            { FadeRendererToClear(enumerator.Current, null); }

        // The last one will do the callback
        FadeRendererToClear(enumerator.Current, callback);
    }

    public void FadeToRenderer(CanvasGroup canvasGroup, Action callback)
    {
        // Find out if the renderer we want to fade is already fading
        KeyValuePair<CanvasGroup,Coroutine> existingFade = Instance.FadingRenderers.Find(renderCoroutinePair => renderCoroutinePair.Key == canvasGroup);

        // If it's already fading, stop the fade
        if (existingFade.Equals(default(KeyValuePair<CanvasRenderer,Coroutine>)) && existingFade.Value != null)
            { Instance.StopCoroutine(existingFade.Value); }

        // Setup a new fade routine for the renderer
        Coroutine fade = null;
        KeyValuePair<CanvasGroup,Coroutine> replacementFade = new KeyValuePair<CanvasGroup,Coroutine>(canvasGroup, fade);

        Action completionAction = () => {
            Instance.FadingRenderers.Remove(replacementFade);
            if (callback != null)
                { callback(); }
        };

        fade = Instance.StartCoroutine(FadeUtility.UIAlphaFade(canvasGroup, canvasGroup.alpha, 1f, FadeDuration, FadeUtility.EaseType.InOut, completionAction));

        Instance.FadingRenderers.Remove(existingFade);  // Remove the previous fade
        Instance.FadingRenderers.Add(replacementFade);  // Add the new fade
        Instance.CurrentOverlays.Add(canvasGroup);      // Store the renderer in a list of renderers that are the current overlays
    }

    public void FadeRendererToClear(CanvasGroup canvasGroup, Action callback)
    {
        // Find out if the renderer we want to fade is already fading
        KeyValuePair<CanvasGroup,Coroutine> existingFade = Instance.FadingRenderers.Find(renderCoroutinePair => renderCoroutinePair.Key == canvasGroup);

        // If it's already fading, stop the fade
        if (existingFade.Equals(default(KeyValuePair<CanvasRenderer,Coroutine>)) && existingFade.Value != null)
            { Instance.StopCoroutine(existingFade.Value); }

        // Setup a new fade routine for the renderer
        Coroutine fade = null;
        KeyValuePair<CanvasGroup,Coroutine> replacementFade = new KeyValuePair<CanvasGroup,Coroutine>(canvasGroup, fade);

        Action completionAction = () => {
            Instance.FadingRenderers.Remove(replacementFade);
            Instance.CurrentOverlays.Remove(canvasGroup);
            if (callback != null)
                { callback(); }
        };

        fade = Instance.StartCoroutine(FadeUtility.UIAlphaFade(canvasGroup, canvasGroup.alpha, 0f, FadeDuration, FadeUtility.EaseType.InOut, completionAction));

        Instance.FadingRenderers.Remove(existingFade);  // Remove the previous fade
        Instance.FadingRenderers.Add(replacementFade);  // Add the new fade
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
}
