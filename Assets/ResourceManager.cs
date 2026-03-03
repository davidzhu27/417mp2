using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    public int ducks = 0;
    public int bucks = 0;
    public bool duckBreedingUnlocked = false;
    public bool duckSellingUnlocked = false;
    public int generators = 0;
    public int multipliers = 0;
    public float generatorRate = 0.2f;
    public float multiplierEffect = 0.5f;
    private float duckAccumulator = 0f;

    // Existing events (keep for compatibility with existing scripts)
    public System.Action OnDuckCountIncreased;
    public System.Action OnDuckCountDecreased;
    public System.Action OnBucksIncreased;
    public System.Action OnBucksDecreased;

    // NEW: unified "changed" events expected by SaveManager / TrophyManager / UI
    public System.Action<int> OnDuckCountChanged;
    public System.Action<int> OnBuckCountChanged;

    public float GetDucksPerSecond()
    {
        return generators * generatorRate * (1 + multipliers * multiplierEffect);
    }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void NotifyDuckChanged()
    {
        OnDuckCountChanged?.Invoke(ducks);
    }

    private void NotifyBuckChanged()
    {
        OnBuckCountChanged?.Invoke(bucks);
    }

    public void AddDucks(int amount)
    {
        ducks += amount;

        OnDuckCountIncreased?.Invoke();
        NotifyDuckChanged();
    }

    public void SellDucks(int amount)
    {
        if (amount <= 0)
            return;

        ducks -= amount;
        ducks = Mathf.Max(ducks, 0);
        OnDuckCountDecreased?.Invoke();
        NotifyDuckChanged();

        bucks += amount;
        OnBucksIncreased?.Invoke();
        NotifyBuckChanged();
    }

    public void PurchaseGenerator()
    {
        int price = GetNextGeneratorPrice();
        if (ducks < price)
            return;

        ducks -= price;
        generators++;
        OnDuckCountDecreased?.Invoke();
        NotifyDuckChanged();
    }

    public int GetNextGeneratorPrice()
    {
        return 20 * (generators + 1);
    }

    public void PurchaseMultiplier()
    {
        int price = GetNextMultiplierPrice();
        if (bucks < price)
            return;

        bucks -= price;
        multipliers++;
        OnBucksDecreased?.Invoke();
        NotifyBuckChanged();
    }

    public int GetNextMultiplierPrice()
    {
        // use an exponential price increase for multipliers
        return 200 * (int)Mathf.Pow(2, multipliers);
    }

    void Update()
    {
        if (GetDucksPerSecond() <= 0f)
            return;

        duckAccumulator += GetDucksPerSecond() * Time.deltaTime;

        if (duckAccumulator >= 1f)
        {
            int toAdd = Mathf.FloorToInt(duckAccumulator);
            duckAccumulator -= toAdd;

            AddDucks(toAdd);
        }
    }
}