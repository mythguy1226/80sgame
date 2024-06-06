
using UnityEngine;

public class EndAndDie : MonoBehaviour
{
    public float radius = 0.0f;
    public void Remove()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, radius);
    }
}
