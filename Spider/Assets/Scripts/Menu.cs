using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Image imageButton1;
    public Image imageButton2;
    public Image imageButton3;
    public Sprite high;
    public Sprite low;
    public void SetLevel(int level)
    {
        imageButton1.sprite = low;
        imageButton2.sprite = low;
        imageButton3.sprite = low;
        if (level == 1)
        {
            imageButton1.sprite = high;
            GameController.level = 0;
        }
        else if (level == 2)
        {
            imageButton2.sprite = high;
            GameController.level = 1;
        }
        else if (level == 3)
        {
            imageButton3.sprite = high;
            GameController.level = 2;
        }
    }
    public void Exit()
    {
        Application.Quit();
    }
}
