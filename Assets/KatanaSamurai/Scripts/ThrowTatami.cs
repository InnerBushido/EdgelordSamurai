using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ThrowTatami : MonoBehaviour {

    public GameObject tatamiPrefab;
    // public Animation charAnimation;
    // public AnimationClip idleAnim;
    // public AnimationClip attackAnim;
    // public AnimationClip throwAnim;

    public Image senseiImage;
    public Sprite senseiSpriteIdle;
    public Sprite senseiSpriteThrow;

    public int upForce = 60;
    public int zForce = 140;

    GameObject currentTatami;

    void Start()
    {
        StartCoroutine(StartThrowing());
        
        senseiImage.sprite = senseiSpriteIdle;
    }

    IEnumerator StartThrowing()
    {
        currentTatami = (GameObject)Instantiate(tatamiPrefab, gameObject.transform.position, tatamiPrefab.transform.rotation);
        currentTatami.transform.parent = gameObject.transform;
        yield return new WaitForSeconds(1);

        StartCoroutine(KeepThrowing());
    }

    IEnumerator KeepThrowing()
    {
        StartCoroutine(ThrowOnce());
        yield return new WaitForSeconds(Random.Range(1f, 4f));
        StartCoroutine(KeepThrowing());
    }

    IEnumerator ThrowOnce()
    {
        Debug.Log("Throw!");
        // charAnimation.clip = throwAnim;
        // charAnimation.Play();
        // charAnimation.wrapMode = WrapMode.PingPong;
        
        senseiImage.sprite = senseiSpriteThrow;

        GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(0.25f);

        currentTatami.transform.parent = null;
        currentTatami.GetComponent<Rigidbody>().isKinematic = false;
        currentTatami.GetComponent<Rigidbody>().maxAngularVelocity = 20;

        currentTatami.transform.eulerAngles = Vector3.up;

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

        float randomY = Random.Range(-0.1f, 0.1f); //Random.Range(-0.15f, 0.1f);
        float randomX = Random.Range(-0.09f, 0.09f);  //Random.Range(-0.1f, 0.1f);

        currentTatami.GetComponent<Rigidbody>().AddForce((transform.forward + new Vector3(randomX, randomY, 0)) * zForce);
        //currentTatami.GetComponent<Rigidbody>().AddForce((Vector3.forward) * 1100);
        currentTatami.GetComponent<Rigidbody>().AddForce(Vector3.up * upForce);
        // currentTatami.GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * Random.Range(2f, 10f));

        yield return new WaitForSeconds(0.75f);
        // charAnimation.clip = idleAnim;
        // charAnimation.Play();
        // charAnimation.wrapMode = WrapMode.Loop;
        
        senseiImage.sprite = senseiSpriteIdle;

        //currentTatami = Instantiate(tatamiPrefab);
        currentTatami = (GameObject)Instantiate(tatamiPrefab, gameObject.transform.position, tatamiPrefab.transform.rotation);
        currentTatami.transform.parent = gameObject.transform;
    }

}
