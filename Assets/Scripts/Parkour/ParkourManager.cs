using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
public enum ParkourState
{
    Intro,
    Gameplay,
    Scoring
}
public class ParkourManager : MonoBehaviour
{

    #region Event
    [HideInInspector()]
    public ParkourState parkourState;
    public delegate void ParkourSwitchState(ParkourState state);
    public static event ParkourSwitchState OnParkourSwitchState;

    public delegate void CheckpointDone(int index, float time, float previousTime);
    public static event CheckpointDone OnCheckpointDone;

    public delegate void ParkourReset();
    public static event ParkourReset OnParkourReset;
    #endregion

    public KeyCode softResetKey = KeyCode.R;
    public KeyCode hardResetKey = KeyCode.Backspace;

    public Parkour parkourData;

    private StartingCheckpoint startingCheckpoint;
    private List<CheckPoint> checkpoints;

    public GameObject player;
    private Rigidbody playerRigidbody;
    private CharacterMove playerMovement;
    private CameraMove cameraMove;
    private TimeManager timeManager;
    private GameObject spectatingCamera;
    public Camera scoringCamera;
    int lastCheckpoint = 0;
    bool firstRun = true;

    private bool isStarted;
    public bool GetIsStarted() { return isStarted; }
    private bool isFinished;
    public bool GetIsFinished() { return isFinished; }
    private float timer;
    public List<float> timerByCheckpoint = new List<float>();
    public List<float> BesttimerByCheckpoint = new List<float>();
    public float GetTimer() { return timer; }

    private Vector3 lastPos;
    private Quaternion lastRotation;
    private Quaternion lastCameraPitch;
    private Vector3 lastVelocity;
    private float lastTime;
    //private int lastScore;
    [HideInInspector()]
    public int score;
    public bool newBestScore = false;

    private Platform[] platforms;
    private List<Transform> targetBuffer;
    private List<Hit> hitBuffer;
    private List<ParkourTrigger> triggerBuffer;
    private List<ParkourTrigger> alreadyTriggered;
    private List<Transform> targets;
    public List<Hit> hits;

    public float maxCompletion;
    public float currentCompletionCheckpoint;
    public float alreadyDone;
    //private float currentCompletionParkour;
    public float parkourLenght;
    private List<float> lenghtByCheckpoint;

    private static ParkourManager instance;
    public static ParkourManager Instance()
    {
        if (instance)
        {
            return instance;
        }
        else
        {
            ParkourManager manager = GameObject.FindObjectOfType<ParkourManager>();
            if (manager)
            {
                instance = manager;
                return instance;
            }
            else
            {
                Debug.LogError("Parkour Manager is not instanced (missing ParkourManager in the scene");
                return null;
            }
        }
    }

