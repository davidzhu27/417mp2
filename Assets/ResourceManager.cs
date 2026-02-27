using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    public int ducks = 0;
    public int bucks = 0;

    public System.Action OnDuckCountChanged;

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

        bucks += amount;
        OnDuckCountChanged?.Invoke();
    }
}