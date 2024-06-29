using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class RoomController : MonoBehaviour
{
    public enum RoomState {Locked, Vacant, Occupied, NeedsCleaning }
    public RoomState currentState;
    public float doorLerpSpeed = 2.0f;
    public bool isUnlocked = false;
    public PathDefiner pathToThisRoom;
    public int roomID;

    public GameObject door;
    public GameObject roof;
    public GameObject cleaningCollider;
    public GameObject moneyAsset;

    [Header("Occupied Sequence")]
    [field: SerializeField] private int minRoomOccupiedTime = 10;
    [field: SerializeField] private int maxRoomOccupiedTime = 30;

    [Header("Purchasing")]
    [field: SerializeField] private PurchaseTrigger purchaseScript;
    [field: SerializeField] private GameObject purchaseIconRoof;
    public int roomUnlockPrice;

    private Quaternion doorOpenRotation = Quaternion.Euler(-90, 0, -90);
    private Quaternion doorClosedRotation = Quaternion.Euler(-90, 0, 0);

    public delegate void RoomStateChanged(RoomController room);
    public event RoomStateChanged OnRoomStateChanged;

    private void Start()
    {
        moneyAsset.SetActive(false);
        if(!isUnlocked && purchaseScript == null)
        {
            Debug.LogError("Purchase Script Missing for Object: " + gameObject.name);
        }
        SetInitialState();
    }
    public void SetInitialState()
    {
        if (!isUnlocked)
        {
            SetRoomState(RoomState.Locked);
        }
        else
        {
            purchaseIconRoof.SetActive(false);
            purchaseScript.gameObject.SetActive(false);
            SetRoomState(RoomState.Vacant);
        }
    }
    public void SetRoomState(RoomState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case RoomState.Locked:
                CloseDoor();
                purchaseScript.gameObject.SetActive(true);
                cleaningCollider.SetActive(false);
                roof.SetActive(true);
                purchaseIconRoof.SetActive(true);
                break;
            case RoomState.Vacant:
                OpenDoor();
                cleaningCollider.SetActive(false);
                break;
            case RoomState.Occupied:
                break;
            case RoomState.NeedsCleaning:
                OpenDoor();
                moneyAsset.SetActive(true);
                cleaningCollider.SetActive(true);
                break;
        }

        OnRoomStateChanged?.Invoke(this);
    }

    private void OpenDoor()
    {
        //StopAllCoroutines();
        if (door.transform.localRotation != doorOpenRotation)
        {
            StartCoroutine(LerpDoorRotation(doorClosedRotation, doorOpenRotation));
        }
        roof.SetActive(false);
    }

    private void CloseDoor()
    {
        //StopAllCoroutines();
        if (door.transform.localRotation != doorClosedRotation)
        {
            StartCoroutine(LerpDoorRotation(doorOpenRotation, doorClosedRotation));
        }
        roof.SetActive(true);
    }

    private IEnumerator LerpDoorRotation(Quaternion startRotation, Quaternion endRotation)
    {
        float timeElapsed = 0;
        while (timeElapsed < 1)
        {
            door.transform.localRotation = Quaternion.Lerp(startRotation, endRotation, timeElapsed);
            timeElapsed += Time.deltaTime * doorLerpSpeed;
            yield return null;
        }
        door.transform.localRotation = endRotation;
    }

    public IEnumerator StartOccupiedSequence(GameObject callingObject)
    {
        float occupiedTime = UnityEngine.Random.Range(minRoomOccupiedTime, maxRoomOccupiedTime + 1);
        yield return new WaitForSeconds(occupiedTime);
        SetRoomState(RoomState.NeedsCleaning);
        Destroy(callingObject);
    }

    public void OnOccupiedActions()
    {
        CloseDoor();
        cleaningCollider.SetActive(false);
    }

    public void OnUnlock()
    {
        isUnlocked = true;
        purchaseIconRoof.SetActive(false);
        purchaseScript.gameObject.SetActive(false);
        SetRoomState(RoomState.Vacant);
        StatsTracker.Instance.AdjustCurrency(-roomUnlockPrice);
        List<bool> listOfBools = new List<bool>();
        listOfBools = SavingLoadingManager.Instance.LoadRoomUnlockStates();
        listOfBools[roomID] = isUnlocked;
        SavingLoadingManager.Instance.SaveRoomUnlockStates(listOfBools);
    }
}