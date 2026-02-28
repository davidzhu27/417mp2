using System;

[Serializable]
public class SaveData
{
    public int ducks;
    public int bucks;
    public bool sellingUnlocked;

    public long lastSaveUnixSeconds;
}