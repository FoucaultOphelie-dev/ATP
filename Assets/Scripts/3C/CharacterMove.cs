using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterMove : MonoBehaviour
{
    #region PUBLIC FIELDS

    [Header("Walk / Run Setting")]
    public KeyCode keyRun;
    public float speedWalk;
    public float speedRun;
    public float playerForce;
    public ForceMode appliedForceMode;
    public bool CanRun = true;
    public bool scaled = true;
    public float factorMove = 1.0f;

    [Header("Slide Setting")]
    public KeyCode keySlide;
    public float timeOfSlide;
    public float speedDuringSlide;
    public float playerSlideForce;
    public ForceMode appliedSlideForceMode;
    public GameObject body;
    public GameObject cam;
    public Animator animCam;
    public AK.Wwise.Event wwiseEventSlide;


    [Header("Jump Settings")]
    public KeyCode keyJump;
    public float playerJumpForce;
    public ForceMode appliedJumpForceMode;
    public float factorMoveJump;
    public AK.Wwise.Event wwiseEventJump;

    [Header("Wall Run")]
    public bool wallRunRight;
    public bool wallRunLeft;
    public float timeOfWallRun;
    public int wallRunLayer;
    public float playerWallRunForce;
    public float factorMoveWallRun;

    [Header("Grab State")]
    public bool isGrab = false;

    [Header("Move State")]
    public bool CanMove = true;

    [Header("Jumping State")] public bool playerIsJumping;

    [Header("Current Player Speed")] public float speed;

    [Header("Ground")] public int groundLayer; public bool playerIsGrounded = true;

    [Header("Viser")]
    public GameObject weapon;
    public bool isAiming = false;
    private Gun gun;

    public Text reloadMessage;
    public Text bullets;

    [Header("Animator")]
    public Animator animator;

    [Header("Sound footsptep")]
    public float frequencyWalk = 0.5f;
    public float frequencuRun = 0.3f;
    private float frequency;
    private float _frequencyTimer = 0f;
    public AK.Wwise.Event wwiseEventFootStep;

    #endregion


    #region PRIVATE FIELDS

    private float m_xAxis;
    private float m_zAxis;
    private Rigidbody m_rb;
    private int m_groundLayerMask;
    private Vector3 m_direction;
    private float m_deltaTime;
    private bool canJump = false;
    private bool canSlide = false;
    private bool inSlide = false;
    private float initialSpeedAnimator;
    private RaycastHit hit;


    private float timeInwallRight;
    private float timeInwallLeft;
    private float initialPlayerWallRunForce;

    private Quaternion rotationInitialBody;
    private Quaternion rotationInitialCam;

    public bool bobbing = true;

    private Vector3 slideInitialPosition;

    //Saved state for reset
    private bool wasAiming;
    #endregion

    void Start()
    {
        ParkourManager.OnCheckpointDone += (int index, float time, float lastTime) => {
            wasAiming = isAiming;
        };
        ParkourManager.OnCheckpointReset += () => {
            isAiming = wasAiming;
        };
        ParkourManager.OnParkourReset += () =>
        {
            isAiming = false;
        };
            #region INITIALISATION
        m_rb = GetComponent<Rigidbody>();

        speed = speedWalk;

        animator = GetComponent<Animator>();
        initialSpeedAnimator = animator.speed;
        animator.SetBool("Walk", false);
        animator.SetBool("Run", false);

        m_groundLayerMask = groundLayer;
        playerIsGrounded = true;
        playerIsJumping = false;
        initialPlayerWallRunForce = playerWallRunForce;
        gun = weapon.transform.GetComponent<Gun>();
        //CheckForGround();
        #endregion
    }

    void Update()
    {
        #region DELTATIME SCALE
        if (scaled)
        {
            m_deltaTime = Time.deltaTime;
        }
        else
        {
            m_deltaTime = Time.unscaledDeltaTime;
            animator.speed = initialSpeedAnimator / Time.timeScale;
        }
        #endregion

        #region WALL RUN
        if (wallRunRight)
        {
            timeInwallRight += m_deltaTime;
            factorMove = factorMoveWallRun;
            bobbing = false;
            GetComponent<CameraMove>().active = false;
            cam.GetComponent<Animator>().Play("WallRightCam", 0, 0);
            m_rb.AddForce(playerWallRunForce * m_rb.mass * m_deltaTime * transform.TransformDirection(Vector3.right), ForceMode.Force);
            if (timeInwallRight >= timeOfWallRun)
            {
                m_rb.useGravity = true;
                playerWallRunForce -= m_deltaTime;
            }
            else
            {
                m_rb.useGravity = false;
                m_rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;
            }
        }
        else
        {
            timeInwallRight = 0;
        }

        if (wallRunLeft)
        {
            timeInwallLeft += m_deltaTime;
            factorMove = factorMoveWallRun;
            bobbing = false;
            GetComponent<CameraMove>().active = false;
            cam.GetComponent<Animator>().Play("WallLeftCam", 0, 0);
            m_rb.AddForce(playerWallRunForce * m_rb.mass * m_deltaTime * transform.TransformDirection(Vector3.left), ForceMode.Force);
            if (timeInwallLeft >= timeOfWallRun)
            {
                m_rb.useGravity = true;
                playerWallRunForce -= m_deltaTime;
            }
            else
            {
                m_rb.useGravity = false;
                m_rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;

            }
        }
        else
        {
            timeInwallLeft = 0;
        }

        if (!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, 1.0f, LayerMask.GetMask("WallRun")))
        {
            if (wallRunRight)
            {
                wallRunRight = false;
                OnExitWallRun();
            }
        }

        if (!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, 1.0f, LayerMask.GetMask("WallRun")))
        {
            if (wallRunLeft)
            {
                wallRunLeft = false;
                OnExitWallRun();
            }
        }   

        #endregion

        #region GRAB
        if (isGrab)
        {
            m_rb.velocity = new Vector3(0, 0, 0);
            m_rb.angularVelocity = new Vector3(0, 0, 0);
            //m_rb.isKinematic = true;
            m_rb.useGravity = false;
            GetComponent<CameraMove>().inSlide = true;
            animator.SetBool("Attach", true);
            animator.SetBool("DoJump", false);
        }
        #endregion

        #region FREEZE ROTATION
        if (playerIsGrounded && transform.rotation.x != 0.0f || transform.rotation.z != 0.0f)
        {
            transform.rotation = new Quaternion(0.0f, transform.rotation.y, 0.0f, transform.rotation.w);
        }
        #endregion

        #region AIMING
        if (Input.GetButtonDown("Fire2") && CanMove)
        {
            isAiming = true;
            CanRun = false;
            //weapon.SetActive(true);
        }

        if (!Input.GetButton("Fire2") && !gun.IsReloading())
        {
            isAiming = false;
            CanRun = true;
            //weapon.SetActive(false);
        }
        if(isAiming) CanRun = false;
        else CanRun = true;
        #endregion

        #region SLIDE
        if (Input.GetKeyDown(keySlide) && !inSlide && CanMove)
        {
            canSlide = true;
        }
        #endregion

        #region MOVE
        m_xAxis = Input.GetAxisRaw("Horizontal") * factorMove;
        m_zAxis = Input.GetAxisRaw("Vertical") * factorMove;
        m_direction = new Vector3(m_xAxis, 0f, m_zAxis).normalized;
        CheckForGround();

        if (inSlide)
        {
            speed = speedDuringSlide;
        }
        else
        {
            if (m_xAxis != 0 || m_zAxis != 0)
            {
                if (CanRun)
                {
                    speed = Input.GetKey(keyRun) ? speedRun : speedWalk;
                    frequency = Input.GetKey(keyRun) ? frequencuRun : frequencyWalk;
                }
                else
                {
                    speed = speedWalk;
                }
            }
            else
            {
                speed = 0;
                if (playerIsGrounded)
                {
                    m_rb.velocity = Vector3.zero;
                }
            }
        }
        if (m_direction != Vector3.zero)
        {
            _frequencyTimer += Time.deltaTime;
            if (_frequencyTimer >= frequency)
            {
                wwiseEventFootStep.Post(gameObject);
                _frequencyTimer = 0f;
            }
        }
        else
        {
            _frequencyTimer = 0f;
        }
        if (bobbing)
        {
            if (!animCam.GetBool("bobbing"))
            {
                animCam.SetBool("bobbing", true);
            }
            animCam.speed = speed;
        }
        else
        {
            if (animCam.GetBool("bobbing"))
            {
                animCam.SetBool("bobbing", false);
            }
        }
        #endregion

        #region JUMP
        if (Input.GetKeyDown(keyJump) && CanMove)
        {
            if (playerIsGrounded || isGrab || wallRunLeft || wallRunRight)
            {
                canJump = true;
            }
        }
        #endregion
    }

    private void FixedUpdate()
    {
        #region DELTATIME SCALE
        if (scaled)
        {
            m_deltaTime = Time.fixedDeltaTime;
        }

        else
        {
            m_deltaTime = Time.fixedUnscaledDeltaTime;
        }
        #endregion

        #region MOVE
        if (CanMove)
        {
            MoveCharacter();
        }
        #endregion

        #region SLIDE
        if (canSlide)
        {
            canSlide = false;
            GetComponent<CameraMove>().inSlide = true;
            inSlide = true;
            CanMove = false;
            Slide(playerSlideForce, appliedSlideForceMode);
        }
        #endregion

        #region JUMP
        if (canJump)
        {
            if (playerIsGrounded)
            {
                canJump = false;
                Jump(playerJumpForce, appliedJumpForceMode, Vector3.up);
            }
            if (isGrab)
            {
                canJump = false;
                gameObject.GetComponent<CameraMove>().inSlide = false;
                animator.SetBool("Slide", false);
                Ungrab();
                playerIsGrounded = false;
                Jump(playerJumpForce, appliedJumpForceMode, transform.TransformDirection(-Vector3.forward));
                turnCharacter(transform.TransformDirection(-Vector3.forward));

            }
            if (wallRunLeft)
            {
                canJump = false;
                Jump(playerJumpForce, appliedJumpForceMode, (transform.TransformDirection(Vector3.right)*9 + transform.TransformDirection(Vector3.up)).normalized);
            }
            if (wallRunRight)
            {
                canJump = false;
                Jump(playerJumpForce, appliedJumpForceMode, (transform.TransformDirection(Vector3.left)*9+ transform.TransformDirection(Vector3.up)).normalized);
            }
        }
        #endregion
    }

    private void OnExitWallRun()
    {
        m_rb.useGravity = true;
        bobbing = true;
        cam.GetComponent<Animator>().Play("Bobbing");
        GetComponent<CameraMove>().active = true;
        playerWallRunForce = initialPlayerWallRunForce;
        m_rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void MoveCharacter()
    {
        if (m_direction != Vector3.zero)
        {
            if (speed == speedWalk)
            {
                animator.SetBool("Walk", true);
                animator.SetBool("Run", false);
                
            }
            else
            {
                animator.SetBool("Walk", false);
                animator.SetBool("Run", true);
            }
            

            //Debug.Log(cam.GetComponent<Animator>().name);

            Vector3 force = playerForce * m_deltaTime * speed * transform.TransformDirection(m_direction);
            if (playerIsJumping) force *= factorMoveJump;
            m_rb.AddForce(force, appliedForceMode);
        }
        else
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
        }
        
    }

    private void Slide(float jumpForce, ForceMode forceMode)
    {
        slideInitialPosition = cam.transform.parent.localPosition;
        animator.SetBool("Slide", true);
        body.transform.localScale = new Vector3(0.58394f, 0.5f, 1.0f);
        Vector3 slidePos = slideInitialPosition;
        slidePos.y = 0.5f;
        cam.transform.parent.localPosition = slidePos;
        m_rb.AddForce(jumpForce * m_rb.mass * m_deltaTime * transform.TransformDirection(Vector3.forward), forceMode);
        wwiseEventSlide.Post(gameObject);
        StartCoroutine(CoSlide());
    }

    IEnumerator CoSlide()
    {
        yield return new WaitForSeconds(timeOfSlide);
        wwiseEventSlide.Stop(gameObject);
        body.transform.localScale = new Vector3(0.58394f, 1.99015f, 1.0f);
        cam.transform.parent.localPosition = slideInitialPosition;
        animator.SetBool("Slide", false);
        inSlide = false;
        GetComponent<CameraMove>().inSlide = false;
        CanMove = true;
    }
    private void Jump(float jumpForce, ForceMode forceMode, Vector3 direction)
    {
        playerIsGrounded = false;
        wwiseEventJump.Post(gameObject);
        //Debug.Log(transform.TransformDirection(Vector3.forward) * speed * m_deltaTime);
        m_rb.AddForce((jumpForce * m_rb.mass * Time.deltaTime * direction), forceMode);
        playerIsJumping = true;
        factorMove = factorMoveJump;
        animator.SetBool("DoJump", true);
    }

    public void Ungrab()
    {
        isGrab = false;
        CanMove = true;
        playerIsGrounded = true;
        playerIsJumping = false;
        factorMove = 1.0f;
        animator.SetBool("DoJump", false);
        //m_rb.isKinematic = false;
        m_rb.useGravity = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("it's collide with " + collision.gameObject.name);
        if (collision.gameObject.layer == m_groundLayerMask)
        {
            CheckForGround();
        }
        if (collision.gameObject.layer == wallRunLayer)
        {
            Debug.Log("count");
            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, 1))
            {
                if (!wallRunRight)
                {
                    wallRunRight = true;
                    m_rb.velocity = Vector3.zero;
                    float angleLeft = Vector3.Angle(transform.TransformDirection(Vector3.forward), collision.gameObject.transform.TransformDirection(Vector3.forward));
                    float angleRight = Vector3.Angle(transform.TransformDirection(Vector3.forward), collision.gameObject.transform.TransformDirection(-Vector3.forward));
                    if (angleLeft <= angleRight)
                    {
                        transform.rotation = collision.gameObject.transform.rotation;
                    }
                    else
                    {
                        transform.rotation = collision.gameObject.transform.rotation;
                        transform.Rotate(new Vector3(0, 180, 0));
                    }
                    playerIsJumping = false;
                    animator.SetBool("DoJump", false);
                    factorMove = 1.0f;
                    CanMove = true;
                }
            }

            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, 1))
            {
                if (!wallRunLeft)
                {
                    wallRunLeft = true;
                    m_rb.velocity = Vector3.zero;
                    float angleLeft = Vector3.Angle(transform.TransformDirection(Vector3.forward), collision.gameObject.transform.TransformDirection(Vector3.forward));
                    float angleRight = Vector3.Angle(transform.TransformDirection(Vector3.forward), collision.gameObject.transform.TransformDirection(-Vector3.forward));
                    if (angleLeft <= angleRight)
                    {
                        transform.rotation = collision.gameObject.transform.rotation;
                    }
                    else
                    {
                        transform.rotation = collision.gameObject.transform.rotation;
                        transform.Rotate(new Vector3(0, 180, 0));
                    }
                    
                    playerIsJumping = false;
                    animator.SetBool("DoJump", false);
                    factorMove = 1.0f;
                    CanMove = true;
                }
                
            }
            
        }
    }

    private void CheckForGround()
    {
        if (Physics.Raycast(transform.position + Vector3.up, transform.TransformDirection(-Vector3.up), out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(-Vector3.up) * hit.distance, Color.yellow);
            //Debug.Log("distance = " + Vector3.Distance(hit.point, transform.position));
            if (Vector3.Distance(hit.point, transform.position) < 0.1f)
            {
                //Debug.Log("canmove :" + CanMove);
                playerIsGrounded = true;
                playerIsJumping = false;
                if (ParkourManager.Instance())
                {
                    if (ParkourManager.Instance().GetIsStarted())
                    {
                        CanMove = true;
                    }
                }
                else
                {
                    CanMove = true;
                }
                factorMove = 1.0f;
                animator.SetBool("DoJump", false);
                transform.parent = hit.transform.parent;
            }
            else
            {
                playerIsGrounded = false;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == m_groundLayerMask)
        {
            if(collision.transform.parent == transform.parent)
            {
                transform.parent = null;
                playerIsGrounded = false;
            }
        }
    }

    private void turnCharacter(Vector3 direction)
    {
        float angleRotation = Vector3.Angle(transform.TransformDirection(Vector3.forward), direction);
        transform.Rotate(0,angleRotation,0,0);
    }


}
