using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ParkourState
{
    Intro,
    Gameplay,
    Scoring
}

public class ParkourManager : MonoBehaviour
{

    #region Event
    public ParkourState parkourState;
    public delegate void ParkourSwitchState(ParkourState state);
    public static event ParkourSwitchState onParkourSwitchState;

    public delegate void CheckpointDone(int index, float time, float previousTime);
    public static event CheckpointDone OnCheckpointDone;
    #endregion

    public KeyCode softResetKey = KeyCode.R;
    public KeyCode hardResetKey= KeyCode.Backspace;

    public Parkour parkourData;

    private StartingCheckpoint startingCheckpoint;
    private List<CheckPoint> checkpoints;

    public GameObject player;
    private Rigidbody playerRigidbody;
    private CharacterMove playerMovement;
    private CameraMove cameraMove;
    private GameObject spectatingCamera;
    int lastCheckpoint = 0;
    bool firstRun = true;

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

    private static ParkourManager instance;
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
        playerMovement = player.GetComponent<CharacterMove>();
        cameraMove = player.GetComponent<CameraMove>();

        checkpoints = GameObject.FindObjectsOfType<CheckPoint>().OrderBy(checkpoint => checkpoint.index).ToList<CheckPoint>();
        startingCheckpoint = GameObject.FindObjectOfType<StartingCheckpoint>();
        lastPos = player.transform.position;
        lastRotation = player.transform.rotation;
        SwitchParkourState(ParkourState.Intro);
        spectatingCamera = GameObject.Find("SpactatingCamera");
    }

    // Update is called once per frame
    void Update()
    {
        switch (parkourState)
        {
            case ParkourState.Intro:
                IntroLoop();
                break;
            case ParkourState.Gameplay:
                GameplayLoop();
                break;
            case ParkourState.Scoring:
                Scoringloop();
                break;
            default:
                break;
        }
    }

    public void IntroLoop()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ResetGameplay();
        }
    }

    public void GameplayLoop()
    {
        if (isStarted)
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
            ResetGameplay();
        }
    }

    public void Scoringloop()
    {

    }


    public void StartRun()
    {
        isStarted = true;
        playerMovement.CanMove = true;
    }

    public bool PlayerEnterCheckpoint(int index)
    {
        if(index == lastCheckpoint + 1)
        {
            timerByCheckpoint.Add(timer);
            float previousTime = -1;
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

                //Time.timeScale = 0;
                
                score = CalculateScore();
                isFinished = true;
                playerMovement.CanMove = false;
                playerMovement.cam.GetComponent<Camera>().enabled = false;
                spectatingCamera.SetActive(true);
                spectatingCamera.GetComponent<Camera>().enabled = true;
                SwitchParkourState(ParkourState.Scoring);
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

    private void SwitchParkourState(ParkourState state)
    {
        parkourState = state;
        onParkourSwitchState?.Invoke(parkourState);
    }

    public void ResetGameplay()
    {
        spectatingCamera.GetComponent<Camera>().enabled = false;
        spectatingCamera.SetActive(false);
        playerMovement.cam.GetComponent<Camera>().enabled = true;
        playerMovement.enabled = true;
        playerMovement.CanMove = false;
        player.GetComponent<Animator>().Rebind();
        cameraMove.enabled = true;
        if (firstRun)
        {
            startingCheckpoint.StartRun();
        }
        else
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
        firstRun = false;
        SwitchParkourState(ParkourState.Gameplay);
    }
}
