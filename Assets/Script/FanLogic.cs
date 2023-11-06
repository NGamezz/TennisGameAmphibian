using UnityEngine;

public class FanLogic : MonoBehaviour, IObstacle
{
    [SerializeField] private int tennisBallLayer;
    [SerializeField] private float pushMagnitude;
    [SerializeField] private Collider mainCollider;

    public void BeingHeld()
    {
        //mainCollider.enabled = false;
    }

    public void Place()
    {
        //mainCollider.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == tennisBallLayer)
        {
            if (other.gameObject.TryGetComponent(out TennisBallLogic tennisBall))
            {
                Vector3 direction = transform.forward;
                direction.y = 0.0f;

                tennisBall.SetTarget(-direction, null);
            }
        }
    }
}
