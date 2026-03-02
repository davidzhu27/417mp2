using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ResourceUIController : MonoBehaviour
{
    [Header("Text")]
    public TextMeshProUGUI duckText;
    public TextMeshProUGUI buckText;

    [Header("Unlock Buttons")]
    public GameObject duckBreederButton;
    public GameObject autoSellerButton;
    public GameObject duckSellingButton;

    [Header("Unlock Thresholds")]
    public int duckBreederUnlockCost = 20;
    public int autoSellerUnlockCost = 20;
    public int duckSellingUnlockCost = 40;

    [Header("Systems")]
    public ConveyorSystem conveyorSystem;

    private bool duckBreederUnlocked = false;
    private bool autoSellerUnlocked = false;
    private bool duckSellingUnlocked = false;

    void Start()
    {
        // Greyed-out preview state
        InitLockedButton(duckBreederButton);
        InitLockedButton(autoSellerButton);
        InitLockedButton(duckSellingButton);

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
    }

    // =============================
    // UNLOCK LOGIC
    // =============================
    void HandleUnlocks()
    {
        if (!duckBreederUnlocked &&
            ResourceManager.Instance.ducks >= duckBreederUnlockCost)
        {
            duckBreederUnlocked = true;
            UnlockButton(duckBreederButton);
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
    }

    void InitLockedButton(GameObject buttonObj)
    {
        if (buttonObj == null) return;

        var button = buttonObj.GetComponent<Button>();
        var image = buttonObj.GetComponent<Image>();

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

        text.text = text.text.Replace("(Unlocks", "Cost");
    }

    // =============================
    // PURCHASE BUTTON CALLBACKS
    // =============================

    public void PurchaseDuckBreeder()
    {
        if (!duckBreederUnlocked) return;
        if (ResourceManager.Instance.ducks < duckBreederUnlockCost) return;

        ResourceManager.Instance.duckBreedingUnlocked = true;
        ResourceManager.Instance.ducksPerSecond = 0.33f; // ~1 duck / 3 sec

        duckBreederButton.SetActive(false);
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
}