using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabCollider : MonoBehaviour
{
    public CharacterMove player;
    public Vector3 offset;
    private bool lerp;
    private Vector3 nextPosition;

    private void Start()
    {
        offset = transform.position - player.gameObject.transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && player.isGrab)
        {
            lerp = true;
            nextPosition = player.gameObject.transform.position + player.gameObject.transform.TransformDirection(Vector3.up * 1.2f + Vector3.forward * 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && player.isGrab)
        {
            player.gameObject.GetComponent<CameraMove>().inSlide = false;
            player.animator.SetBool("Slide", false);
            player.Ungrab();
            player.playerIsGrounded = false;
            player.playerIsJumping = true;
            player.factorMove = player.factorMoveJump;
            player.animator.SetBool("DoJump", true);
        }

            if (lerp)
        {
            player.gameObject.GetComponent<CameraMove>().inSlide = false;
            player.animator.SetBool("Slide", false);
            player.gameObject.transform.position = Vector3.Lerp(player.gameObject.transform.position, nextPosition,0.2f);
            if (Vector3.Distance(player.gameObject.transform.position, nextPosition) < 0.1f)
            {
                lerp = false;
                player.Ungrab();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collision avec " + other.gameObject.name);
        if(other.gameObject.tag == "ColliderGrab" && player.playerIsJumping)
        {
            player.isGrab = true;
            player.CanMove = false;
            //other.ClosestPoint(transform.position);
            player.gameObject.transform.rotation = other.gameObject.transform.rotation;
            offset = transform.position - player.gameObject.transform.position;
            player.gameObject.transform.position = other.ClosestPoint(transform.position) - offset;
        }
    }
}
