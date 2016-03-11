using UnityEngine;

public class GameManager : UnitySingletonPersistent<GameManager> {

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
}
