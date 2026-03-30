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

    public ParticleSystem trophySpawnParticles;
    public AudioSource audioSource;

    public AudioClip bronzeSpawnClip;
    public AudioClip silverSpawnClip;
    public AudioClip goldSpawnClip;

    [Header("Pop Animation")]
    public float popDuration = 0.3f;
    public float popOvershoot = 1.2f;

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

        if ((bucks >= 50 || Spawned500) && !has500Trophy)
        {
            Spawn(trophy500Prefab, spawn500, "Trophy_Bronze", bronzeSpawnClip);
            Spawned500 = true;
        }

        if ((bucks >= 100 || Spawned1000) && !has1000Trophy)
        {
            Spawn(trophy1000Prefab, spawn1000, "Trophy_Silver", silverSpawnClip);
            Spawned1000 = true;
        }

        if ((bucks >= 1000 || Spawned1000000) && !has1MTrophy)
        {
            Spawn(trophy1000000Prefab, spawn1000000, "Trophy_Gold", goldSpawnClip);
            Spawned1000000 = true;
        }
    }

    private void Spawn(GameObject prefab, Transform spawnPoint, string name, AudioClip clip)
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

        // 1. Particles
        if (trophySpawnParticles != null)
        {
            ParticleSystem ps = Instantiate(trophySpawnParticles, pos, Quaternion.identity);
            ps.Play();
            Destroy(ps.gameObject, 3f);
        }

        // 2.Sound
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }

        // 3. Pop-in scale animation (extra polish)
        StartCoroutine(PopIn(obj.transform));

        Debug.Log($"Spawned {name} at {pos}");
    }
    private IEnumerator PopIn(Transform target)
    {
        Vector3 finalScale = target.localScale;
        target.localScale = Vector3.zero;

        float t = 0f;
        while (t < popDuration)
        {
            t += Time.deltaTime;
            float p = t / popDuration;

            if (p < 0.7f)
            {
                float p1 = p / 0.7f;
                target.localScale = Vector3.Lerp(Vector3.zero, finalScale * popOvershoot, p1);
            }
            else
            {
                float p2 = (p - 0.7f) / 0.3f;
                target.localScale = Vector3.Lerp(finalScale * popOvershoot, finalScale, p2);
            }

            yield return null;
        }

        target.localScale = finalScale;
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
