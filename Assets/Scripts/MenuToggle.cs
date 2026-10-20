using UnityEngine;

public class MenuToggle : MonoBehaviour
{
    public GameObject menu;   // the panel to open and close

    public void Toggle()
    {
        menu.SetActive(!menu.activeSelf);
    }
}