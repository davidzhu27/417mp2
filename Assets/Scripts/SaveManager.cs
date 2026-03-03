using System;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private const string SaveFileName = "save.json";
    private string SavePath => Path.Combine(Application.persistentDataPath, SaveFileName);

    private void Start()
    {
        Load();
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private void OnDisable()
    {
        Save();
    }

    public void Save()
    {
        if (ResourceManager.Instance == null)
        {
            Debug.LogWarning("SaveManager: ResourceManager.Instance is null, cannot save.");
            return;
        }

        SaveData data = new SaveData
        {
            ducks = ResourceManager.Instance.ducks,
            bucks = ResourceManager.Instance.bucks,

            generators = ResourceManager.Instance.generators,
            multipliers = ResourceManager.Instance.multipliers,

            duckBreedingUnlocked = ResourceManager.Instance.duckBreedingUnlocked,
            duckSellingUnlocked = ResourceManager.Instance.duckSellingUnlocked,

            // Trophies (saved as flags so they persist between sessions)
            trophy500Spawned = TrophyManager.Spawned500,
            trophy1000Spawned = TrophyManager.Spawned1000,
            trophy1000000Spawned = TrophyManager.Spawned1000000,

            lastSaveUnixSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SavePath, json);

        Debug.Log($"Saved to: {SavePath}");
    }

    public void Load()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("No save file found. Starting fresh.");
            return;
        }

        if (ResourceManager.Instance == null)
        {
            Debug.LogWarning("SaveManager: ResourceManager.Instance is null, cannot load yet.");
            return;
        }

        string json = File.ReadAllText(SavePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        long dt = now - data.lastSaveUnixSeconds;
        dt = Math.Max(0, dt);

        // Idle progress (simple): use current production rate if possible
        // If generators exist, use ResourceManager's actual rate; otherwise fall back.
        float ducksPerSecond = (ResourceManager.Instance != null) ? ResourceManager.Instance.GetDucksPerSecond() : 0.5f;
        if (ducksPerSecond <= 0f) ducksPerSecond = 0.5f;

        int gained = Mathf.FloorToInt(ducksPerSecond * dt);

        // Restore resources
        ResourceManager.Instance.ducks = Mathf.Max(0, data.ducks + gained);
        ResourceManager.Instance.bucks = Mathf.Max(0, data.bucks);

        // Restore generators / powerups
        ResourceManager.Instance.generators = Mathf.Max(0, data.generators);
        ResourceManager.Instance.multipliers = Mathf.Max(0, data.multipliers);

        // Restore unlocks
        ResourceManager.Instance.duckBreedingUnlocked = data.duckBreedingUnlocked;
        ResourceManager.Instance.duckSellingUnlocked = data.duckSellingUnlocked;

        // Restore trophy flags (so trophies don't re-spawn on reload)
        TrophyManager.Spawned500 = data.trophy500Spawned;
        TrophyManager.Spawned1000 = data.trophy1000Spawned;
        TrophyManager.Spawned1000000 = data.trophy1000000Spawned;

        Debug.Log($"Idle progress: +{gained} ducks over {dt} seconds.");

        // Notify UI/systems (your ResourceManager events are Action<int>)
        ResourceManager.Instance.OnDuckCountChanged?.Invoke(ResourceManager.Instance.ducks);
        ResourceManager.Instance.OnBuckCountChanged?.Invoke(ResourceManager.Instance.bucks);

        Debug.Log("Loaded save.");
    }

    public void DeleteSave()
    {
        if (File.Exists(SavePath)) File.Delete(SavePath);

        // Also reset trophy flags so a fresh run behaves correctly
        TrophyManager.Spawned500 = false;
        TrophyManager.Spawned1000 = false;
        TrophyManager.Spawned1000000 = false;

        Debug.Log("Save deleted.");
    }

    [Serializable]
    private class SaveData
    {
        public int ducks;
        public int bucks;

        public int generators;
        public int multipliers;

        public bool duckBreedingUnlocked;
        public bool duckSellingUnlocked;

        public bool trophy500Spawned;
        public bool trophy1000Spawned;
        public bool trophy1000000Spawned;

        public long lastSaveUnixSeconds;
    }
}