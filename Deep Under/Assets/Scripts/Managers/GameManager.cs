﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class GameManager : UnitySingletonPersistent<GameManager> {

    private Coroutine WaitingToReload;
    private Player player = null;
    public Player Player
    {
        get
        {
            if (this.player == null)
            {
                GameObject playerObject = GameObject.FindWithTag("Player");
                if (playerObject)
                {
                    this.player = playerObject.GetComponent<Player>();
                }
            }

            return this.player;
        }
    }

	public void PauseTime()
    {
        Time.timeScale = 0f;
    }

    public void ResumeTime()
    {
        Time.timeScale = 1f;
    }

    public void WaitForInputToReload()
    {
        WaitingToReload = StartCoroutine(ReloadOnInput());
    }

    private IEnumerator ReloadOnInput()
    {
        // Wait for input
        while (!Input.anyKeyDown)
            { yield return null; }

        Action reload = () => {
            SceneManager.LoadScene (SceneManager.GetActiveScene().name);
            WaitingToReload = null;
        };

        CanvasGroup[] keep = new CanvasGroup[] { GUIManager.Instance.FadeOverlay };
        GUIManager.Instance.FadeToClearExclusive(keep, reload);
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel") && Instance.WaitingToReload == null)
        {
            if (GUIManager.Instance.GamePaused)
                { GUIManager.Instance.ResumeGame(); }
            else
                { GUIManager.Instance.PauseGame(); }
        }
    }
}
