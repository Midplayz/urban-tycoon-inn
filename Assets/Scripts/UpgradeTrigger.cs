using System.Collections;
using UnityEngine;
using static RoomController;
using UnityEngine.UI;
using TMPro;

public class UpgradeTrigger : MonoBehaviour
{
    public float stayTime = 1.0f;
    private float timer = 0.0f;
    private bool playerInside = false;
    private bool playerStaying = false;
    public GameObject particleEffect;
    public Material notEnoughMaterial;
    public Material enoughMaterial;
    public GameObject imagePiece;
    public int upgradeCost = 500;

    public Image progressBarImage;
    public TextMeshProUGUI progressText;
    public float targetTime = 5.0f;
    private float currentTimeScale = 1f;
    private int upgradeLevel = 0;

    private void OnDisable()
    {
        if (StatsTracker.Instance != null)
        {
            StatsTracker.Instance.OnCurrencyAdjusted -= HandleCurrencyAdded;
        }
    }
    private void Start()
    {
        upgradeLevel = SavingLoadingManager.Instance.LoadUpgradeLevel();
        if(upgradeLevel>0)
        {
            Time.timeScale = currentTimeScale + (upgradeLevel * 0.5f);
            upgradeCost = upgradeCost * (int)Mathf.Pow(2, upgradeLevel);
        }
        StatsTracker.Instance.OnCurrencyAdjusted += HandleCurrencyAdded;
        ChangeVisuals();
    }
    public void OnPlayerEnterFunction()
    {
        progressText.text = "Applying Upgrade...";
        progressText.gameObject.SetActive(true);
        StartProgressBar();
    }

    public void OnPlayerLeaveFunction()
    {
        timer = 0.0f;
        playerInside = false;
        playerStaying = false;
        progressBarImage.fillAmount = 0.0f;
        progressText.gameObject.SetActive(false);
        StopAllCoroutines();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (StatsTracker.Instance.ReturnCashInHand() >= upgradeCost)
            {
                playerInside = true;
            }
            else
            {
                progressText.text = "You need $" + upgradeCost.ToString() + "!";
                progressText.gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && playerInside && !playerStaying)
        {
            timer += Time.deltaTime;
            if (timer >= stayTime)
            {
                OnPlayerEnterFunction();
                playerStaying = true;
                timer = 0.0f;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerLeaveFunction();
        }
    }

    public IEnumerator FillProgressBar()
    {
        float currentTime = 0.0f;
        while (currentTime < targetTime)
        {
            currentTime += Time.deltaTime;
            progressBarImage.fillAmount = Mathf.Clamp01(currentTime / targetTime);
            yield return null;
        }
        onUpgrade();
        SoundManager.Instance.PlayPurchaseSound();
        OnPlayerLeaveFunction();
    }

    public void StartProgressBar()
    {
        StopAllCoroutines();
        StartCoroutine(FillProgressBar());
    }

    private void HandleCurrencyAdded()
    {
        Debug.Log("Called HandleCurrencyAdded");
        ChangeVisuals();
    }

    private void ChangeVisuals()
    {
        if (StatsTracker.Instance.ReturnCashInHand() >= upgradeCost)
        {
            imagePiece.GetComponent<MeshRenderer>().material = enoughMaterial;
            particleEffect.SetActive(true);
        }
        else
        {
            imagePiece.GetComponent<MeshRenderer>().material = notEnoughMaterial;
            particleEffect.SetActive(false);
        }
    }

    private void onUpgrade()
    {
        upgradeLevel++;
        StatsTracker.Instance.AdjustCurrency(-upgradeCost);
        upgradeCost = upgradeCost *= 2;
        currentTimeScale += 0.5f;
        Time.timeScale = currentTimeScale;
        ChangeVisuals();
        SavingLoadingManager.Instance.SaveUpgradeLevel(upgradeLevel);
    }

    private void OnApplicationQuit()
    {
        SavingLoadingManager.Instance.SaveUpgradeLevel(upgradeLevel);
    }
}
