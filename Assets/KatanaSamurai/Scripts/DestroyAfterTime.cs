using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DestroyAfterTime : MonoBehaviour
{
    public float m_TimeToDestroy = 5.0f;

    public void Initialize(float timeToDie = 5)
    {
        m_TimeToDestroy = timeToDie;

        StartCoroutine(WaitToDie());
    }

    IEnumerator WaitToDie()
    {
        yield return new WaitForSeconds(m_TimeToDestroy);
        Destroy(gameObject);
    }
}
