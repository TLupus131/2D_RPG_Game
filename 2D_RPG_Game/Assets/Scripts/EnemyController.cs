using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject player;
    public float moveSpeed;
    public float distanceBetween;
    public LayerMask solidObjectsLayer;

    private float distance;

    private void Start()
    {
        
    }

    private void Update()
    {
        distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();

        if (distance < distanceBetween && IsWalkable(transform.position)) 
        {
            transform.position = Vector2.MoveTowards(this.transform.position, player.transform.position, moveSpeed*Time.deltaTime);
        }
        else if (distance <= 1)  
        {

        }
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        RaycastHit2D hit = Physics2D.Linecast(targetPos, player.transform.position, solidObjectsLayer);
        if (hit.collider != null)
        {
            return false;
        }
        return true;
    }

}
