using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class ConveyorSystem : MonoBehaviour
{
    [Header("Belt Setup")]
    public GameObject beltPrefab;
    public int initialBeltCount = 3;
    public float beltStepLength = 1.0f;

    [Header("Spawn / Despawn")]
    public Transform spawnPoint;
    public Transform despawnPoint;

    [Header("Roll Animation")]
    public float rollDuration = 0.4f;

    [Header("Input")]
    public InputActionReference stepAction;

    private List<GameObject> belts = new List<GameObject>();
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
        {
            ResourceManager.Instance.OnDuckCountChanged -= TryAutoFillBelts;
        }
    }

    void TryAutoFillBelts()
    {
        // Belts are ordered back → front
        foreach (GameObject belt in belts)
        {
            ConveyorBeltSlot slot = belt.GetComponent<ConveyorBeltSlot>();
            if (slot == null)
                continue;

            if (!slot.IsFull() && ResourceManager.Instance.ducks > 0)
            {
                slot.AutoFill();
                break; // only fill ONE belt per duck increment
            }
        }
    }
    // ===============================
    // INITIALIZATION
    // ===============================

    void SpawnInitialBelts()
    {
        belts.Clear();

        for (int i = 0; i < initialBeltCount; i++)
        {
            Vector3 pos = spawnPoint.position
                        - Vector3.right * beltStepLength * i;

            GameObject belt = Instantiate(
                beltPrefab,
                pos,
                spawnPoint.rotation,
                transform
            );
            ConveyorBeltSlot slot = belt.GetComponent<ConveyorBeltSlot>();
            if (slot != null)
            {
                slot.AutoFill();
            }
            belts.Add(belt);
        }
    }

    // ===============================
    // INPUT
    // ===============================

    void OnStepPressed(InputAction.CallbackContext ctx)
    {
        if (!isRolling)
            StartCoroutine(RollConveyor());
    }

    // ===============================
    // CORE LOGIC
    // ===============================

    IEnumerator RollConveyor()
    {
        isRolling = true;

        Vector3 startOffset = Vector3.zero;
        Vector3 endOffset = Vector3.right * beltStepLength;

        float t = 0f;

        // Cache start positions
        Dictionary<GameObject, Vector3> startPositions = new Dictionary<GameObject, Vector3>();
        foreach (GameObject belt in belts)
            startPositions[belt] = belt.transform.position;

        // Smooth roll animation
        while (t < rollDuration)
        {
            float alpha = t / rollDuration;
            Vector3 offset = Vector3.Lerp(startOffset, endOffset, alpha);

            foreach (GameObject belt in belts)
            {
                belt.transform.position = startPositions[belt] + offset;
            }

            t += Time.deltaTime;
            yield return null;
        }

        // Snap exactly to final position
        foreach (GameObject belt in belts)
        {
            belt.transform.position = startPositions[belt] + endOffset;
        }

        HandleDespawnAndSpawn();

        isRolling = false;
    }

    void HandleDespawnAndSpawn()
    {
        for (int i = belts.Count - 1; i >= 0; i--)
        {
            if (belts[i].transform.position.x >= despawnPoint.position.x)
            {
                ConveyorBeltSlot slot = belts[i].GetComponent<ConveyorBeltSlot>();
                if (slot != null)
                {
                    int sold = slot.SellAll();
                    ResourceManager.Instance.SellDucks(sold);
                }

                Destroy(belts[i]);
                belts.RemoveAt(i);

                SpawnNewBelt();
            }
        }
    }

    void SpawnNewBelt()
    {
        GameObject newBelt = Instantiate(
            beltPrefab,
            spawnPoint.position,
            spawnPoint.rotation,
            transform
        );

        ConveyorBeltSlot slot = newBelt.GetComponent<ConveyorBeltSlot>();
        if (slot != null)
        {
            slot.AutoFill();
        }

        belts.Add(newBelt);
    }
}