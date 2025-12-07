using UnityEngine;

public class DestroyedTargetPart : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Erzeuge eine kleine zufällige Kraft in eine zufällige Richtung
            Vector3 randomDirection = Random.onUnitSphere;
            float randomForce = Random.Range(1f, 3f); // Passe die Werte nach Bedarf an
            rb.AddForce(randomDirection * randomForce, ForceMode.Impulse);

            // Optional: Füge auch ein zufälliges Drehmoment hinzu
            Vector3 randomTorque = Random.onUnitSphere * Random.Range(1f, 3f);
            rb.AddTorque(randomTorque, ForceMode.Impulse);
        }
    }
}
