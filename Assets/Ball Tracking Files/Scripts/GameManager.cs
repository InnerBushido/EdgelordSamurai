using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    private int score = 0;

    [SerializeField] private List<Target> targets;
    private Target currentTarget;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetGame();
    }

    // Aktualisiere die Score-Anzeige nach dem Erhöhen des Scores
    public void shot()
    {
        currentTarget = null;
        score++;
        scoreText.text = $"Score: {score.ToString()}";
        Invoke("TargetHit", 1.6f); // Warte 0.5 Sekunden, bevor ein neues Target aktiviert wird
    }

    public void ResetGame()
    {
        // Setze den Score zurück
        score = 0;
        scoreText.text = $"Score: {score.ToString()}";
        // Deaktiviere alle Targets
        foreach (var target in targets)
        {
            target.gameObject.SetActive(false);
        }
        // Wähle ein zufälliges Target aus der Liste aus und aktiviere es
        int randomIndex = Random.Range(0, targets.Count);
        currentTarget = targets[randomIndex];
        currentTarget.gameObject.SetActive(true);
    }

    public void TargetHit()
    {
        // Wähle ein zufälliges Target aus der Liste aus und aktiviere es
        int randomIndex = Random.Range(0, targets.Count);
        currentTarget = targets[randomIndex];
        currentTarget.gameObject.SetActive(true);
    }
}
