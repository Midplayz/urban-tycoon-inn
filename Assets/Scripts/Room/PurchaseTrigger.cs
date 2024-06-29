using System.Collections;
using UnityEngine;
using static RoomController;
using UnityEngine.UI;
using TMPro;

public class PurchaseTrigger : MonoBehaviour
{
    public float stayTime = 1.0f;
    private float timer = 0.0f;
    private bool playerInside = false;
    private bool playerStaying = false;
    public RoomController roomController;
    public GameObject particleEffect;
    public Material notEnoughMaterial;
    public Material enoughMaterial;
    public GameObject imagePiece;

    public Image progressBarImage;
    public TextMeshProUGUI progressText;
    public float targetTime = 5.0f;

    private void OnDisable()
    {
        if (StatsTracker.Instance != null)
        {
            StatsTracker.Instance.OnCurrencyAdjusted -= HandleCurrencyAdded;
        }
    }
    private void Start()
    {
        StatsTracker.Instance.OnCurrencyAdjusted += HandleCurrencyAdded;
        ChangeVisuals();
    }
    public void OnPlayerEnterFunction()
    {
        progressText.text = "Processing Purchase...";
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
            if (StatsTracker.Instance.ReturnCashInHand() >= roomController.roomUnlockPrice)
            {
                playerInside = true;
            }
            else
            {
                progressText.text = "You need $" + roomController.roomUnlockPrice.ToString() + "!";
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
        roomController.OnUnlock();
        roomController.SetRoomState(RoomState.Vacant);
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
        if (StatsTracker.Instance.ReturnCashInHand() >= roomController.roomUnlockPrice)
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
}
