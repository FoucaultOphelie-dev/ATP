using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParkourManager : MonoBehaviour
{
    public delegate bool CheckpointDone(int index);
    public static event CheckpointDone onCheckpointDone;
    public KeyCode softResetKey = KeyCode.R;
    public KeyCode hardResetKey= KeyCode.Backspace;

    private List<CheckPoint> checkpoints;
    public GameObject player;
    private Rigidbody playerRigidbody;
    private static ParkourManager instance;
    int lastCheckpoint = 0;

    private bool isStarted;
    public bool GetIsStarted() { return isStarted; }
    private bool isFinished;
    public bool GetIsFinished() { return isFinished; }
    private float timer;
    public float GetTimer() { return timer; }

    private Vector3 lastPos;
    private Quaternion lastRotation;
    private Vector3 lastVelocity;
    private int lastScore;

    public static ParkourManager Instance()
    {
        if (instance)
        {
            return instance;
        }
        else
        {
            Debug.LogError("Parkour Manager is not instanced (missing ParkourManager in the scene");
            return null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerRigidbody = player.GetComponent<Rigidbody>();
        checkpoints = GameObject.FindObjectsOfType<CheckPoint>().OrderBy(checkpoint => checkpoint.index).ToList<CheckPoint>();
        lastPos = player.transform.position;
        lastRotation = player.transform.rotation;
        checkpoints[0].transform.parent.GetComponent<Animator>().SetTrigger("Start");
    }

    // Update is called once per frame
    void Update()
    {
        if (isStarted && !isFinished)
        {
            timer += Time.deltaTime * Time.timeScale;
        }
        if (Input.GetKeyDown(softResetKey))
        {
            player.transform.position = lastPos;
            player.transform.rotation = lastRotation;
            playerRigidbody.velocity = lastVelocity;
        }
        if (Input.GetKeyDown(hardResetKey))
        {
            player.transform.position = checkpoints[0].transform.position;
            player.transform.rotation = Quaternion.identity;
            playerRigidbody.velocity = Vector3.zero;
            timer = 0;
        }
    }
    public void StartRun()
    {
        Debug.Log("started");
        isStarted = true;
    }
    public bool OnCheckpointDone(int index)
    {
        if(index == lastCheckpoint + 1)
        {
            if(index < checkpoints.Count - 1)
            {
                lastCheckpoint++;
                lastPos = player.transform.position;
                lastRotation = player.transform.rotation;
                lastVelocity = playerRigidbody.velocity;
                lastScore = 0;
                onCheckpointDone?.Invoke(index);
            }
            else
            {
                Time.timeScale = 0;
                isFinished = true;
            }
            return true;
        }
        return false;
    }
}
