using UnityEngine;

public class DestroyedTarget : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke("removetarget", 1.5f);
    }

    private void removetarget() {
        Destroy(gameObject);
    }
}
