using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class StatsTracker : MonoBehaviour
{
    public static StatsTracker Instance;
    [SerializeField] private int cashInHand = 0;
    [SerializeField] private TextMeshProUGUI cashText;
    [SerializeField] private TextMeshProUGUI targetText;
    private List<RoomController> roomControllers;
    public event System.Action OnCurrencyAdjusted;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        cashInHand = SavingLoadingManager.Instance.LoadPlayerMoney();
        OnCurrencyAdjusted.Invoke();
        cashText.text = "$" + cashInHand.ToString();
        if (RoomManager.Instance != null && RoomManager.Instance.rooms != null)
        {
            GetNextTarget();
        }
    }

    public void AdjustCurrency(int amount)
    {
        cashInHand += amount;
        cashText.text = "$" + cashInHand.ToString();
        OnCurrencyAdjusted?.Invoke();
        if(amount<0)
        {
            GetNextTarget();
        }
    }
    
    private void GetNextTarget()
    {
        roomControllers = new List<RoomController>(
            RoomManager.Instance.rooms
                .Where(room => !room.isUnlocked)
                .OrderBy(room => room.roomUnlockPrice)
        );
        if (roomControllers.Count > 0)
        {
            targetText.text = "Next Target: $" + roomControllers[0].roomUnlockPrice.ToString();
        }
        else
        {
            targetText.text = "Everything Unlocked! Congrats!";
        }
    }

    public int ReturnCashInHand()
    {
        return cashInHand;
    }

    private void OnApplicationQuit()
    {
        SavingLoadingManager.Instance.SavePlayerMoney(cashInHand);
    }
}
