using UnityEngine;
using System.Collections.Generic;

public class ConveyorBeltSlot : MonoBehaviour
{
    public int capacity = 3;

    public Transform duckVisualParent;
    public GameObject duckVisualPrefab;

    private int duckCount = 0;
    private List<GameObject> duckVisuals = new List<GameObject>();

    public bool IsFull()
    {
        return duckCount >= capacity;
    }

    // =========================
    // AUTO-FILL (VISUAL ONLY)
    // =========================
    public void AutoFill()
    {
        int space = capacity - duckCount;
        if (space <= 0)
            return;

        int ducksAvailable = ResourceManager.Instance.ducks;
        int toFill = Mathf.Min(space, ducksAvailable - TotalDucksOnBelts());

        if (toFill <= 0)
            return;

        duckCount += toFill;
        SpawnDuckVisuals(toFill);
    }

    // =========================
    // SELL DUCKS
    // =========================
    public int SellAll()
    {
        int sold = duckCount;
        duckCount = 0;

        foreach (var duck in duckVisuals)
            Destroy(duck);

        duckVisuals.Clear();
        return sold;
    }

    // =========================
    // VISUALS
    // =========================
    void SpawnDuckVisuals(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject duck = Instantiate(duckVisualPrefab, duckVisualParent);
            duck.transform.localPosition = new Vector3(0f, 0f, 0.15f * duckVisuals.Count);
            duckVisuals.Add(duck);
        }
    }

    // =========================
    // SAFETY (OPTIONAL)
    // =========================
    int TotalDucksOnBelts()
    {
        ConveyorBeltSlot[] slots = FindObjectsOfType<ConveyorBeltSlot>();
        int total = 0;
        foreach (var slot in slots)
            total += slot.duckCount;
        return total;
    }
}