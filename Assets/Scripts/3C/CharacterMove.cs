using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    public float speedWalk;
    public float speedRun;
    private float speed;

    public bool scaled = false;

    private Vector3 direction;

    private float deltaTime;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        speed = speedWalk;
        animator = GetComponent<Animator>();
        animator.SetBool("Walk", false);
        animator.SetBool("Run", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (scaled)
        {
            deltaTime = Time.deltaTime;
        }

        else
        {
            deltaTime = Time.unscaledDeltaTime;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            speed = speedRun;
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            speed = speedWalk;
        }

        Debug.Log(animator.GetBool("Walk"));
        MoveCharacter();
        
        
    }

    private void MoveCharacter()
    {
        direction = transform.forward * Input.GetAxisRaw("Vertical") + transform.right * Input.GetAxisRaw("Horizontal");

        direction = direction.normalized;

        
        Debug.Log(direction);

        if (direction != Vector3.zero)
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
            transform.position += direction * deltaTime * speed;
        }
        else
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
        }
        
    }

}
