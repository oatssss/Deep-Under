using UnityEngine;
using System.Collections.Generic;

public class GUIManager : UnitySingleton<GUIManager> {
    
    private bool GamePaused = false;
    [SerializeField] private Menu PauseMenu;
    private Menu CurrentMenu;
    private Stack<Menu> History = new Stack<Menu>();
    private enum TRANSITION { STACK, NOSTACK, CLOSE }
    
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
    
    private void PauseTime()
    {
        Time.timeScale = 0f;
    }
    
    private void ResumeTime()
    {
        Time.timeScale = 1f;
    }
    
    public void PauseGame()
    {
        Instance.GamePaused = true;
        Instance.PauseTime();
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
        Instance.ResumeTime();
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
