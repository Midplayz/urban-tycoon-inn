using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class SavingLoadingManager : MonoBehaviour
{
    public static SavingLoadingManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            DeleteAllData();
        }
    }

    public void SaveRoomUnlockStates(List<bool> roomUnlockStates)
    {
        for (int i = 0; i < roomUnlockStates.Count; i++)
        {
            PlayerPrefs.SetInt("RoomUnlocked_" + i, roomUnlockStates[i] ? 1 : 0);
        }
        for (int i = 0; i < roomUnlockStates.Count; i++)
        {
        Debug.Log("Saving State " + roomUnlockStates[i]);
        }
            PlayerPrefs.SetInt("RoomCount", roomUnlockStates.Count);
    }

    public List<bool> LoadRoomUnlockStates()
    {
        List<bool> roomUnlockStates = new List<bool>();
        int roomCount = PlayerPrefs.GetInt("RoomCount", 0);
        for (int i = 0; i < roomCount; i++)
        {
            bool isUnlocked = PlayerPrefs.GetInt("RoomUnlocked_" + i, 0) == 1;
            roomUnlockStates.Add(isUnlocked);
        }
        for (int i = 0; i < roomUnlockStates.Count; i++)
        {
            Debug.Log("Loading State " + roomUnlockStates[i]);
        }
        return roomUnlockStates;
    }

    public void SavePlayerMoney(int money)
    {
        PlayerPrefs.SetInt("PlayerMoney", money);
    }

    public int LoadPlayerMoney()
    {
        return PlayerPrefs.GetInt("PlayerMoney", 0);
    }

    public void SaveUpgradeLevel(int upgradeLevel)
    {
        PlayerPrefs.SetInt("UpgradeLevel", upgradeLevel);
    }

    public int LoadUpgradeLevel()
    {
        return PlayerPrefs.GetInt("UpgradeLevel", 0);
    }

    public void SaveAll(List<bool> roomUnlockStates, int money, int upgradeLevel)
    {
        SaveRoomUnlockStates(roomUnlockStates);
        SavePlayerMoney(money);
        SaveUpgradeLevel(upgradeLevel);
        PlayerPrefs.Save();
    }

    public void LoadAll(out List<bool> roomUnlockStates, out int money, out int upgradeLevel)
    {
        roomUnlockStates = LoadRoomUnlockStates();
        money = LoadPlayerMoney();
        upgradeLevel = LoadUpgradeLevel();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.Save();
    }

    public void DeleteAllData()
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
