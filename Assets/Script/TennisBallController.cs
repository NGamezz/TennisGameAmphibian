using System.Collections;
using UnityEngine;

public class TennisBallController : MonoBehaviour
{
    [SerializeField] private float minDistanceToTarget = 5.0f;
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float distanceFromTableForPoint = 8.0f;
    [SerializeField] private float delayBetweenScores = 2.0f;

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
        tableCenter = FindObjectOfType<TableCenter>().transform;
        target = transform.position;
    }

    private IEnumerator StartBallThrow()
    {
        if (player == null) { StopAllCoroutines(); }

        player.Score += 1;
        target = transform.position;

        canScore = false;

        yield return new WaitForSeconds(delayBetweenScores);

        canScore = true;
    }

    private void Update()
    {
        if (Vector3.Distance(transform.position, tableCenter.position) > distanceFromTableForPoint && canScore)
        {
            StartCoroutine(StartBallThrow());
        }

        if (Vector3.Distance(transform.position, target) <= minDistanceToTarget || target == null) { return; }

        Vector3 direction = target - transform.position;
        direction.y = 0.0f;

        transform.Translate(10.0f * moveSpeed * Time.deltaTime * direction.normalized);
    }
}