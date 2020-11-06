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
        if (Input.GetKeyDown(KeyCode.Z) && player.isGrab)
        {
            lerp = true;
            nextPosition = player.gameObject.transform.position + player.gameObject.transform.TransformDirection(Vector3.up * 1.3f + Vector3.forward * 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.S) && player.isGrab)
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
        if(other.gameObject.tag == "ColliderGrab" && player.playerIsJumping)
        {
            Debug.Log("collision enter avec " + other.gameObject.name);
            player.isGrab = true;
            player.CanMove = false;
            player.gameObject.transform.rotation = other.gameObject.transform.rotation;
            offset = transform.position - player.gameObject.transform.position;
            Vector3 newPosition = other.ClosestPoint(transform.position) - offset;
            player.gameObject.transform.position = newPosition;
        }
    }
}
