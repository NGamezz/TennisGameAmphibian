using UnityEngine;

public class TennisBallLogic : MonoBehaviour
{
    [SerializeField] private float minDistanceToTarget = 5.0f;
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float distanceFromTableCenterForPoint = 8.0f;

    [SerializeField] private float widthOfTheTable = 12.0f;

    private Rigidbody rb;
    private Transform tableCenter;
    private bool canScore = true;
    private Vector3 target;

    private Vector3 startPosition = Vector3.zero;
    private PlayerMovement player;

    private bool isIdle = true;

    public void SetTarget(Vector3 futurePosition, PlayerMovement player)
    {
        isIdle = false;

        if (player != null)
        {
            this.player = player;
            target = futurePosition - transform.position;
            rb.velocity = Vector3.zero;
            rb.AddForce(target.normalized * moveSpeed, ForceMode.Impulse);
        }
        else
        {
            target = futurePosition;
        }

        target.y = 0.0f;
    }

    private void Start()
    {
        startPosition = transform.position;
        tableCenter = FindObjectOfType<TableCenterIdentifier>().transform;
        target = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    private void StartBallThrow()
    {
        if (player == null) { return; }

        player.Score += 1;
        target = transform.position;

        canScore = false;
        player = null;
    }

    public void ResetBall()
    {
        rb.velocity = Vector3.zero;
        isIdle = true;
        player = null;
        transform.position = startPosition;
    }

    private void Update()
    {
        if (isIdle) { return; }

        if (Vector3.Distance(transform.position, tableCenter.position) > distanceFromTableCenterForPoint && canScore)
        {
            StartBallThrow();
            rb.velocity = Vector3.zero;
        }

        if (!canScore && Vector3.Distance(transform.position, tableCenter.position) <= widthOfTheTable / 2.0f)
        {
            canScore = true;
        }

        if (Vector3.Distance(transform.position, target) <= minDistanceToTarget || target == null) { return; }

        rb.AddRelativeForce(moveSpeed * target.normalized, ForceMode.Force);
        VelocityLimiting();
    }

    private void VelocityLimiting()
    {
        Vector3 flatVelocity = new(rb.velocity.x, 0.0f, rb.velocity.z);

        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 newVelocity = flatVelocity.normalized * moveSpeed;
            rb.velocity = newVelocity;
        }
    }
}