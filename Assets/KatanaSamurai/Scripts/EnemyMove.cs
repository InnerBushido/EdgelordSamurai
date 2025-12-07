using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public Vector3 m_MovementDirection = Vector3.back;
    public float m_MovementSpeed = 0.5f;

    private void Update()
    {
        transform.position = transform.position + (m_MovementDirection * m_MovementSpeed * Time.deltaTime);

        if(transform.position.z < -2)
        {
            Destroy(gameObject);
        }
    }
}
