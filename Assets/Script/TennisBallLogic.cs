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

    private PlayerMovement player;

    public void SetTarget(Vector3 futurePosition, PlayerMovement player)
    {
        target = futurePosition - transform.position;
        target.y = 0.0f;
        this.player = player;
    }

    private void Start()
    {
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

    private void Update()
    {
        if (Vector3.Distance(transform.position, tableCenter.position) >= 15.0f)
        {
            Vector3 direction = tableCenter.position - transform.position;
            direction.y = 0.0f;
            transform.Translate(direction.normalized * 0.01f);
        }

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