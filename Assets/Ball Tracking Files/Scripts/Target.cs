using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private GameObject destroyedTargetPrefab;
    [SerializeField] private GameManager gameManager;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            gameObject.SetActive(false);
            Instantiate(destroyedTargetPrefab, transform.position, transform.rotation);
            gameManager.shot();
        }

    }
}
