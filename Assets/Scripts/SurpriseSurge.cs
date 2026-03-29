using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class SurpriseSurge : MonoBehaviour
{
    public enum RewardType
    {
        Ducks,
        Bucks,
        DoubleDucks
    }

    [Header("Power-up Settings")]
    public GameObject surgeObject;

    public float firstSpawnDelay = 10f;
    public float minSpawnTime = 15f;
    public float maxSpawnTime = 25f;

    [Header("Reward Values")]
    public int duckRewardAmount = 20;
    public int buckRewardAmount = 10;
    public float bonusDuration = 10f;
    public float multiplier = 2f;

    [Header("UI Text")]
    public TMP_Text surgeText;          // 普通提示文字
    public TMP_Text surgeWorldText;     // 盒子上/旁边文字
    public TMP_Text rewardPopupText;    // 屏幕中间的大字提示

    [Header("Rotation")]
    public float rotationSpeed = 60f;

    [Header("Click Feedback")]
    public float clickAnimDuration = 0.2f;
    public float clickAnimScale = 1.2f;

    private bool surgeActive = false;
    private bool bonusRunning = false;
    private bool isAnimating = false;

    private Vector3 originalScale;
    private RewardType currentRewardType;

    void Start()
    {
        Debug.Log("SurpriseSurge Start running on " + gameObject.name);

        if (surgeObject != null)
        {
            surgeObject.SetActive(false);
            originalScale = surgeObject.transform.localScale;
        }

        if (surgeText != null)
            surgeText.text = "";

        if (surgeWorldText != null)
            surgeWorldText.text = "";

        if (rewardPopupText != null)
            rewardPopupText.text = "";

        StartCoroutine(SurgeRoutine());
    }

    void Update()
    {
        if (surgeObject != null && surgeObject.activeSelf && !isAnimating)
        {
            surgeObject.transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f);
        }

        // Desktop testing: press G when surge is active
        if (surgeActive && !bonusRunning && Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
        {
            Debug.Log("Desktop trigger key pressed: G");
            ActivateSurge();
        }
    }

    IEnumerator SurgeRoutine()
    {
        yield return new WaitForSeconds(firstSpawnDelay);

        while (true)
        {
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            if (surgeObject == null || bonusRunning || surgeActive)
                continue;

            surgeActive = true;
            surgeObject.SetActive(true);
            surgeObject.transform.localScale = originalScale;

            currentRewardType = GetRandomRewardType();

            if (surgeText != null)
                surgeText.text = "Press G to claim a random reward";

            if (surgeWorldText != null)
                surgeWorldText.text = GetWorldTextForReward(currentRewardType);

            Debug.Log("Surge object spawned with reward: " + currentRewardType);
        }
    }

    RewardType GetRandomRewardType()
    {
        int roll = Random.Range(0, 3);

        switch (roll)
        {
            case 0:
                return RewardType.Ducks;
            case 1:
                return RewardType.Bucks;
            default:
                return RewardType.DoubleDucks;
        }
    }

    string GetWorldTextForReward(RewardType rewardType)
    {
        switch (rewardType)
        {
            case RewardType.Ducks:
                return "+20 Ducks";
            case RewardType.Bucks:
                return "+10 Bucks";
            case RewardType.DoubleDucks:
                return "2x Ducks";
            default:
                return "Reward";
        }
    }

    public void ActivateSurge()
    {
        Debug.Log("ActivateSurge called");

        if (!surgeActive || ResourceManager.Instance == null || isAnimating)
            return;

        StartCoroutine(ActivateAndRewardRoutine());
    }

    IEnumerator ActivateAndRewardRoutine()
    {
        isAnimating = true;
        surgeActive = false;

        Vector3 startScale = surgeObject.transform.localScale;
        Vector3 enlargedScale = originalScale * clickAnimScale;
        Vector3 squashedScale = originalScale * 0.85f;

        float half = clickAnimDuration * 0.5f;
        float t = 0f;

        while (t < half)
        {
            t += Time.deltaTime;
            float k = t / half;
            surgeObject.transform.localScale = Vector3.Lerp(startScale, enlargedScale, k);
            yield return null;
        }

        t = 0f;
        while (t < half)
        {
            t += Time.deltaTime;
            float k = t / half;
            surgeObject.transform.localScale = Vector3.Lerp(enlargedScale, squashedScale, k);
            yield return null;
        }

        surgeObject.SetActive(false);
        surgeObject.transform.localScale = originalScale;

        if (surgeWorldText != null)
            surgeWorldText.text = "";

        ApplyReward(currentRewardType);

        isAnimating = false;
    }

    void ApplyReward(RewardType rewardType)
    {
        switch (rewardType)
        {
            case RewardType.Ducks:
                ResourceManager.Instance.AddDucks(duckRewardAmount);

                if (surgeText != null)
                    surgeText.text = "+" + duckRewardAmount + " Ducks!";

                StartCoroutine(ShowRewardPopup("You got +" + duckRewardAmount + " Ducks!"));
                break;

            case RewardType.Bucks:
                ResourceManager.Instance.bucks += buckRewardAmount;
                ResourceManager.Instance.OnBucksChanged?.Invoke();

                if (surgeText != null)
                    surgeText.text = "+" + buckRewardAmount + " Bucks!";

                StartCoroutine(ShowRewardPopup("You got +" + buckRewardAmount + " Bucks!"));
                break;

            case RewardType.DoubleDucks:
                if (!bonusRunning)
                    StartCoroutine(BonusRoutine());

                if (surgeText != null)
                    surgeText.text = "2x Duck Boost Activated!";

                StartCoroutine(ShowRewardPopup("2x Duck Boost Activated!"));
                break;
        }
    }

    IEnumerator BonusRoutine()
    {
        bonusRunning = true;

        float previousSurgeMultiplier = ResourceManager.Instance.surgeMultiplier;
        ResourceManager.Instance.surgeMultiplier = previousSurgeMultiplier * multiplier;

        Debug.Log("Bonus active: duck production doubled");

        yield return new WaitForSeconds(bonusDuration);

        ResourceManager.Instance.surgeMultiplier = previousSurgeMultiplier;

        if (surgeText != null)
            surgeText.text = "Boost ended";

        yield return new WaitForSeconds(2f);

        if (surgeText != null)
            surgeText.text = "";

        bonusRunning = false;
    }

    IEnumerator ShowRewardPopup(string message)
    {
        if (rewardPopupText == null)
            yield break;

        rewardPopupText.text = message;
        rewardPopupText.gameObject.SetActive(true);

        Vector3 originalPopupScale = rewardPopupText.transform.localScale;
        rewardPopupText.transform.localScale = Vector3.one * 0.5f;

        float duration = 0.25f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float k = t / duration;
            rewardPopupText.transform.localScale = Vector3.Lerp(Vector3.one * 0.5f, originalPopupScale * 1.2f, k);
            yield return null;
        }

        rewardPopupText.transform.localScale = originalPopupScale;

        yield return new WaitForSeconds(1.5f);

        rewardPopupText.text = "";
        rewardPopupText.gameObject.SetActive(false);
        rewardPopupText.transform.localScale = originalPopupScale;
    }
}