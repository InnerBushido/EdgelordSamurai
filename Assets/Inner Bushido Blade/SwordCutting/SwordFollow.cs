using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordFollow : MonoBehaviour
{
    public Transform m_SwordToFollow;

    private Rigidbody m_rigidbody;

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        m_rigidbody.MovePosition(m_SwordToFollow.position);
        m_rigidbody.MoveRotation(m_SwordToFollow.rotation);
    }

}
