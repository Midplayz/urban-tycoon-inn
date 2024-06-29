using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using static RoomController;
using TMPro;

public class CleaningTrigger : MonoBehaviour
{
    public float stayTime = 1.0f;
    private float timer = 0.0f;
    private bool playerInside = false;
    private bool playerStaying = false;

    public Image progressBarImage;
    public TextMeshProUGUI progressText;
    public float targetTime = 5.0f; 

    public void OnPlayerEnterFunction()
    {
        progressText.text = "Cleaning Room...";
        progressText.gameObject.SetActive(true);
        SoundManager.Instance.PlayCleaningSound();
        StartProgressBar();
    }

    public void OnPlayerLeaveFunction()
    {
        timer = 0.0f;
        playerInside = false;
        playerStaying = false;
        progressBarImage.fillAmount = 0.0f;
        progressText.gameObject.SetActive(false);
        SoundManager.Instance.StopCleaningSound();
        StopAllCoroutines();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
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
        RoomController roomController = GetComponentInParent<RoomController>();
        roomController.SetRoomState(RoomState.Vacant);
        OnPlayerLeaveFunction();
    }

    public void StartProgressBar()
    {
        StopAllCoroutines();
        StartCoroutine(FillProgressBar());
    }
}
