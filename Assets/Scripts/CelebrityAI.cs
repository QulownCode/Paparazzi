using UnityEngine;

public class CelebrityAI : MonoBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public float waitTime = 2f;
    public float actionTime = 3f;

    private Animator animator;

    private int currentWaypoint = 0;

    private bool waiting = false;
    private bool performingAction = false;

    private float waitTimer = 0f;
    private float actionTimer = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        // ACTION STATE
        if (performingAction)
        {
            actionTimer -= Time.deltaTime;

            animator.SetBool("IsWalking", false);

            if (actionTimer <= 0f)
            {
                performingAction = false;
            }

            return;
        }

        // WAITING STATE
        if (waiting)
        {
            waitTimer -= Time.deltaTime;

            animator.SetBool("IsWalking", false);

            if (waitTimer <= 0f)
            {
                waiting = false;

                int randomAction = Random.Range(1, 3);

                performingAction = true;
                actionTimer = actionTime;

                if (randomAction == 1)
                {
                    animator.Play("Wave");
                }
                else
                {
                    animator.Play("Pose 1");
                }
            }

            return;
        }

        // MOVEMENT
        Transform target = waypoints[currentWaypoint];

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        direction.Normalize();

        transform.position += direction * moveSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                lookRotation,
                5f * Time.deltaTime
            );
        }

        animator.SetBool("IsWalking", true);

        if (distance < 1f)
        {
            currentWaypoint++;

            if (currentWaypoint >= waypoints.Length)
            {
                currentWaypoint = 0;
            }

            waiting = true;
            waitTimer = waitTime;
        }
    }
}