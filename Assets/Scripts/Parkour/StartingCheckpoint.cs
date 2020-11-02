using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartingCheckpoint : MonoBehaviour
{
    public void TriggerStartRun()
    {
        ParkourManager.Instance().StartRun();
    }
}
