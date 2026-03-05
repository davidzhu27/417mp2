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
    public int hlg1 = 0;
    public int hlg2 = 0;
    public int hlg3 = 0;
    public float generatorRate = 0.2f;
    public float multiplierEffect = 0.5f;
    public float hlg1Effect = 100f;
    public float hlg2Effect = 500f;
    public float hlg3Effect = 2500f;
    private float duckAccumulator = 0f;

    // Existing events (keep for compatibility with existing scripts)
    public System.Action OnDuckCountChanged;
    public System.Action OnBucksChanged;

    public float GetDucksPerSecond()
    {
        float baseRate = generators * generatorRate;
        float multiplier = 1f + (multipliers * multiplierEffect);
        float hlgBonus = (hlg1 * hlg1Effect) + (hlg2 * hlg2Effect) + (hlg3 * hlg3Effect);
        return baseRate * multiplier + hlgBonus;
    }

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

        OnDuckCountChanged?.Invoke();
    }

    public void SellDucks(int amount)
    {
        if (amount <= 0)
            return;

        ducks -= amount;
        ducks = Mathf.Max(ducks, 0);
        OnDuckCountChanged?.Invoke();

        bucks += amount;
        OnBucksChanged?.Invoke();
    }

    public void PurchaseGenerator()
    {
        int price = GetNextGeneratorPrice();
        if (ducks < price)
            return;

        ducks -= price;
        generators++;
        OnDuckCountChanged?.Invoke();
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
        OnBucksChanged?.Invoke();
    }

    public int GetNextMultiplierPrice()
    {
        // use an exponential price increase for multipliers
        return 20 * (int)Mathf.Pow(2, multipliers);
    }

    public void PurchaseHLG(int hlgNumber)
    {
        int price = GetNextHLGPrice(hlgNumber);
        if (bucks < price)
            return;
        bucks -= price;

        switch (hlgNumber)
        {
            case 1:
                hlg1++;
                OnBucksChanged?.Invoke();
                break;
            case 2:
                hlg2++;
                OnBucksChanged?.Invoke();
                break;
            case 3:
                hlg3++;
                OnBucksChanged?.Invoke();
                break;
        }
    }

    public int GetNextHLGPrice(int hlgNumber)
    {
        switch (hlgNumber)
        {
            case 1:
                return 10 * (int)Mathf.Pow(2, hlg1);
            case 2:
                return 50 * (int)Mathf.Pow(2, hlg2);
            case 3:
                return 250 * (int)Mathf.Pow(2, hlg3);
            default:
                return int.MaxValue;
        }
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
