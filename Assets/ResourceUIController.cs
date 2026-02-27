using UnityEngine;
using TMPro;

public class ResourceUIController : MonoBehaviour
{
    public TextMeshProUGUI duckText;
    public TextMeshProUGUI buckText;

    void Start()
    {
        UpdateUI();

        // Subscribe to changes
        ResourceManager.Instance.OnDuckCountChanged += UpdateUI;
    }

    void OnDestroy()
    {
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.OnDuckCountChanged -= UpdateUI;
    }

    void UpdateUI()
    {
        duckText.text = $"🦆 Ducks: {ResourceManager.Instance.ducks}";
        buckText.text = $"💰 Bucks: {ResourceManager.Instance.bucks}";
    }
}