using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;

public class ResourceUIController : MonoBehaviour
{
    [Header("Text")]
    public TextMeshProUGUI duckText;
    public TextMeshProUGUI buckText;
    public TextMeshProUGUI genInfoText;

    [Header("Unlock Buttons")]
    public GameObject duckBreederButton;
    public GameObject autoSellerButton;
    public GameObject duckSellingButton;
    public GameObject generatorMultiplierButton;

    [Header("Unlock Thresholds")]
    public int autoSellerUnlockCost = 20;
    public int duckSellingUnlockCost = 40;

    [Header("Systems")]
    public ConveyorSystem conveyorSystem;

    private bool autoSellerUnlocked = false;
    private bool duckSellingUnlocked = false;

    void Start()
    {
        // Greyed-out preview state
        InitLockedButton(duckBreederButton);
        InitLockedButton(autoSellerButton);
        InitLockedButton(duckSellingButton);
        InitLockedButton(generatorMultiplierButton);

        UpdateUI();

        ResourceManager.Instance.OnDuckCountIncreased += OnResourcesChanged;
        ResourceManager.Instance.OnDuckCountDecreased += OnResourcesChanged;
        ResourceManager.Instance.OnBucksIncreased += OnResourcesChanged;
        ResourceManager.Instance.OnBucksDecreased += OnResourcesChanged;
    }

    void OnDestroy()
    {
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.OnDuckCountIncreased -= OnResourcesChanged;
            ResourceManager.Instance.OnDuckCountDecreased -= OnResourcesChanged;
            ResourceManager.Instance.OnBucksIncreased -= OnResourcesChanged;
            ResourceManager.Instance.OnBucksDecreased -= OnResourcesChanged;
    }

    // =============================
    // EVENT HANDLER
    // =============================
    void OnResourcesChanged()
    {
        UpdateUI();
        HandleUnlocks();
    }

    // =============================
    // UI UPDATE
    // =============================
    void UpdateUI()
    {
        duckText.text = $"Ducks: {ResourceManager.Instance.ducks}";
        buckText.text = $"Bucks: {ResourceManager.Instance.bucks}";
        StringBuilder sb = new StringBuilder();
        sb.AppendLine($"Ducks/sec: {ResourceManager.Instance.GetDucksPerSecond():F2}");
        sb.AppendLine($"Generators (+{ResourceManager.Instance.generatorRate}/s): {ResourceManager.Instance.generators}");
        sb.AppendLine($"Multipliers (+{ResourceManager.Instance.multiplierEffect * 100}%): {ResourceManager.Instance.multipliers}");
        genInfoText.text = sb.ToString();
    }

    // =============================
    // UNLOCK LOGIC
    // =============================
    void HandleUnlocks()
    {
        if (ResourceManager.Instance.ducks >= ResourceManager.Instance.GetNextGeneratorPrice())
        {
            UnlockButton(duckBreederButton);
        } else {
            LockButton(duckBreederButton);
        }

        if (!autoSellerUnlocked &&
            ResourceManager.Instance.bucks >= autoSellerUnlockCost)
        {
            autoSellerUnlocked = true;
            UnlockButton(autoSellerButton);
        }

        if (!duckSellingUnlocked &&
            ResourceManager.Instance.ducks >= duckSellingUnlockCost)
        {
            duckSellingUnlocked = true;
            UnlockButton(duckSellingButton);
        }

        if (ResourceManager.Instance.generators >= 1 &&
            ResourceManager.Instance.bucks >= ResourceManager.Instance.GetNextMultiplierPrice())
        {
            UnlockButton(generatorMultiplierButton);
        } else {
            LockButton(generatorMultiplierButton);
        }
    }

    void InitLockedButton(GameObject buttonObj)
    {
        if (buttonObj == null) return;

        var button = buttonObj.GetComponent<Button>();
        var image = buttonObj.GetComponent<Image>();

        button.interactable = false;
        image.color = new Color(0.6f, 0.6f, 0.6f, 0.6f);
    }

    void LockButton(GameObject buttonObj)
    {
        if (buttonObj == null) return;

        var button = buttonObj.GetComponent<Button>();
        var image = buttonObj.GetComponent<Image>();
        var text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

        button.interactable = false;
        image.color = new Color(0.6f, 0.6f, 0.6f, 0.6f);
    }

    void UnlockButton(GameObject buttonObj)
    {
        if (buttonObj == null) return;

        var button = buttonObj.GetComponent<Button>();
        var image = buttonObj.GetComponent<Image>();
        var text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

        button.interactable = true;
        image.color = Color.white;

        text.text = text.text.Replace("Unlocks", "Cost");
    }

    // =============================
    // PURCHASE BUTTON CALLBACKS
    // =============================

    public void PurchaseDuckBreeder()
    {
        if (ResourceManager.Instance.ducks < ResourceManager.Instance.GetNextGeneratorPrice()) return;

        ResourceManager.Instance.PurchaseGenerator();

        // update duckBreederButton with new price
        var text = duckBreederButton.GetComponentInChildren<TextMeshProUGUI>();
        int nextPrice = ResourceManager.Instance.GetNextGeneratorPrice();
        text.text = $"Duck Science (Cost: {nextPrice} Ducks)";
    }

    public void PurchaseAutoSeller()
    {
        if (!autoSellerUnlocked) return;
        if (ResourceManager.Instance.bucks < autoSellerUnlockCost) return;

        conveyorSystem.autoSellerActive = true;
    
        autoSellerButton.SetActive(false);
    }

    public void PurchaseDuckLicense()
    {
        if (!duckSellingUnlocked) return;
        if (ResourceManager.Instance.ducks < duckSellingUnlockCost) return;

        conveyorSystem.duckSellingUnlocked = true;

        duckSellingButton.SetActive(false);
    }

    public void PurchaseGeneratorMultiplier()
    {
        if (ResourceManager.Instance.generators < 1) return;
        if (ResourceManager.Instance.bucks < ResourceManager.Instance.GetNextMultiplierPrice()) return;

        ResourceManager.Instance.PurchaseMultiplier();

        // update generatorMultiplierButton with new price
        var text = generatorMultiplierButton.GetComponentInChildren<TextMeshProUGUI>();
        int nextPrice = ResourceManager.Instance.GetNextMultiplierPrice();
        text.text = $"Generator Multiplier (Cost: {nextPrice} Bucks)";
    }
}
