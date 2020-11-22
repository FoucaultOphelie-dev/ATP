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


    [Header("Jump Settings")]
    public KeyCode keyJump;
    public float playerJumpForce;
    public ForceMode appliedJumpForceMode;
    public float factorMoveJump;

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
    #endregion

    void Start()
    {
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
        if (scaled)
        {
            m_deltaTime = Time.deltaTime;
        }
        else
        {
            m_deltaTime = Time.unscaledDeltaTime;
            animator.speed = initialSpeedAnimator / Time.timeScale;
        }

        if (wallRunRight)
        {
            timeInwallRight += m_deltaTime;
            factorMove = factorMoveWallRun;
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
            wallRunRight = false;
        }
        else
        {
            Debug.Log("right" + hit.transform.name);
        }

        if (!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, 1.0f, LayerMask.GetMask("WallRun")))
        {
            wallRunLeft = false;
        }
        else
        {

            Debug.Log("left" + hit.transform.name);
        }
        


        if (!wallRunLeft && !wallRunRight)
        {
            m_rb.useGravity = true;
            playerWallRunForce = initialPlayerWallRunForce;
            GetComponent<CameraMove>().inSlide = false;
            m_rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        if (isGrab)
        {
            m_rb.velocity = new Vector3(0, 0, 0);
            m_rb.angularVelocity = new Vector3(0, 0, 0);
            //m_rb.isKinematic = true;
            m_rb.useGravity = false;
            GetComponent<CameraMove>().inSlide = true;
            animator.SetBool("Slide", true);
        }

        if (playerIsGrounded && transform.rotation.x != 0.0f || transform.rotation.z != 0.0f)
        {
            transform.rotation = new Quaternion(0.0f, transform.rotation.y, 0.0f, transform.rotation.w);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            isAiming = true;
            weapon.SetActive(true);
        }

        if (Input.GetButtonUp("Fire2"))
        {
            isAiming = false;
            weapon.SetActive(false);
        }

        if (Input.GetKeyDown(keySlide) && !inSlide)
        {
            canSlide = true;
        }

        if (isAiming)
        {
            CanRun = false;
        }
        else
        {
            CanRun = true;
        }

        m_xAxis = Input.GetAxisRaw("Horizontal") * factorMove;
        m_zAxis = Input.GetAxisRaw("Vertical") * factorMove;

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

        if (Input.GetKeyDown(keyJump))
        {
            if (playerIsGrounded || isGrab || wallRunLeft || wallRunRight)
            {
                canJump = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (scaled)
        {
            m_deltaTime = Time.fixedDeltaTime;
        }

        else
        {
            m_deltaTime = Time.fixedUnscaledDeltaTime;
        }

        if (CanMove)
        {
            MoveCharacter();
        }

        if (canSlide)
        {
            canSlide = false;
            GetComponent<CameraMove>().inSlide = true;
            inSlide = true;
            CanMove = false;
            Slide(playerSlideForce, appliedSlideForceMode);
        }
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
    }

    private void MoveCharacter()
    {
        m_direction = new Vector3(m_xAxis, 0f, m_zAxis).normalized;
        
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
            Vector3 force = playerForce * m_deltaTime * speed * transform.TransformDirection(m_direction);
            m_rb.AddForce(force, appliedForceMode);
            //m_rb.MovePosition(transform.position + m_deltaTime * speed * transform.TransformDirection(m_direction));
        }
        else
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
        }
        
    }

    private void Slide(float jumpForce, ForceMode forceMode)
    {
        animator.SetBool("Slide", true);
        body.transform.Rotate(new Vector3(-80.0f, 0.0f, 0.0f));
        cam.transform.Rotate(new Vector3(70.0f, 0, 0));
        m_rb.AddForce(jumpForce * m_rb.mass * m_deltaTime * transform.TransformDirection(Vector3.forward), forceMode);
        StartCoroutine(CoSlide());
    }

    IEnumerator CoSlide()
    {
        yield return new WaitForSeconds(timeOfSlide);
        body.transform.Rotate(new Vector3(80.0f, 0.0f, 0.0f));
        cam.transform.Rotate(new Vector3(-70.0f, 0, 0));
        animator.SetBool("Slide", false);
        inSlide = false;
        GetComponent<CameraMove>().inSlide = false;
        CanMove = true;
    }
    private void Jump(float jumpForce, ForceMode forceMode, Vector3 direction)
    {
        playerIsGrounded = false;
        //Debug.Log(transform.TransformDirection(Vector3.forward) * speed * m_deltaTime);
        m_rb.AddForce((jumpForce * m_rb.mass * m_deltaTime * direction), forceMode);
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
                        transform.rotation = new Quaternion(collision.gameObject.transform.rotation.x, -collision.gameObject.transform.rotation.y, collision.gameObject.transform.rotation.z, collision.gameObject.transform.rotation.w);
                    }
                    playerIsJumping = false;
                    animator.SetBool("DoJump", false);
                    factorMove = 1.0f;
                    CanMove = true;
                    GetComponent<CameraMove>().inSlide = true;
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
                        transform.rotation =  new Quaternion(collision.gameObject.transform.rotation.x, -collision.gameObject.transform.rotation.y, collision.gameObject.transform.rotation.z, collision.gameObject.transform.rotation.w);
                    }
                    
                    playerIsJumping = false;
                    animator.SetBool("DoJump", false);
                    factorMove = 1.0f;
                    CanMove = true;
                    GetComponent<CameraMove>().inSlide = true;
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
                CanMove = true;
                factorMove = 1.0f;
                animator.SetBool("DoJump", false);
                transform.parent = hit.transform.parent;
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


    private void reload()
    {
        gun.setReloading(true);
        gun.setReloadStartTime(Time.time);
        gun.setAmountOfBullets(gun.maxAmo);
        reloadMessage.text = "";
    }
}
