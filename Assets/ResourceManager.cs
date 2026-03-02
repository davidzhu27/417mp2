using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    public int ducks = 0;
    public int bucks = 0;
    public bool duckBreedingUnlocked = false;
    public bool duckSellingUnlocked = false;
    public float ducksPerSecond = 0f;
    private float duckAccumulator = 0f;

    public System.Action OnDuckCountIncreased;
    public System.Action OnDuckCountDecreased;
    public System.Action OnBucksIncreased;
    public System.Action OnBucksDecreased;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddDucks(int amount)
    {
        ducks += amount;
        OnDuckCountIncreased?.Invoke();
    }

    public void SellDucks(int amount)
    {
        if (amount <= 0)
            return;

        ducks -= amount;
        ducks = Mathf.Max(ducks, 0);
        OnDuckCountDecreased?.Invoke();
        bucks += amount;
        OnBucksIncreased?.Invoke();
    }

    void Update()
    {
        if (!duckBreedingUnlocked || ducksPerSecond <= 0f)
            return;

        duckAccumulator += ducksPerSecond * Time.deltaTime;

        if (duckAccumulator >= 1f)
        {
            int toAdd = Mathf.FloorToInt(duckAccumulator);
            duckAccumulator -= toAdd;

            AddDucks(toAdd);
        }
    }
}