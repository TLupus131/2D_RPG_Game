// EnemyController.cs
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    public GameObject player;
    public float moveSpeed;
    public float distanceBetween;
    public LayerMask solidObjectsLayer;

    private Animator animator;
    private bool isMoving;
    private bool loadSceneRequested = false;
    private float distance;
    private bool isAttacking = false;
    private Vector2 initialPosition;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        initialPosition = transform.position;
    }

    private void Update()
    {
        distance = Vector2.Distance(transform.position, player.transform.position);
        Vector2 direction = player.transform.position - transform.position;
        direction.Normalize();

        if (distance < distanceBetween && IsWalkable(player.transform.position, transform.position))
        {
            animator.SetFloat("MoveX", player.transform.position.x - transform.position.x);
            animator.SetFloat("MoveY", player.transform.position.y - transform.position.y);
            isMoving = true;
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
        }
        else
        {
            isMoving = false;
        }
        if (distance < 1 && !loadSceneRequested && !isAttacking)
        {
            GameManager.Instance.RemoveObjectAtPosition(initialPosition);
            GameManager.Instance.UpdatePlayerPosition(player.transform.position);
            loadSceneRequested = true;
            StartCoroutine(LoadSceneAsync("rpg"));
            gameObject.SetActive(false);
        }

        animator.SetBool("IsMoving", isMoving);
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    private bool IsWalkable(Vector3 start, Vector3 targetPos)
    {
        RaycastHit2D hit = Physics2D.Linecast(start, targetPos, solidObjectsLayer);
        return hit.collider == null;
    }
}
