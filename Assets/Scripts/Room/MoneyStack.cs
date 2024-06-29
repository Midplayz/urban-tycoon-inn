using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyStack : MonoBehaviour
{
    public int value = 50;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StatsTracker.Instance.AdjustCurrency(value);
            SoundManager.Instance.PlayMoneyCollectSound();
            gameObject.SetActive(false);
        }
    }
}
