using UnityEngine;

public class DuckBreederPurchase : MonoBehaviour
{
    public int cost = 20;

    public void PurchaseDuckBreeder()
    {
        if (ResourceManager.Instance.ducks < cost || ResourceManager.Instance.duckBreedingUnlocked)
            return;

        ResourceManager.Instance.duckBreedingUnlocked = true;
        ResourceManager.Instance.ducksPerSecond = 0.33f;
        ResourceManager.Instance.ducks -= cost;
        gameObject.SetActive(false);
    }
}