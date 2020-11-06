﻿using System.Collections;
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
    public bool CanRun = true;
    public bool scaled = true;

    [Header("Jump Settings")]
    public KeyCode keyJump;
    public float playerJumpForce;
    public ForceMode appliedForceMode;


    [Header("Jumping State")] public bool playerIsJumping;

    [Header("Current Player Speed")] public float speed;

    [Header("Ground LayerMask int")] public int groundLayer;

    [Header("Viser")]
    public GameObject weapon;
    public bool isAiming = false;
    private Gun gun;

    public Text reloadMessage;
    public Text bullets;

    #endregion


    #region PRIVATE FIELDS

    private float m_xAxis;
    private float m_zAxis;
    private Rigidbody m_rb;
    private bool m_leftShiftPressed;
    private int m_groundLayerMask;
    private bool m_playerIsGrounded = true;
    private Vector3 m_direction;
    private float m_deltaTime;
    private Animator m_animator;
    private bool canJump = false;
    private float initialSpeedAnimator;
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

        gun = weapon.transform.GetComponent<Gun>();
        Debug.Log(gun);
        #endregion
    }

    void Update()
    {
        
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

        if (CanRun)
        {
            speed = Input.GetKey(keyRun) ? speedRun : speedWalk;
        }
        else
        {
            speed = speedWalk;
        }

        m_xAxis = Input.GetAxis("Horizontal");
        m_zAxis = Input.GetAxis("Vertical");

        if (m_playerIsGrounded && Input.GetKeyDown(keyJump))
        {
            canJump = true;
        }

        if (gun.getReloading() && Time.time - gun.getReloadStartTime() > gun.reloadingTime)
        {
            gun.setReloading(false);
            reloadMessage.text = "";
        }
        if (gun.getReloading())
        {
            reloadMessage.text = "Reloading";
        }
        if (Input.GetKeyDown("r") && gun.getAmountOfBullets() < gun.maxAmo)
        {
            reload();
        }
        bullets.text = gun.getAmountOfBullets().ToString();
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

        MoveCharacter();

        if (canJump)
        {
            canJump = false;
            Debug.Log("Jump");
            Jump(playerJumpForce, appliedForceMode);
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

            m_rb.MovePosition(transform.position + m_deltaTime * speed * transform.TransformDirection(m_direction));
        }
        else
        {
            m_animator.SetBool("Walk", false);
            m_animator.SetBool("Run", false);
        }
        
    }

    private void Jump(float jumpForce, ForceMode forceMode)
    {
        Debug.Log(jumpForce * m_rb.mass * m_deltaTime * Vector3.up);
        m_rb.AddForce(jumpForce * m_rb.mass * m_deltaTime * Vector3.up, forceMode);
        playerIsJumping = true;
        m_animator.SetBool("DoJump", true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == m_groundLayerMask)
        {
            m_playerIsGrounded = true;
            playerIsJumping = false;
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


    private void reload()
    {
        gun.setReloading(true);
        gun.setReloadStartTime(Time.time);
        gun.setAmountOfBullets(gun.maxAmo);
        reloadMessage.text = "";
    }
}
