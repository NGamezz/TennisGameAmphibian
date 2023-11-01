using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody)), RequireComponent(typeof(LineRenderer))]
public class PlayerMovement : MonoBehaviour
{
    public bool CurrentTurn;

    [SerializeField] private float movementSpeed = 5.0f;
    [SerializeField] private float maxDistanceToCentreTable = 150.0f;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform orientationHolder;
    [SerializeField] private float aimSensitivity = 50.0f;
    [SerializeField] private float lineLenght = 15.0f;
    [SerializeField] private float whackRadius = 15.0f;
    [SerializeField] private float timeUntilAimResets = 5.0f;

    private LineRenderer lineRenderer;

    public float Score = 0.0f;

    private Vector2 movementInput = Vector2.zero;

    private Vector2 aimInput = Vector2.zero;

    [SerializeField] private bool isAiming = false;

    private Rigidbody rb;

    private Transform tableCenter;
    private Vector3 directionToTable;
    private Vector3 aimPoint;

    [SerializeField] private float fireDelay = 1.0f;
    private bool canFire = true;
    private float timer = 0.0f;
    private float yRotation = 0.0f;

    private void Start()
    {
        tableCenter = FindObjectOfType<TableCenter>().transform;

        Vector3 tablePosition = tableCenter.position;
        tablePosition.y = 0.0f;

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;

        directionToTable = tableCenter.position - transform.position;
        directionToTable.y = 0.0f;
        aimPoint = directionToTable.normalized * lineLenght;

        lineRenderer.SetPosition(0, orientation.position);
        lineRenderer.SetPosition(1, aimPoint);
        lineRenderer.enabled = true;

        rb = GetComponent<Rigidbody>();
        timer = timeUntilAimResets;
    }

    private void Update()
    {
        if (GameManager.Instance == null) { return; }

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
        if (!CurrentTurn || !canFire) { return; }

        canFire = false;
        Collider[] hits = Physics.OverlapSphere(transform.position, whackRadius);

        foreach (Collider hit in hits)
        {
            if (hit.transform.TryGetComponent(out TennisBallController ballController))
            {
                ballController.SetTarget(aimPoint, this);
            }
        }

        StartCoroutine(ResetCanFire());
        GameManager.Instance.ChangeTurn();
    }

    private IEnumerator ResetCanFire()
    {
        yield return new WaitForSeconds(fireDelay);

        canFire = true;
    }

    private void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, tableCenter.position) > maxDistanceToCentreTable) { return; }

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
}