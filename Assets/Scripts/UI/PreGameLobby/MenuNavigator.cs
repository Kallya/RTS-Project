using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// allow pseudo-navigation through non networked menus
public class MenuNavigator : MonoBehaviour
{
    public void MenuBtnClick(GameObject menu)
    {
        menu.SetActive(true);    
    }

    public void ExitClick(GameObject menu)
    {
        menu.SetActive(false);
    }

    public void ExitAppClick()
    {
        Application.Quit();
    }
}
