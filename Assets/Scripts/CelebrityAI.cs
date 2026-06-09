using UnityEngine;

public class CelebrityAI : MonoBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public float waitTime = 4f;
    public float actionTime = 4f;

    private Animator animator;

    private int currentWaypoint = 0;

    private enum State
    {
        Walk,
        Wait,
        Act
    }

    private State currentState;

    private float timer = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        currentState = State.Walk;
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        switch (currentState)
        {
            case State.Walk:
                UpdateWalk();
                break;

            case State.Wait:
                UpdateWait();
                break;

            case State.Act:
                UpdateAct();
                break;
        }
        Debug.Log("STATE: " + currentState + " POS: " + transform.position);
    }

    // ---------------- WALK ----------------
    void UpdateWalk()
    {
        Transform target = waypoints[currentWaypoint];

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        direction.Normalize();

        // movement
        transform.position += direction * moveSpeed * Time.deltaTime;

        // rotation
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5f * Time.deltaTime);
        }

        animator.SetBool("IsWalking", true);

        // reached waypoint
        if (distance < 1f)
        {
            currentState = State.Wait;
            timer = waitTime;
        }
    }

    // ---------------- WAIT ----------------
    void UpdateWait()
    {
        animator.SetBool("IsWalking", false);

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            StartAction();
        }
    }

    // ---------------- ACT ----------------
    void UpdateAct()
    {
        animator.SetBool("IsWalking", false);

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Wait until animation is actually done playing
        if (stateInfo.normalizedTime >= 1f)
        {
            currentState = State.Walk;

            currentWaypoint++;
            if (currentWaypoint >= waypoints.Length)
                currentWaypoint = 0;
        }
    }

    // ---------------- ACTION START ----------------
    void StartAction()
    {
        currentState = State.Act;
        timer = actionTime;

        int randomAction = Random.Range(0, 2);

        if (randomAction == 0)
        {
            animator.SetTrigger("Nails");
        }
        else
        {
            animator.SetTrigger("Wave");
        }
    }
}