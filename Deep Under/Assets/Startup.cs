using UnityEngine;
using System.Collections;

public class Startup : MonoBehaviour {

	[SerializeField] private Menu StartupMenu;
    [SerializeField] private Menu LevelSelection;

    private string Level1 = "Tutorial1";
    private string Level2 = "Tutorial2";
    private string Level3 = "Tutorial3";
    private string Level4 = "Tutorial4";
    private string Level5 = "Tutorial5";
    private string Level6 = "LevelTwo 1";
    private string Level7 = "LevelOne 1";
    private string Level8 = "LevelOne 2";

    /*
    void Awake()
    {
        GUIManager.Instance.EnergyBar.alpha = 0;
        GUIManager.Instance.GhostBar.alpha = 0;
    }
    */

	void Start()
    {
        GameManager.Instance.DisallowPauseMenu = true;
        GUIManager.Instance.OpenMenu(this.StartupMenu);
        GUIManager.Instance.EnergyBar.alpha = 0;
        GUIManager.Instance.GhostBar.alpha = 0;
        GUIManager.Instance.Tutorial1Shown = false;
        GUIManager.Instance.Tutorial2Shown = false;
        GUIManager.Instance.Tutorial3Shown = false;
        GUIManager.Instance.Tutorial4Shown = false;
        GUIManager.Instance.Tutorial5Shown = false;
    }

    public void StartGame()
    {
        GameManager.LoadLevel("Tutorial1");
    }

    public void LevelSelect()
    {
        GUIManager.Instance.OpenMenu(this.LevelSelection);
    }

    public void Back()
    {
        GUIManager.Instance.BackFromCurrentMenu();
    }

    public void LoadLevel1()
    {
        GameManager.LoadLevel(Level1);
    }

    public void LoadLevel2()
    {
        GameManager.LoadLevel(Level2);
    }

    public void LoadLevel3()
    {
        GameManager.LoadLevel(Level3);
    }

    public void LoadLevel4()
    {
        GameManager.LoadLevel(Level4);
    }

    public void LoadLevel5()
    {
        GameManager.LoadLevel(Level5);
    }

    public void LoadLevel6()
    {
        GameManager.LoadLevel(Level6);
    }

    public void LoadLevel7()
    {
        GameManager.LoadLevel(Level7);
    }

    public void LoadLevel8()
    {
        GameManager.LoadLevel(Level8);
    }
}
