using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    [Header("Jumping State")] public bool playerIsJumping;

    [Header("Current Player Speed")] public float speed;

    [Header("Ground LayerMask int")] public int groundLayer;

    [Header("Viser")]
    public GameObject gun;
    public bool isAiming = false;

    #endregion


    #region PRIVATE FIELDS

    private float m_xAxis;
    private float m_zAxis;
    private Rigidbody m_rb;
    private int m_groundLayerMask;
    private bool m_playerIsGrounded = true;
    private Vector3 m_direction;
    private float m_deltaTime;
    private Animator m_animator;
    private bool canJump = false;
    private bool canSlide = false;
    private bool inSlide = false;
    private float initialSpeedAnimator;
    private bool CanMove = true;
    private float factorMove = 1.0f;

    private Quaternion rotationInitialBody;
    private Quaternion rotationInitialCam;
    #endregion

    void Start()
    {
        #region INITIALISATION
        m_rb = GetComponent<Rigidbody>();

        speed = speedWalk;

        m_animator = GetComponent<Animator>();
        initialSpeedAnimator = m_animator.speed;
        m_animator.SetBool("Walk", false);
        m_animator.SetBool("Run", false);

        m_groundLayerMask = groundLayer;
        #endregion
    }

    void Update()
    {
        if (m_playerIsGrounded && transform.rotation.x != 0.0f || transform.rotation.z != 0.0f)
        {
            transform.rotation = new Quaternion(0.0f, transform.rotation.y, 0.0f, transform.rotation.w);
        }
        if (Input.GetButtonDown("Fire2"))
        {
            isAiming = true;
            gun.SetActive(true);
        }

        if (Input.GetButtonUp("Fire2"))
        {
            isAiming = false;
            gun.SetActive(false);
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
        if (scaled)
        {
            m_deltaTime = Time.deltaTime;
        }
        else
        {
            m_deltaTime = Time.unscaledDeltaTime;
            m_animator.speed = initialSpeedAnimator/Time.timeScale;
        }

        m_xAxis = Input.GetAxis("Horizontal") * factorMove;
        m_zAxis = Input.GetAxis("Vertical") * factorMove;

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
                if (m_playerIsGrounded)
                {
                    m_rb.velocity = new Vector3(0, 0, 0);
                }
            }
        }

        

        if (m_playerIsGrounded && Input.GetKeyDown(keyJump))
        {
            canJump = true;
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
            canJump = false;
            //Debug.Log("Jump");
            Jump(playerJumpForce, appliedJumpForceMode);
        }
    }

    private void MoveCharacter()
    {
        m_direction = new Vector3(m_xAxis, 0f, m_zAxis);

        
        if (m_direction != Vector3.zero)
        {
            if (speed == speedWalk)
            {
                m_animator.SetBool("Walk", true);
                m_animator.SetBool("Run", false);
            }
            else
            {
                m_animator.SetBool("Walk", false);
                m_animator.SetBool("Run", true);
            }

            m_rb.AddForce(playerForce * m_deltaTime * speed * transform.TransformDirection(m_direction), appliedForceMode);
            //m_rb.MovePosition(transform.position + m_deltaTime * speed * transform.TransformDirection(m_direction));
        }
        else
        {
            m_animator.SetBool("Walk", false);
            m_animator.SetBool("Run", false);
        }
        
    }

    private void Slide(float jumpForce, ForceMode forceMode)
    {
        m_animator.SetBool("Slide", true);
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
        m_animator.SetBool("Slide", false);
        inSlide = false;
        GetComponent<CameraMove>().inSlide = false;
        CanMove = true;
    }
    private void Jump(float jumpForce, ForceMode forceMode)
    {
        m_playerIsGrounded = false;
        //Debug.Log(transform.TransformDirection(Vector3.forward) * speed * m_deltaTime);
        m_rb.AddForce((jumpForce * m_rb.mass * m_deltaTime * Vector3.up), forceMode);
        playerIsJumping = true;
        factorMove = factorMoveJump;
        m_animator.SetBool("DoJump", true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == m_groundLayerMask)
        {
            m_playerIsGrounded = true;
            playerIsJumping = false;
            CanMove = true;
            factorMove = 1.0f;
            m_animator.SetBool("DoJump", false);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.layer == m_groundLayerMask)
        {
            m_playerIsGrounded = false;
        }
    }
}
