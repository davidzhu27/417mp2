using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class ConveyorSystem : MonoBehaviour
{
    [Header("Unlocking")]
    public bool duckSellingUnlocked = false;

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
    public float autoRollSpeed = 1.0f; // units per second
    public int nextUpgradePrice = 100;

    private List<GameObject> belts = new List<GameObject>();
    private HashSet<GameObject> beltsSpawnedThisAction = new HashSet<GameObject>();
    private bool isRolling = false;

    void Start()
    {
        SpawnInitialBelts();

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

    // ===============================
    // UPDATE (AUTO MODE ONLY)
    // ===============================

    void Update()
    {
        if (!duckSellingUnlocked || !autoSellerActive || isRolling)
            return;

        beltsSpawnedThisAction.Clear();
        float delta = autoRollSpeed * Time.deltaTime;
        MoveBelts(delta);
    }

    // ===============================
    // INPUT (MANUAL MODE ONLY)
    // ===============================

    void OnStepPressed(InputAction.CallbackContext ctx)
    {
        if (!duckSellingUnlocked || autoSellerActive || isRolling)
            return;

        StartCoroutine(RollConveyorStep());
    }

    // ===============================
    // MANUAL STEP ANIMATION
    // ===============================

    IEnumerator RollConveyorStep()
    {
        isRolling = true;
        beltsSpawnedThisAction.Clear();

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
    }

    // ===============================
    // MOVEMENT CORE
    // ===============================

    void MoveBelts(float delta)
    {
        float despawnX = despawnPoint.position.x;
        Vector3 right = Vector3.right * delta;

        // Move all belts that were not spawned during this action
        foreach (GameObject belt in belts)
        {
            if (beltsSpawnedThisAction.Contains(belt))
                continue;
            belt.transform.position += right;
        }

        // Despawn any belt past the despawn point and spawn a new one at spawn (new one does not move this action)
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
            ConveyorBeltSlot newSlot = newBelt.GetComponent<ConveyorBeltSlot>();
            if (newSlot != null)
                newSlot.AutoFill();

            belts.Add(newBelt);
            beltsSpawnedThisAction.Add(newBelt);

            TryAutoFillBelts();
        }
    }

    // ===============================
    // AUTO-FILL
    // ===============================

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
            {
                continue;
            }
            if (ResourceManager.Instance.ducks <= 0)
            {
                Debug.Log($"Belt at position x = {x:F2} is not full but no ducks available.");
                continue;
            }

            slot.AutoFill();
            break;
        }
    }

    // ===============================
    // INITIALIZATION
    // ===============================

    void SpawnInitialBelts()
    {
        belts.Clear();
        beltsSpawnedThisAction.Clear();

        for (int i = 0; i < initialBeltCount; i++)
        {
            Vector3 pos = spawnPoint.position - Vector3.right * beltStepLength * i;
            GameObject belt = Instantiate(beltPrefab, pos, spawnPoint.rotation, transform);

            ConveyorBeltSlot slot = belt.GetComponent<ConveyorBeltSlot>();
            if (slot != null)
                slot.AutoFill();

            belts.Add(belt);
        }
    }
}
