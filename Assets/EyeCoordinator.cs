using UnityEngine;
using System.Collections;

public class EyeCoordinator : MonoBehaviour
{
    [Header("Eye References")]
    [SerializeField] private EyeController leftEye;
    [SerializeField] private EyeController rightEye;

    [Header("Blink Settings")]
    [SerializeField] private float minBlinkInterval = 2.0f;
    [SerializeField] private float maxBlinkInterval = 6.0f;

    private Coroutine surpriseCoroutine;

    void Start()
    {
        // Start the infinite blink loop
        StartCoroutine(BlinkLogic());
    }

    private IEnumerator BlinkLogic()
    {
        while (true)
        {
            // Wait for a random amount of time between blinks
            float waitTime = Random.Range(minBlinkInterval, maxBlinkInterval);
            yield return new WaitForSeconds(waitTime);

            Debug.Log("Triggering blink on both eyes");
            // Trigger blinks on both controllers
            if (leftEye != null) StartCoroutine(leftEye.BlinkRoutine());
            if (rightEye != null) StartCoroutine(rightEye.BlinkRoutine());
        }
    }

    public void SurprisedEyes()
    {
        StartCoroutine(leftEye.WidenEyes());
        StartCoroutine(rightEye.WidenEyes());
    }
}
