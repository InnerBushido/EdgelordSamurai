using UnityEngine;

public class Mover : MonoBehaviour
{
    [Tooltip("The object that should follow this one.")]
    public Transform target = null;

    private Vector3 initialOffset;

    public void Init()
    {
        if (target == null)
        {
            Debug.LogWarning("Mover: Target is not assigned.");
            return;
        }

        // Calculate initial local offset in the Mover's space
        initialOffset = target.position - transform.position;
    }

    void Update()
    {
        if (target == null) return;

        // Maintain initial offset
        target.position = transform.position + initialOffset;

        // Match rotation exactly
        target.rotation = transform.rotation;
    }
}
