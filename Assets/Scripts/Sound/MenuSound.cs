using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSound : MonoBehaviour
{
    public AK.Wwise.State state;
    void Update()
    {
        state.SetValue();
    }
}
