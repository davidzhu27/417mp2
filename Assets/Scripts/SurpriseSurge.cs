using System.Collections;
using UnityEngine;
using TMPro;

public class SurpriseSurge : MonoBehaviour
{
    [Header("Power-up Settings")]
    public GameObject surgeObject;
    public float minSpawnTime = 1f;
    public float maxSpawnTime = 1f;
    public float visibleDuration = 5f;
    public float bonusDuration = 10f;
    public float multiplier = 2f;

    [Header("UI")]
    public TMP_Text surgeText;

    private bool surgeActive = false;
    private bool bonusRunning = false;
    private float originalGeneratorRate;

    void Start()
    {
        Debug.Log("SurpriseSurge Start running");

        if (surgeObject != null)
        {
            surgeObject.SetActive(false);
            Debug.Log("Surge object hidden at start");
        }

        if (surgeText != null)
            surgeText.text = "";

        if (ResourceManager.Instance != null)
            originalGeneratorRate = ResourceManager.Instance.generatorRate;

        StartCoroutine(SurgeRoutine());
    }

    IEnumerator SurgeRoutine()
    {
        while (true)
        {
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            Debug.Log("Waiting " + waitTime + " seconds before spawning surge");
            yield return new WaitForSeconds(waitTime);

            if (surgeObject == null || bonusRunning)
                continue;

            surgeActive = true;
            surgeObject.SetActive(true);
            Debug.Log("Surge object spawned");

            if (surgeText != null)
                surgeText.text = "Quick! Bonus available!";

            yield return new WaitForSeconds(visibleDuration);

            if (surgeActive)
            {
                surgeActive = false;
                surgeObject.SetActive(false);
                Debug.Log("Surge expired before player clicked it");

                if (surgeText != null)
                    surgeText.text = "";
            }
        }
    }

    public void ActivateSurge()
    {
        Debug.Log("ActivateSurge called");

        if (!surgeActive || bonusRunning || ResourceManager.Instance == null)
            return;

        surgeActive = false;
        surgeObject.SetActive(false);
        StartCoroutine(BonusRoutine());
    }

    IEnumerator BonusRoutine()
    {
        bonusRunning = true;

        float previousRate = ResourceManager.Instance.generatorRate;
        ResourceManager.Instance.generatorRate = previousRate * multiplier;

        Debug.Log("Bonus active: generator rate doubled");

        if (surgeText != null)
            surgeText.text = "Bonus active! 2x Ducks";

        yield return new WaitForSeconds(bonusDuration);

        ResourceManager.Instance.generatorRate = previousRate;

        if (surgeText != null)
            surgeText.text = "Bonus ended";

        Debug.Log("Bonus ended");

        yield return new WaitForSeconds(2f);

        if (surgeText != null)
            surgeText.text = "";

        bonusRunning = false;
    }
}