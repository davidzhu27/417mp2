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
            bucks = ResourceManager.Instance.bucks
            // sellingUnlocked 先不管，不写也没关系（默认 false）
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

        ResourceManager.Instance.ducks = Mathf.Max(0, data.ducks);
        ResourceManager.Instance.bucks = Mathf.Max(0, data.bucks);

        ResourceManager.Instance.OnDuckCountChanged?.Invoke();
        Debug.Log("Loaded save.");
    }

    public void DeleteSave()
    {
        if (File.Exists(SavePath)) File.Delete(SavePath);
        Debug.Log("Save deleted.");
    }
}