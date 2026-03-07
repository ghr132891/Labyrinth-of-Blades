using UnityEngine;
public interface ISaveable
{
    public void LoadData(GameData gameData);
    public void SaveData(ref GameData gameData);


}
