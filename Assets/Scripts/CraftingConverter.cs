using UnityEngine;

public class CraftingConverter : MonoBehaviour
{
    public int duckCost = 10;
    public int buckReward = 1;
    public AudioSource successSound;

    public void ConvertDucksToBucks()
    {
        if (ResourceManager.Instance == null)
            return;

        if (ResourceManager.Instance.ducks < duckCost)
        {
            Debug.Log("Not enough Ducks.");
            return;
        }

        ResourceManager.Instance.ducks -= duckCost;
        ResourceManager.Instance.bucks += buckReward;

        ResourceManager.Instance.OnDuckCountChanged?.Invoke();
        ResourceManager.Instance.OnBucksChanged?.Invoke();

        if (successSound != null)
            successSound.Play();

        Debug.Log("Converted 10 Ducks into 1 Buck.");
    }
}