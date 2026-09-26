using UnityEngine;

public class CarBehaviour : MonoBehaviour
{
    public ReticleBehaviour Reticle;
    public float Speed = 1.2f;

    private Vector3 targetPosition;
    private bool hasTarget;

    public void SetTargetPosition(Vector3 position)
    {
        targetPosition = position;
        hasTarget = true;
    }

    private void Update()
    {
        if (!hasTarget)
            return;

        if (Vector3.Distance(targetPosition, transform.position) < 0.1f)
            return;

        Vector3 direction = targetPosition - transform.position;

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                lookRotation,
                Time.deltaTime * 10f
            );
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            Speed * Time.deltaTime
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        PackageBehaviour package = other.GetComponent<PackageBehaviour>();

        if (package != null)
        {
            Destroy(other.gameObject);
        }
    }
}
