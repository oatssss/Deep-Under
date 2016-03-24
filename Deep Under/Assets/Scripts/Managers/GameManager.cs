using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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

        // Reload!
        SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
        WaitingToReload = null;
        GUIManager.Instance.FadeToClear(null);
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
