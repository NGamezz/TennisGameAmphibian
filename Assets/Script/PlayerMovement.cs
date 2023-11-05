using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(LineRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private float maxDistanceToCentreTable = 150.0f;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform orientationHolder;
    [SerializeField] private float aimSensitivity = 50.0f;
    [SerializeField] private float lineLenght = 15.0f;
    [SerializeField] private float whackRadius = 15.0f;
    [SerializeField] private float timeUntilAimResets = 5.0f;
    [SerializeField] private float moveSpeedOfObstacle = 2.0f;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Slider obstacleCooldownBar;

    private LineRenderer lineRenderer;

    private float score = 0.0f;
    public float Score
    {
        get
        {
            return score;
        }
        set
        {
            score = value;
            scoreText.text = $"Your Current Score = {value}.";
        }
    }

    private Vector2 movementInput = Vector2.zero;

    private Vector2 aimInput = Vector2.zero;

    [SerializeField] private bool isAiming = false;

    private Rigidbody rb;

    private GameObject currentObstacle;

    private Transform tableCenter;
    private Vector3 directionToTable;
    private Vector3 aimPoint;

    [SerializeField] private float delayUntilObstaclePlacement = 15.0f;
    private float counterUntilObstaclePlacement = 0.0f;
    private bool canHoldObstacle = false;
    private Rigidbody currentObstacleRb = null;

    public bool GameStarted = false;

    [SerializeField] private float fireDelay = 1.0f;
    private bool canFire = false;
    private float timer = 0.0f;
    private float yRotation = 0.0f;

    private bool canPlace = true;

    private void Start()
    {
        tableCenter = FindObjectOfType<TableCenterIdentifier>().transform;

        Vector3 tablePosition = tableCenter.position;
        tablePosition.y = 0.0f;

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;

        counterUntilObstaclePlacement = 0.0f;
        obstacleCooldownBar.maxValue = delayUntilObstaclePlacement;
        obstacleCooldownBar.minValue = 0.0f;
        obstacleCooldownBar.value = obstacleCooldownBar.minValue;

        directionToTable = tableCenter.position - transform.position;
        directionToTable.y = 0.0f;
        aimPoint = directionToTable.normalized * lineLenght;

        lineRenderer.SetPosition(0, orientation.position);
        lineRenderer.SetPosition(1, aimPoint);
        lineRenderer.enabled = true;

        rb = GetComponent<Rigidbody>();
        timer = timeUntilAimResets;

        StartCoroutine(ResetCanFire());
    }

    private void Update()
    {
        if (GameManager.Instance == null || !GameStarted) { return; }

        PickupHandling();
        AimUpdate();
    }

    //It's for the delay between presses.
    private IEnumerator ResetDelayForPlacement()
    {
        yield return new WaitForSeconds(1.0f);

        canPlace = true;
    }

    public void HoldObstacle()
    {
        if (GameManager.Instance == null || !GameStarted) { return; }

        if (HoldingObstacle() && canPlace)
        {
            PlacePickup();
        }
        else if (!HoldingObstacle() && canHoldObstacle && canPlace)
        {
            currentObstacle = Instantiate(GameManager.Instance.possibleObstacles[Random.Range(0, GameManager.Instance.possibleObstacles.Count)], GameManager.Instance.transform);
            canHoldObstacle = false;

            if (!currentObstacle.TryGetComponent(out Rigidbody rb))
            {
                currentObstacle = null;
                return;
            }

            currentObstacleRb = rb;
            rb.useGravity = false;
            rb.GetComponent<Collider>().enabled = false;
        }

        canPlace = false;
        StartCoroutine(ResetDelayForPlacement());
    }

    private void AimUpdate()
    {
        orientationHolder.LookAt(tableCenter);

        if (isAiming)
        {
            yRotation += aimInput.x * aimSensitivity * Time.deltaTime;
            yRotation = Mathf.Clamp(yRotation, -45, 45);

            orientation.localRotation = Quaternion.Euler(0.0f, yRotation, 0.0f);

            Vector3 newDirection = orientation.forward;
            newDirection.y = 0.0f;

            aimPoint = newDirection.normalized * lineLenght;
        }
        else
        {
            if (timer <= 0)
            {
                yRotation = 0.0f;
                timer = timeUntilAimResets;

                directionToTable = tableCenter.position - transform.position;
                directionToTable.y = 0.0f;
                aimPoint = directionToTable.normalized * lineLenght;

                orientation.LookAt(directionToTable.normalized * lineLenght);
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }

        if (Vector3.Distance(transform.position, tableCenter.position) > maxDistanceToCentreTable)
        {
            Vector3 direction = tableCenter.position - transform.position;
            direction.y = 0.0f;
            transform.Translate(direction.normalized * 0.01f);
        }

        lineRenderer.SetPosition(0, orientation.position);
        lineRenderer.SetPosition(1, aimPoint);
    }

    public void WhackTennisBall()
    {
        if (!canFire || !GameStarted) { return; }

        canFire = false;
        Collider[] hits = Physics.OverlapSphere(transform.position, whackRadius);

        foreach (Collider hit in hits)
        {
            if (hit.transform.TryGetComponent(out TennisBallLogic ballController))
            {
                ballController.SetTarget(aimPoint, this);
            }
        }

        StartCoroutine(ResetCanFire());
    }

    private IEnumerator ResetCanFire()
    {
        yield return new WaitForSeconds(fireDelay);

        canFire = true;
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, tableCenter.position) > maxDistanceToCentreTable || HoldingObstacle() || !GameStarted) { return; }

        rb.AddRelativeForce(movementSpeed * new Vector3(movementInput.x, 0.0f, movementInput.y), ForceMode.Force);
    }

    public void OnMove(InputAction.CallbackContext contex) => movementInput = contex.ReadValue<Vector2>();

    public void OnLook(InputAction.CallbackContext context)
    {
        aimInput = context.ReadValue<Vector2>();

        if (aimInput.x != 0)
        {
            isAiming = true;
        }
        else
        {
            isAiming = false;
        }
    }

    private bool HoldingObstacle()
    {
        return currentObstacle != null;
    }

    private void PickupHandling()
    {
        if (!HoldingObstacle() && !canHoldObstacle)
        {
            if (counterUntilObstaclePlacement >= delayUntilObstaclePlacement)
            {
                canHoldObstacle = true;
                counterUntilObstaclePlacement = 0.0f;
                obstacleCooldownBar.value = obstacleCooldownBar.maxValue;
            }
            else
            {
                counterUntilObstaclePlacement += Time.deltaTime;
                obstacleCooldownBar.value = counterUntilObstaclePlacement;
            }
        }

        if (!HoldingObstacle()) { return; }

        currentObstacleRb.AddRelativeForce(moveSpeedOfObstacle * new Vector3(movementInput.x, 0.0f, movementInput.y), ForceMode.Force);
    }

    public void PlacePickup()
    {
        if (!HoldingObstacle()) { return; }

        currentObstacleRb.useGravity = true;
        currentObstacleRb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        currentObstacle.GetComponent<Collider>().enabled = true;

        currentObstacle = null;
    }
}