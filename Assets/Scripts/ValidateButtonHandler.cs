using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidateButtonHandler : MonoBehaviour
{
    public GameObject popUpSystem;
    //public TimeManager timeManager;
    public void closePopUp()
    {
        popUpSystem.SetActive(false);
        Cursor.visible = false;
        //timeManager.unpauseGame();
    }
}
