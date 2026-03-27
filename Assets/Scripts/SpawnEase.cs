using System.Collections;
using UnityEngine;

public class SpawnEase : MonoBehaviour
{
    public float duration = 0.35f;
    public float overshootScale = 1.15f;

    private Vector3 targetScale;

    public void Play()
    {
        StopAllCoroutines();
        targetScale = transform.localScale;
        transform.localScale = Vector3.zero;
        StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        float first = duration * 0.6f;
        float second = duration * 0.4f;
        float t = 0f;

        Vector3 overshoot = targetScale * overshootScale;

        while (t < first)
        {
            t += Time.deltaTime;
            float k = t / first;
            transform.localScale = Vector3.Lerp(Vector3.zero, overshoot, k);
            yield return null;
        }

        transform.localScale = overshoot;
        t = 0f;

        while (t < second)
        {
            t += Time.deltaTime;
            float k = t / second;
            transform.localScale = Vector3.Lerp(overshoot, targetScale, k);
            yield return null;
        }

        transform.localScale = targetScale;
    }
}