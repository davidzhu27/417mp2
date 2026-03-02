using UnityEngine;

public class DuckBreeder : MonoBehaviour
{
    public bool isActive = false;
    public float ducksPerSecond = 0.33f; // ~1 duck every 3 seconds

    private float accumulator = 0f;

    void Update()
    {
        if (!isActive) return;

        accumulator += ducksPerSecond * Time.deltaTime;

        if (accumulator >= 1f)
        {
            int toAdd = Mathf.FloorToInt(accumulator);
            accumulator -= toAdd;

            ResourceManager.Instance.AddDucks(toAdd);
        }
    }
}