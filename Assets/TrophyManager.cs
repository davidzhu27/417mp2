using UnityEngine;

public class TrophyManager : MonoBehaviour
{
    // Saved flags (persist via SaveManager)
    public static bool Spawned500;
    public static bool Spawned1000;
    public static bool Spawned1000000;

    public GameObject trophy500Prefab;
    public GameObject trophy1000Prefab;
    public GameObject trophy1000000Prefab;

    public Transform spawn500;
    public Transform spawn1000;
    public Transform spawn1000000;

    private void Start()
    {
        CheckAndSpawn();

        // Trophies are based on BUCKS
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.OnBuckCountChanged += OnBucksChanged;
    }

    private void OnDestroy()
    {
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.OnBuckCountChanged -= OnBucksChanged;
    }

    private void OnBucksChanged(int _)
    {
        CheckAndSpawn();
    }

    private void CheckAndSpawn()
    {
        if (ResourceManager.Instance == null) return;

        int bucks = ResourceManager.Instance.bucks;

        if (!Spawned500 && bucks >= 500)
        {
            Spawn(trophy500Prefab, spawn500, "Trophy_500");
            Spawned500 = true;
        }

        if (!Spawned1000 && bucks >= 1000)
        {
            Spawn(trophy1000Prefab, spawn1000, "Trophy_1000");
            Spawned1000 = true;
        }

        if (!Spawned1000000 && bucks >= 1000000)
        {
            Spawn(trophy1000000Prefab, spawn1000000, "Trophy_1000000");
            Spawned1000000 = true;
        }
    }

    private void Spawn(GameObject prefab, Transform spawnPoint, string name)
    {
        if (prefab == null)
        {
            Debug.LogWarning($"TrophyManager: Missing prefab for {name}");
            return;
        }

        Vector3 pos = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion rot = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        GameObject obj = Instantiate(prefab, pos, rot);
        obj.name = name;

        Debug.Log($"Spawned {name}");
    }
}