using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static RoomController;

public class CheckinTrigger : MonoBehaviour
{
    public float stayTime = 1.0f;
    private float timer = 0.0f;
    private bool playerInside = false;
    private bool playerStaying = false;

    public CustomerSpawning customerSpawning;
    public TextMeshProUGUI progressText;
    public Image progressBarImage; 
    public float targetTime = 5.0f; 

    public void OnPlayerEnterFunction()
    {
        progressText.text = "Checking-in the Customer...";
        progressText.gameObject.SetActive(true);
        SoundManager.Instance.PlayCheckinSound();
        StartProgressBar();
    }

    public void OnPlayerLeaveFunction()
    {
        timer = 0.0f;
        playerInside = false;
        playerStaying = false;
        progressBarImage.fillAmount = 0.0f;
        progressText.gameObject.SetActive(false);
        SoundManager.Instance.StopCheckinSound();
        StopAllCoroutines();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (customerSpawning.VacantRoomAvailable())
            {
                playerInside = true;
            }
            else
            {
                progressText.text = "No Vacant Rooms Available!";
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
        customerSpawning.AssignVacantRoomToCustomer();
        OnPlayerLeaveFunction();
    }

    public void StartProgressBar()
    {
        StopAllCoroutines();
        StartCoroutine(FillProgressBar());
    }
}
