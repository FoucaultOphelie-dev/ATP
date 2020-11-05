using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParkourManager : MonoBehaviour
{
    public delegate void CheckpointDone(int index, float time, float previousTime);
    public static event CheckpointDone OnCheckpointDone;

    public delegate bool ParkourDone();
    public static event ParkourDone onParkourDone;

    public KeyCode softResetKey = KeyCode.R;
    public KeyCode hardResetKey= KeyCode.Backspace;

    public Parkour parkourData;

    private StartingCheckpoint startingCheckpoint;
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
    public List<float> timerByCheckpoint = new List<float>();
    public float GetTimer() { return timer; }

    private Vector3 lastPos;
    private Quaternion lastRotation;
    private Quaternion lastCameraPitch;
    private Vector3 lastVelocity;
    private float lastTime;
    private int lastScore;
    public int score;
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
        timerByCheckpoint = new List<float>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerRigidbody = player.GetComponent<Rigidbody>();
        checkpoints = GameObject.FindObjectsOfType<CheckPoint>().OrderBy(checkpoint => checkpoint.index).ToList<CheckPoint>();
        startingCheckpoint = GameObject.FindObjectOfType<StartingCheckpoint>();
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
            Camera.main.transform.rotation = lastCameraPitch;
            timer = lastTime;
        }
        if (Input.GetKeyDown(hardResetKey))
        {
            player.transform.position = checkpoints[0].transform.position;
            player.transform.rotation = Quaternion.identity;
            playerRigidbody.velocity = Vector3.zero;
            timer = 0;
            lastCheckpoint = 0;
            isFinished = false;
            isStarted = false;
            foreach (var checkpoint in checkpoints)
            {
                checkpoint.ResetCheckpoint();
                timerByCheckpoint.Clear();
            }
            startingCheckpoint.ResetRun();
        }
    }
    public void StartRun()
    {
        Debug.Log("started");
        isStarted = true;
    }
    public bool PlayerEnterCheckpoint(int index)
    {
        if(index == lastCheckpoint + 1)
        {
            timerByCheckpoint.Add(timer);
            float previousTime = -1;
            Debug.Log((index - 1) + "<" + parkourData.timerByCheckpoint.Count);
            if (index - 1 < parkourData.timerByCheckpoint.Count)
                previousTime = parkourData.timerByCheckpoint[index - 1];
            OnCheckpointDone?.Invoke(index, timer, previousTime);
            if (index < checkpoints.Count - 1)
            {
                lastCheckpoint++;
                lastPos = player.transform.position;
                lastRotation = player.transform.rotation;
                lastVelocity = playerRigidbody.velocity;
                lastCameraPitch = Camera.main.transform.rotation;
                lastTime = timer;
                lastScore = 0;
            }
            else
            {
                // if player done a better time save it
                if(parkourData.timerByCheckpoint.Count > 0)
                {
                    if (timerByCheckpoint[index - 1] < parkourData.timerByCheckpoint[index - 1])
                        parkourData.timerByCheckpoint = new List<float>(timerByCheckpoint);
                }
                else
                {
                    parkourData.timerByCheckpoint = new List<float>(timerByCheckpoint);
                }

                Time.timeScale = 0;
                score = CalculateScore();
                isFinished = true;
                onParkourDone?.Invoke();
            }
            return true;
        }
        return false;
    }

    private int CalculateScore()
    {
        int score = parkourData.startingScore - (int)(timer * 10);
        return score;
    }
}
