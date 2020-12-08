using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupSystem : MonoBehaviour
{
    public GameObject popUpBox;
    //public Animator animator;
    public TMP_Text popUpText;

    public void openPopUp(string text)
    {
        popUpBox.SetActive(true);
        popUpText.text = text;
        Cursor.visible = true;
        //animator.SetTrigger("pop");
    }
}
