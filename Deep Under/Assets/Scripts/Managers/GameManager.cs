using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class GameManager : UnitySingleton<GameManager> {

    public bool DisallowPauseMenu;
    private Coroutine WaitingToReload;
    private Coroutine WaitingOnInput;
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

    public void WaitForInput(Action callbackOnInput)
    {
        WaitingOnInput = StartCoroutine(ActionOnInput(callbackOnInput));
    }

    public static void LoadLevel(string sceneName)
    {
        Instance.PauseTime();
        AsyncOperation loadOp = null;
        Action load = () => {
            FishManager.Instance.Reset();
            OrbManager.Instance.Reset();
            loadOp = SceneManager.LoadSceneAsync(sceneName);
            GUIManager.Instance.LoadScreen(loadOp, 1);
            GUIManager.Instance.EnergyBar.alpha = 1;
            GUIManager.Instance.GhostBar.alpha = 1;
            Instance.ResumeTime();
        };
        GUIManager.Instance.FadeToBlack(load);
        // GUIManager.Instance.LoadScreen(loadOp, 1);
        Instance.WaitingToReload = null;
    }

    public static void LoadLevel(Scene scene)
    {
        LoadLevel(scene.name);
    }

    private IEnumerator ReloadOnInput()
    {
        // Wait for input
        while (!Input.anyKeyDown)
            { yield return null; }

        CanvasGroup[] keep = new CanvasGroup[] { GUIManager.Instance.FadeOverlay };
        GUIManager.Instance.FadeToClearExclusive(keep, () => LoadLevel(SceneManager.GetActiveScene()) );
    }

    private IEnumerator ActionOnInput(Action callbackOnInput)
    {
        // Wait for input
        while (!Input.anyKeyDown)
            { yield return null; }

        if (callbackOnInput != null)
            { callbackOnInput(); }
    }

    void Update()
    {
		if ((Input.GetKeyUp(KeyCode.JoystickButton7) || Input.GetKeyDown("escape")) && Instance.WaitingToReload == null && !this.DisallowPauseMenu)
        //if (Input.GetButtonDown("Cancel"))
        {
            if (GUIManager.Instance.GamePaused)
                { GUIManager.Instance.ResumeGame(); }
            else
                { GUIManager.Instance.PauseGame(); }
        }
    }

    void Start()
    {
        // GUIManager.Instance.FadeToClear(null);
        GUIManager.Instance.ShowTutorial(SceneManager.GetActiveScene().name);
    }
}
