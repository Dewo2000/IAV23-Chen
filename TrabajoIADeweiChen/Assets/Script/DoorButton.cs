using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    private bool buttonDown = false;
    // Start is called before the first frame update

    // Update is called once per frame
    public bool isOpen()
    {
        return buttonDown;
    }
    public void ResetButton()
    {
        GetComponent<Renderer>().material.color = new Color(1, 1, 1, 1f);
        buttonDown = false;
    }
    public bool pushButton()
    {
        if (!buttonDown)
        {
            GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.6f);
            buttonDown = true;
            return true;
        }
        else
        {
            return false;
        }
    }
}
