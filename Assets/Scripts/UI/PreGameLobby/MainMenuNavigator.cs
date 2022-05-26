using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// allow navigation through non networked menus
public class MainMenuNavigator : MonoBehaviour
{
    public void MenuBtnClick(GameObject menu)
    {
        menu.SetActive(true);    
    }

    public void ReturnToMainClick(GameObject menu)
    {
        menu.SetActive(false);
    }
}
