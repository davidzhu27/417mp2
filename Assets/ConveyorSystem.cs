using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ConveyorSystem : MonoBehaviour
{
    [Header("Unlocking")]
    public bool duckSellingUnlocked = false;

    [Header("Cooldown UI")]
    public TMP_Text cooldownText;

    [Header("Cooldown")]
    public float stepCooldown = 1.0f;
    private bool isOnCooldown = false;

    [Header("Belt Setup")]
    public GameObject beltPrefab;
    public int initialBeltCount = 2;
    public float beltStepLength = 1.0f;

    [Header("Spawn / Despawn")]
    public Transform spawnPoint;
    public Transform despawnPoint;

    [Header("Manual Step Animation")]
    public float rollDuration = 0.4f;

    [Header("Input")]
    public InputActionReference stepAction;

    [Header("Auto Seller")]
    public bool autoSellerActive = false;

    [Header("Continuous Roll")]
    public float autoRollSpeed = 1.0f;
    public float nextUpgradePrice = 10.0f;

    [Header("Sounds")]
    public ConveyorSounds conveyorSounds;

    private List<GameObject> belts = new List<GameObject>();
    private HashSet<GameObject> beltsSpawnedThisAction = new HashSet<GameObject>();
    private bool isRolling = false;

    void Start()
    {
        SpawnInitialBelts();

        if (cooldownText != null)
            cooldownText.text = "";

        stepAction.action.Enable();
        stepAction.action.performed += OnStepPressed;

        ResourceManager.Instance.OnDuckCountChanged += TryAutoFillBelts;
    }

    void OnDestroy()
    {
        stepAction.action.performed -= OnStepPressed;

        if (ResourceManager.Instance != null)
            ResourceManager.Instance.OnDuckCountChanged -= TryAutoFillBelts;
    }

    void Update()
    {
        if (!duckSellingUnlocked || !autoSellerActive || isRolling)
            return;

        beltsSpawnedThisAction.Clear();
        float delta = autoRollSpeed * Time.deltaTime;
        MoveBelts(delta);
    }

    void OnStepPressed(InputAction.CallbackContext ctx)
    {
        if (!duckSellingUnlocked || autoSellerActive || isRolling || isOnCooldown)
            return;

        StartCoroutine(RollConveyorStep());
        StartCoroutine(StepCooldownRoutine());
    }

    IEnumerator StepCooldownRoutine()
    {
        isOnCooldown = true;
        float timeLeft = stepCooldown;

        while (timeLeft > 0f)
        {
            if (cooldownText != null)
                cooldownText.text = "Cooldown: " + timeLeft.ToString("F1");

            timeLeft -= Time.deltaTime;
            yield return null;
        }

        isOnCooldown = false;

        if (cooldownText != null)
            cooldownText.text = "Ready!";
    }

    IEnumerator RollConveyorStep()
    {
        isRolling = true;
        beltsSpawnedThisAction.Clear();
        conveyorSounds.StartLoop();

        float elapsed = 0f;
        while (elapsed < rollDuration)
        {
            float nextElapsed = Mathf.Min(elapsed + Time.deltaTime, rollDuration);
            float stepDelta = beltStepLength * (nextElapsed - elapsed) / rollDuration;

            MoveBelts(stepDelta);

            elapsed = nextElapsed;
            yield return null;
        }

        isRolling = false;
        conveyorSounds.StopConveyor();
    }

    void MoveBelts(float delta)
    {
        float despawnX = despawnPoint.position.x;
        Vector3 right = Vector3.right * delta;

        conveyorSounds.StartLoop();

        foreach (GameObject belt in belts)
        {
            if (beltsSpawnedThisAction.Contains(belt))
                continue;

            belt.transform.position += right;
        }

        for (int i = belts.Count - 1; i >= 0; i--)
        {
            if (belts[i].transform.position.x < despawnX)
                continue;

            ConveyorBeltSlot slot = belts[i].GetComponent<ConveyorBeltSlot>();
            if (slot != null)
            {
                int sold = slot.SellAll();
                ResourceManager.Instance.SellDucks(sold);
            }

            Destroy(belts[i]);
            belts.RemoveAt(i);

            GameObject newBelt = Instantiate(beltPrefab, spawnPoint.position, spawnPoint.rotation, transform);

            SpawnEase ease = newBelt.GetComponent<SpawnEase>();
            if (ease == null)
                ease = newBelt.AddComponent<SpawnEase>();
            ease.Play();

            ConveyorBeltSlot newSlot = newBelt.GetComponent<ConveyorBeltSlot>();
            if (newSlot != null)
                newSlot.AutoFill();

            belts.Add(newBelt);
            beltsSpawnedThisAction.Add(newBelt);

            TryAutoFillBelts();
        }

        conveyorSounds.StopConveyor();
    }

    void TryAutoFillBelts()
    {
        foreach (GameObject belt in belts)
        {
            ConveyorBeltSlot slot = belt.GetComponent<ConveyorBeltSlot>();
            if (slot == null)
            {
                Debug.Log($"Belt at position x = {belt.transform.position.x:F2} has no ConveyorBeltSlot.");
                continue;
            }

            float x = belt.transform.position.x;

            if (slot.IsFull())
                continue;

            if (ResourceManager.Instance.ducks <= 0)
            {
                Debug.Log($"Belt at position x = {x:F2} is not full but no ducks available.");
                continue;
            }

            slot.AutoFill();
            break;
        }
    }

    void SpawnInitialBelts()
    {
        belts.Clear();
        beltsSpawnedThisAction.Clear();

        for (int i = 0; i < initialBeltCount; i++)
        {
            Vector3 pos = spawnPoint.position - Vector3.right * beltStepLength * i;
            GameObject belt = Instantiate(beltPrefab, pos, spawnPoint.rotation, transform);

            SpawnEase ease = belt.GetComponent<SpawnEase>();
            if (ease == null)
                ease = belt.AddComponent<SpawnEase>();
            ease.Play();

            ConveyorBeltSlot slot = belt.GetComponent<ConveyorBeltSlot>();
            if (slot != null)
                slot.AutoFill();

            belts.Add(belt);
        }
    }
}