    private void Awake()
    {
        instance = this;
        OnCheckpointDone = null;
        OnParkourSwitchState = null;
        timerByCheckpoint = new List<float>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerRigidbody = player.GetComponent<Rigidbody>();
        playerMovement = player.GetComponent<CharacterMove>();
        timeManager = FindObjectOfType<TimeManager>();
        cameraMove = player.GetComponent<CameraMove>();
        scoringCamera = player.transform.Find("Body/ScoringCamera").GetComponent<Camera>();

        checkpoints = GameObject.FindObjectsOfType<CheckPoint>().OrderBy(checkpoint => checkpoint.index).ToList<CheckPoint>();
        lenghtByCheckpoint = new List<float>();
        for (int i = 1; i < checkpoints.Count; i++)
        {
            float lenght = (checkpoints[i - 1].transform.position - checkpoints[i].transform.position).magnitude;
            lenghtByCheckpoint.Add(lenght);
            parkourLenght += lenght;
        }

        startingCheckpoint = GameObject.FindObjectOfType<StartingCheckpoint>();
        lastPos = player.transform.position;
        lastRotation = player.transform.rotation;


        spectatingCamera = GameObject.Find("SpectatingCamera");
        if (!spectatingCamera)
        {
            spectatingCamera = GameObject.FindGameObjectWithTag("SpectatingCamera");
        }

        targetBuffer = new List<Transform>();
        hitBuffer = new List<Hit>();
        triggerBuffer = new List<ParkourTrigger>();
        alreadyTriggered = new List<ParkourTrigger>();
        targets = new List<Transform>();
        hits = new List<Hit>();
        platforms = FindObjectsOfType<Platform>();

        //Load best run
        ParkourSaveData data = loadData(parkourData);

        BesttimerByCheckpoint = new List<float>();
        if (data != null)
        {
            BesttimerByCheckpoint = new List<float>(data.timerByCheckpoint);
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        SwitchParkourState(ParkourState.Intro);
        ParkourTrigger.OnTrigger += TriggerParkour;
        Gun.OnTargetHit += TargetHit;
        Fadder.BeginFadeIn();
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
            currentCompletionCheckpoint = (FindNearestPointOnLine(
                checkpoints[lastCheckpoint].transform.position,
                checkpoints[lastCheckpoint + 1].transform.position,
                player.transform.position) - checkpoints[lastCheckpoint].transform.position).magnitude;
            if (currentCompletionCheckpoint > maxCompletion) maxCompletion = currentCompletionCheckpoint;
            //Debug.Log(currentCompletionCheckpoint + "/" +);
        }
        if (Input.GetKeyDown(softResetKey))
        {
            foreach (Platform platform in platforms) platform.SoftReset();
            player.transform.position = lastPos;
            player.transform.rotation = lastRotation;
            playerRigidbody.velocity = lastVelocity;
            Camera.main.transform.rotation = lastCameraPitch;
            timer = lastTime;
            foreach(var trigger in triggerBuffer)
            {
                trigger.ResetTrigger();
            }
            triggerBuffer.Clear();
            targetBuffer.Clear();
            hitBuffer.Clear();
            maxCompletion = 0;
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
        playerMovement.bobbing = true;
        timeManager.powerActivate = true;
    }

    public bool PlayerEnterCheckpoint(int index)
    {
        if(index == lastCheckpoint + 1)
        {
            // Save Time
            timerByCheckpoint.Add(timer);
            float previousTime = -1;
            if (index - 1 < BesttimerByCheckpoint.Count)
                previousTime = BesttimerByCheckpoint[index - 1];

            // handle Old State
            hits.AddRange(hitBuffer);
            hitBuffer.Clear();
            targetBuffer.Clear();
            alreadyTriggered.AddRange(triggerBuffer);
            triggerBuffer.Clear();

            // Trigger Checkpoint Done
            OnCheckpointDone?.Invoke(index, timer, previousTime);
            if (index < checkpoints.Count - 1)
            {
                alreadyDone += lenghtByCheckpoint[lastCheckpoint];
                maxCompletion = 0;
                // Save State
                lastCheckpoint++;
                lastPos = player.transform.position;
                lastRotation = player.transform.rotation;
                lastVelocity = playerRigidbody.velocity;
                lastCameraPitch = Camera.main.transform.rotation;
                lastTime = timer;
            }
            else
            {
                FinishParkour();
            }
            return true;
        }
        return false;
    }
    static public ParkourSaveData loadData(Parkour parkour)
    {
        if (ProfileManager.Instance().IsCurrentProfileValid())
        {
            Profile profile = ProfileManager.Instance().GetCurrentProfile();
            if (profile.parkoursSaveData.ContainsKey(parkour.guid))
            {
                return profile.parkoursSaveData[parkour.guid];
            }
        }
        return null;
    }
    public void FinishParkour()
    {
        score = CalculateScore();
        isFinished = true;
        playerMovement.enabled = false;
        playerMovement.cam.GetComponent<Camera>().enabled = false;
        scoringCamera.enabled = true;
        cameraMove.enabled = false;
        spectatingCamera.SetActive(true);
        spectatingCamera.GetComponent<Camera>().enabled = true;
        SwitchParkourState(ParkourState.Scoring);

        ParkourSaveData data = loadData(parkourData);
        if (data == null)
        {
            data = new ParkourSaveData();
        }

        if(score > data.bestScore)
        {
            newBestScore = true;
            BesttimerByCheckpoint = new List<float>(timerByCheckpoint);
            data.bestScore = score;
            data.timerByCheckpoint = new List<float>(timerByCheckpoint);
            int previousBestMedal = data.bestMedalObtained;
            int i = parkourData.medals.Length - 1;
            while (i >= 0)
            {
                if (score < parkourData.medals[i].score) break;
                i--;
            }
            data.bestMedalObtained = i+1;
            if (ProfileManager.Instance().IsCurrentProfileValid())
            {
                Profile profile = ProfileManager.Instance().GetCurrentProfile();
                if (profile.parkoursSaveData.ContainsKey(parkourData.guid))
                {
                    profile.parkoursSaveData[parkourData.guid] = data;
                }
                else
                {
                    profile.parkoursSaveData.Add(parkourData.guid, data);
                }
                int j = data.bestMedalObtained;
                while (j < previousBestMedal)
                {
                    profile.medalsObtained[j]++;
                    j++;
                }
                ProfileManager.Instance().SaveCurrentProfile();
            }
        }
    }

    public int TimeScore()
    {
        return parkourData.startingScore - (int)(timer * 10);
    }
    public int CalculateScore()
    {
        int score = TimeScore();
        foreach(var hit in hits){
            score += hit.score;
        }
        return score > 0 ? score : 0;
    }

    private void TargetHit(Hit feedback, Transform target)
    {
        hitBuffer.Add(feedback);
        targetBuffer.Add(target);
    }

    private void TriggerParkour(ParkourTrigger trigger)
    {
        triggerBuffer.Add(trigger);
    }

    private void SwitchParkourState(ParkourState state)
    {
        parkourState = state;
        OnParkourSwitchState?.Invoke(parkourState);
    }

    public static void ResetGameplay()
    {
        ParkourManager instance = ParkourManager.Instance();
        if (!instance) return;
        instance.spectatingCamera.GetComponent<Camera>().enabled = false;
        instance.scoringCamera.enabled = false;
        instance.spectatingCamera.SetActive(false);
        instance.playerMovement.cam.GetComponent<Camera>().enabled = true;
        instance.playerMovement.enabled = true;
        instance.playerMovement.CanMove = false;
        instance.playerMovement.bobbing = false;
        instance.cameraMove.enabled = true;
        instance.player.GetComponent<Animator>().Rebind();

        if (instance.firstRun)
        {
            instance.startingCheckpoint.StartRun();
        }
        else
        {
            instance.player.transform.position = instance.checkpoints[0].transform.position;
            instance.player.transform.rotation = Quaternion.identity;
            instance.playerRigidbody.velocity = Vector3.zero;
            instance.timer = 0;
            instance.lastCheckpoint = 0;
            instance.isFinished = false;
            instance.isStarted = false;
            instance.newBestScore = false;
            instance.alreadyTriggered.AddRange(instance.triggerBuffer);
            foreach (var trigger in instance.alreadyTriggered)
            {
                trigger.ResetTrigger();
            }
            instance.alreadyTriggered.Clear();
            instance.triggerBuffer.Clear();
            instance.targetBuffer.Clear();
            instance.hitBuffer.Clear();
            instance.hits.Clear();
            foreach (Platform platform in instance.platforms) platform.HardReset();
            foreach (var checkpoint in instance.checkpoints)
            {
                checkpoint.ResetCheckpoint();
                instance.timerByCheckpoint.Clear();
            }
            instance.startingCheckpoint.ResetRun();
        }
        instance.firstRun = false;
        instance.SwitchParkourState(ParkourState.Gameplay);
        instance.alreadyDone = 0;
        instance.maxCompletion = 0;
        OnParkourReset?.Invoke();
    }
    public Vector3 FindNearestPointOnLine(Vector3 origin, Vector3 end, Vector3 point)
    {
        //Get heading
        Vector3 heading = (end - origin);
        float magnitudeMax = heading.magnitude;
        heading.Normalize();

        //Do projection from the point but clamp it
        Vector3 lhs = point - origin;
        float dotP = Vector3.Dot(lhs, heading);
        dotP = Mathf.Clamp(dotP, 0f, magnitudeMax);
        return origin + heading * dotP;
    }
    public List<CheckPoint> GetCheckpoints() { return checkpoints; }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (parkourState != ParkourState.Gameplay) return;
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(checkpoints[lastCheckpoint].transform.position,
                checkpoints[lastCheckpoint + 1].transform.position);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(player.transform.position, 0.5f);
        Gizmos.color = Color.green;
        Vector3 posOnLine = FindNearestPointOnLine(
                checkpoints[lastCheckpoint].transform.position,
                checkpoints[lastCheckpoint + 1].transform.position,
                player.transform.position);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(posOnLine, 0.5f);
    }
#endif
}
