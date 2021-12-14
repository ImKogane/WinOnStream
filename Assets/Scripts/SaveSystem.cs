using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Debug = UnityEngine.Debug;

public static class SaveSystem 
{
    private static string _saveFilepath = "Saves";
    private static string _saveFileExtension = ".json";
    private static string _saveFileName = "save";

    private static Stopwatch _stopWatch = new Stopwatch();
    
    public static void LoadData(SaveData data)
    {
        
    }
    
    public static async void SaveData()
    {
        _stopWatch.Restart();
        Debug.Log($"[{_stopWatch.ElapsedMilliseconds} SaveData: start");
        SaveData data = GetData();
        
        await WriteData(data);
        Debug.Log($"[{_stopWatch.ElapsedMilliseconds} SaveData: stop");
        
    }
    
    public static SaveData GetData()
    {
        Debug.Log($"[{_stopWatch.ElapsedMilliseconds} GetData: start");
        
        List<PlayerData> tempPlayersDatas = new List<PlayerData>();
        List<TileData> tempTilesDatas = new List<TileData>();
        
        
        foreach (Player player in PlayerManager.Instance.PlayerList)
        {
            PlayerData tempPlayerData;
            
            tempPlayerData._playerName = player.namePlayer;
            tempPlayerData._playerHealth = player._playerLife;
            tempPlayerData._durationOfActiveBurningDebuff = new List<int>();
            tempPlayerData._durationOfActiveFreezeDebuff = new List<int>();

            if (player.debuffList.Count > 0)
            {
                foreach (var debuff in player.debuffList)
                {
                    switch (debuff)
                    {
                        case BurningDebuff burningDebuff:
                            tempPlayerData._durationOfActiveBurningDebuff.Add(burningDebuff.duration);
                            break;
                        
                        case FreezeDebuff freezeDebuff:
                            tempPlayerData._durationOfActiveFreezeDebuff.Add(freezeDebuff.duration);
                            break;
                    }
                }
            }

            tempPlayerData._playerTile = player.CurrentTile.GetCoord();
            if (player.playerWeapon != null)
            {
                tempPlayerData._weaponData = new WeaponData(player.playerWeapon);
            }
            else
            {
                tempPlayerData._weaponData = new WeaponData();
            }

            if (player.playerMoveBuff != null)
            {
                tempPlayerData._weaponBuffData = new WeaponBuffData(player.playerWeaponBuff);
            }
            else
            {
                tempPlayerData._weaponBuffData = new WeaponBuffData();
            }

            if (player.playerWeaponBuff != null)
            {
                tempPlayerData._movementBuffData = new MovementBuffData(player.playerMoveBuff);
            }
            else
            {
                tempPlayerData._movementBuffData = new MovementBuffData();
            }
            
            tempPlayerData._materialIndex =
                PlayerManager.Instance.SkinSystem.GetMaterialIndex(player.playerModel.material);
            tempPlayerData._skinnedMeshIndex =
                PlayerManager.Instance.SkinSystem.GetSkinIndex(player.playerModel.sharedMesh);
            
            tempPlayersDatas.Add(tempPlayerData);
        }

        foreach (Tile tile in BoardManager.Instance.tilesList)
        {
            TileData tempTileData;

            tempTileData._tileTransform = new TransformData(tile.transform);
            tempTileData._tileCoords = tile.GetCoord();
            tempTileData._hasObstacle = tile.hasObstacle;
            tempTileData._tileTrapCount = tile.trapList.Count;
            
            tempTilesDatas.Add(tempTileData);
        }
        
        SaveData data = new SaveData()
        {
            _currentTurn = GlobalManager.Instance.GetCurrentTurn(),
            _playersDatas = tempPlayersDatas,
            _tilesDatas = tempTilesDatas
        };

        Debug.Log($"[{_stopWatch.ElapsedMilliseconds} SaveData: stop");
        
        return data;
    }

    private static async Task WriteData(SaveData data)
    {
        Debug.Log($"[{_stopWatch.ElapsedMilliseconds} WriteData: start");
        
        string directoryPath = Path.Combine(Application.persistentDataPath, _saveFilepath);
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string filePath = Path.Combine(directoryPath, _saveFileName + _saveFileExtension);

        string jsonData = JsonUtility.ToJson(data);

        byte[] bytes = await Task.Run(() =>
        {
            string jsonData = JsonUtility.ToJson(data);
            return Encoding.Unicode.GetBytes(jsonData);
        });
        
        
        //File.WriteAllText(filePath, jsonData);

        using (FileStream filestream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write))
        {
            await filestream.WriteAsync(bytes, 0, bytes.Length);
        }
        
        Debug.Log($"[{_stopWatch.ElapsedMilliseconds} WriteData: start");

    }

    

}