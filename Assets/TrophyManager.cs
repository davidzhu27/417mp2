using UnityEngine;

public class TrophyManager : MonoBehaviour
{
    public GameObject trophy500Prefab;
    public GameObject trophy1000Prefab;
    public GameObject trophy1000000Prefab;

    public Transform spawn500;
    public Transform spawn1000;
    public Transform spawn1000000;

    private bool spawned500;
    private bool spawned1000;
    private bool spawned1000000;

    private void Start()
    {
        CheckAndSpawn();

        if (ResourceManager.Instance != null)
            ResourceManager.Instance.OnDuckCountChanged += CheckAndSpawn;
    }

    private void OnDestroy()
    {
        if (ResourceManager.Instance != null)
            ResourceManager.Instance.OnDuckCountChanged -= CheckAndSpawn;
    }

    private void CheckAndSpawn()
    {
        if (ResourceManager.Instance == null) return;

        int bucks = ResourceManager.Instance.bucks;

        if (!spawned500 && bucks >= 500)
        {
            Spawn(trophy500Prefab, spawn500, "Trophy_500");
            spawned500 = true;
        }

        if (!spawned1000 && bucks >= 1000)
        {
            Spawn(trophy1000Prefab, spawn1000, "Trophy_1000");
            spawned1000 = true;
        }

        if (!spawned1000000 && bucks >= 1000000)
        {
            Spawn(trophy1000000Prefab, spawn1000000, "Trophy_1000000");
            spawned1000000 = true;
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