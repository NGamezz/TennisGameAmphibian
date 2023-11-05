using UnityEngine;

public class TennisBallLogic : MonoBehaviour
{
    [SerializeField] private float minDistanceToTarget = 5.0f;
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float distanceFromTableCenterForPoint = 8.0f;

    [SerializeField] private float widthOfTheTable = 12.0f;

    private Transform tableCenter;
    private bool canScore = true;
    private Vector3 target;

    private PlayerMovement player;

    public void SetTarget(Vector3 futurePosition, PlayerMovement player)
    {
        target = futurePosition;
        this.player = player;
    }

    private void Start()
    {
        tableCenter = FindObjectOfType<TableCenterIdentifier>().transform;
        target = transform.position;
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
        if (Vector3.Distance(transform.position, tableCenter.position) > distanceFromTableCenterForPoint && canScore)
        {
            StartBallThrow();
        }

        if (!canScore && Vector3.Distance(transform.position, tableCenter.position) <= widthOfTheTable / 2.0f)
        {
            canScore = true;
        }

        if (Vector3.Distance(transform.position, target) <= minDistanceToTarget || target == null) { return; }

        Vector3 direction = target - transform.position;
        direction.y = 0.0f;

        transform.Translate(10.0f * moveSpeed * Time.deltaTime * direction.normalized);
    }
}