using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController_Dungeon : MonoBehaviour
{
    public float moveSpeed;

    private bool isMoving;

    private Vector2 input;

    private Animator animator;

    public LayerMask solidObjectsLayer;

    public LayerMask interactableLayer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input != Vector2.zero)
            {
                animator.SetFloat("MoveX", input.x);
                animator.SetFloat("MoveY", input.y);
                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if (IsWalkable(targetPos))
                {
                    StartCoroutine(Move(targetPos));
                }
            }
        }

        animator.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Interact();
        }

    }
    void Interact()
    {
        var facingDir = new Vector3(animator.GetFloat("MoveX"), animator.GetFloat("MoveY"));
        var interactPos = transform.position + facingDir;
        var collider = Physics2D.OverlapCircle(interactPos, 0.2f, interactableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
        }
    }

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        Vector2 boxColliderSize = GetComponent<BoxCollider2D>().size;
        Collider2D hitCollider = Physics2D.OverlapBox(targetPos, boxColliderSize, 0.5f, solidObjectsLayer | interactableLayer);

        if (hitCollider != null)
        {
            return false;
        }
        return true;
    }

}

