using UnityEngine;

public class SpawnTatamiOnBall : MonoBehaviour
{
    public GameObject tatamiPrefab;
    
    GameObject currentTatami;

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch)) // A button
        {
            if (currentTatami != null)
            {
                Destroy(currentTatami);
            }
            
            SpawnTatami();
        }
    }

    private void SpawnTatami()
    {
        currentTatami = (GameObject)Instantiate(tatamiPrefab, gameObject.transform.position, tatamiPrefab.transform.rotation);
        currentTatami.transform.parent = gameObject.transform;
        currentTatami.transform.localScale = Vector3.one;
        
        int randomRotation = Random.Range(0, 4);

        if (randomRotation == 0)
        {
            currentTatami.transform.Rotate(currentTatami.transform.forward, 0.0f);
        }
        else if (randomRotation == 1)
        {
            currentTatami.transform.Rotate(currentTatami.transform.forward, 45);
        }
        else if (randomRotation == 2)
        {
            currentTatami.transform.Rotate(currentTatami.transform.forward, 90);
        }
        else if (randomRotation == 3)
        {
            currentTatami.transform.Rotate(currentTatami.transform.forward, 135);
        }
    }
}
