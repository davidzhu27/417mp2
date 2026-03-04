using System.Collections;
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

    private bool _subscribedToBucks;

    private void OnEnable()
    {
        // subscribe as early as possible
        TrySubscribeToBucks();

        // run an initial check after one frame so ResourceManager is definitely initialized
        StartCoroutine(CheckNextFrame());
    }

    private void OnDisable()
    {
        if (_subscribedToBucks && ResourceManager.Instance != null)
        {
            ResourceManager.Instance.OnBucksChanged -= OnBucksChanged;
            _subscribedToBucks = false;
        }
    }

    private IEnumerator CheckNextFrame()
    {
        yield return null;
        // In case ResourceManager was created after OnEnable
        TrySubscribeToBucks();
        CheckAndSpawn();
    }

    private void TrySubscribeToBucks()
    {
        if (_subscribedToBucks) return;
        if (ResourceManager.Instance == null) return;

        ResourceManager.Instance.OnBucksChanged += OnBucksChanged;
        _subscribedToBucks = true;
    }

    private void OnBucksChanged()
    {
        CheckAndSpawn();
    }

    private void CheckAndSpawn()
    {
        if (ResourceManager.Instance == null) return;

        int bucks = ResourceManager.Instance.bucks;

        Debug.Log($"[TrophyManager] bucks={bucks}, flags: 500={Spawned500}, 1000={Spawned1000}, 1M={Spawned1000000}");

        bool has500Trophy = GameObject.Find("Trophy_Bronze") != null;
        bool has1000Trophy = GameObject.Find("Trophy_Silver") != null;
        bool has1MTrophy = GameObject.Find("Trophy_Gold") != null;

        if ((bucks >= 500 || Spawned500) && !has500Trophy)
        {
            Spawn(trophy500Prefab, spawn500, "Trophy_Bronze");
            Spawned500 = true;
        }

        if ((bucks >= 1000 || Spawned1000) && !has1000Trophy)
        {
            Spawn(trophy1000Prefab, spawn1000, "Trophy_Silver");
            Spawned1000 = true;
        }

        if ((bucks >= 1000000 || Spawned1000000) && !has1MTrophy)
        {
            Spawn(trophy1000000Prefab, spawn1000000, "Trophy_Gold");
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

        if (spawnPoint != null)
        {
            obj.transform.localScale = spawnPoint.localScale;
        }

        Debug.Log($"Spawned {name} at {pos}");
    }

    // ===== TEST HELPERS =====
    // 右键组件标题就能点：Reset Trophy Flags（只用于测试）
    [ContextMenu("Reset Trophy Flags")]
    public void ResetTrophyFlags()
    {
        Spawned500 = false;
        Spawned1000 = false;
        Spawned1000000 = false;
        Debug.Log("Trophy flags reset.");
        CheckAndSpawn();
    }
}
