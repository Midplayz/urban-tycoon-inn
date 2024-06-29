using UnityEngine;
using System.Collections.Generic;

public class RoomManager : MonoBehaviour
{
    public List<RoomController> rooms = new List<RoomController>();

    public List<RoomController> vacantRooms = new List<RoomController>();
    public List<RoomController> occupiedRooms = new List<RoomController>();
    public List<RoomController> needsCleaningRooms = new List<RoomController>();
    public List<RoomController> unlockedRooms = new List<RoomController>();

    public static RoomManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        List<bool> allRoomsUnlockedState = new List<bool>();
        allRoomsUnlockedState = SavingLoadingManager.Instance.LoadRoomUnlockStates();
        if (allRoomsUnlockedState.Count > 0)
        {
            for (int i = 0; i < rooms.Count; i++)
            {
                rooms[i].isUnlocked = allRoomsUnlockedState[i];
                rooms[i].SetInitialState();
            }
        }
        else 
        {
            List<bool> isUnlockedList = new List<bool>();
            foreach (RoomController room in rooms)
            {
                isUnlockedList.Add(room.isUnlocked);
            }
            SavingLoadingManager.Instance.SaveRoomUnlockStates(isUnlockedList);
        }
        foreach (RoomController room in rooms)
        {
            room.OnRoomStateChanged += HandleRoomStateChanged;
            UpdateRoomLists(room);
        }
    }

    private void HandleRoomStateChanged(RoomController room)
    {
        UpdateRoomLists(room);
    }

    private void UpdateRoomLists(RoomController room)
    {
        RemoveFromAllLists(room);

        switch (room.currentState)
        {
            case RoomController.RoomState.Vacant:
                vacantRooms.Add(room);
                break;
            case RoomController.RoomState.Occupied:
                occupiedRooms.Add(room);
                break;
            case RoomController.RoomState.NeedsCleaning:
                needsCleaningRooms.Add(room);
                break;
        }

        if (room.isUnlocked)
        {
            unlockedRooms.Add(room);
        }
    }

    private void RemoveFromAllLists(RoomController room)
    {
        vacantRooms.Remove(room);
        occupiedRooms.Remove(room);
        needsCleaningRooms.Remove(room);
        unlockedRooms.Remove(room);
    }

    public List<RoomController> GetVacantRooms()
    {
        if (vacantRooms.Count > 0)
        {
            List<RoomController> sortedVacantRooms = new List<RoomController>(vacantRooms);
            sortedVacantRooms.Sort((room1, room2) => room2.roomID.CompareTo(room1.roomID));
            return sortedVacantRooms;
        }
        return null;
    }

    public List<RoomController> GetOccupiedRooms()
    {
        return new List<RoomController>(occupiedRooms);
    }

    public List<RoomController> GetNeedsCleaningRooms()
    {
        return new List<RoomController>(needsCleaningRooms);
    }

    public List<RoomController> GetUnlockedRooms()
    {
        return new List<RoomController>(unlockedRooms);
    }

    public RoomController GetRoomByIndex(int index)
    {
        foreach (RoomController room in rooms)
        {
            if (room.roomID == index)
            {
                return room;
            }
        }
        return null;
    }

    private void OnApplicationQuit()
    {
        List<bool> isUnlockedList = new List<bool>();
        foreach (RoomController room in rooms)
        {
            isUnlockedList.Add(room.isUnlocked);
        }
        SavingLoadingManager.Instance.SaveRoomUnlockStates(isUnlockedList);
    }
}
