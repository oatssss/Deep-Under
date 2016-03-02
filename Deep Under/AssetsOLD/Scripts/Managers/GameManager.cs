using UnityEngine;

public class GameManager : UnitySingleton<GameManager> {

    private Player player = null;
    public Player Player
    {
        get
        {
            if (this.player == null)
                { this.player = GameObject.FindWithTag("Player").GetComponent<Player>(); }

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
}
