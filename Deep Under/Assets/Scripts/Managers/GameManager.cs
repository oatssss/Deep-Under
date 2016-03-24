using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class GameManager : UnitySingleton<GameManager> {

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

    public static void LoadLevel(string sceneName)
    {
        AsyncOperation loadOp = null;
        Action load = () => { loadOp = SceneManager.LoadSceneAsync(sceneName); };
        GUIManager.Instance.FadeToBlack(load);
        GUIManager.Instance.LoadScreen(loadOp, 1);
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

    void Update()
    {
		if ((Input.GetKeyUp(KeyCode.JoystickButton7) || Input.GetKeyUp(KeyCode.JoystickButton7)) && Instance.WaitingToReload == null)
        {
            if (GUIManager.Instance.GamePaused)
                { GUIManager.Instance.ResumeGame(); }
            else
                { GUIManager.Instance.PauseGame(); }
        }
    }

    void Start()
    {
        GUIManager.Instance.FadeToClear(null);
    }
}
