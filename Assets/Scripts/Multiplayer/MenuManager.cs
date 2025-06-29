using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public static MenuManager instance;

    public Menu[] menus;

    /*   void Awake()
       {
           instance = this;
       }
   */
    void Awake()
    {
      //  instance = this;
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // 💡 bu satır her şeyi çözer
    }

    public void OpenMenu(string menuName)
    {

        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].menuName == menuName)
            {
                Debug.Log("open: " + menus[i]);
                menus[i].Open();
            }
            else if (menus[i].isOpen)
            {
                Debug.Log("close: " + menus[i]);
                CloseMenu(menus[i]);
            }
        }
    }

    public void OpenMenu(Menu menu)
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (menus[i].isOpen)
            {
                CloseMenu(menus[i]);
            }
        }
        menu.Open();
    }

    public void CloseMenu(Menu menu)
    {
        menu.Close();
    }

    public void CloseAllMenus()
    {
        Debug.Log("Close all menus");
        foreach (Menu menu in menus)
        {
            menu.Close();
        }
    }
}
