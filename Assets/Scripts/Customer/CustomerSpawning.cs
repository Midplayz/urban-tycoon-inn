using UnityEngine;
using System.Collections.Generic;

public class CustomerSpawning : MonoBehaviour
{
    public Transform spawnPoint;
    public Transform waitingPoint;
    public List<GameObject> customerPrefabs;
    public List<PathDefiner> paths;
    public GameObject checkinCollider;

    public GameObject cashPileObject;
    private List<GameObject> individualCash;

    private List<CustomerController> customers = new List<CustomerController>();
    private CustomerController currentWaitingCustomer;

    private void Start()
    {
        SpawnNextCustomer();
        checkinCollider.SetActive(false);

        individualCash = new List<GameObject>();
        foreach (Transform child in cashPileObject.transform)
        {
            child.gameObject.SetActive(false);
            individualCash.Add(child.gameObject);
        }
    }

    private void Update()
    {

        if (currentWaitingCustomer != null && currentWaitingCustomer.currentState == CustomerController.State.Assigned)
        {
            currentWaitingCustomer = null;
            SpawnNextCustomer();
        }
    }

    private void SpawnNextCustomer()
    {
        if (currentWaitingCustomer != null) return;

        GameObject customerPrefab = customerPrefabs[Random.Range(0, customerPrefabs.Count)];
        GameObject customerObject = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
        CustomerController customerController = customerObject.GetComponent<CustomerController>();

        customerController.checkinCollider = checkinCollider;
        customerController.SetTarget(waitingPoint);
        customerController.SetState(CustomerController.State.MoveToWaiting);

        customers.Add(customerController);
        currentWaitingCustomer = customerController;
    }
    public bool VacantRoomAvailable()
    {
        return RoomManager.Instance.GetVacantRooms() != null;
    }
    public void AssignVacantRoomToCustomer()
    {
        List<RoomController> vacantRooms = RoomManager.Instance.GetVacantRooms();
        if (vacantRooms != null)
        {
            CustomerController customer = currentWaitingCustomer;
            RoomController vacantRoom = vacantRooms[0]; 
            int roomIndex = vacantRoom.roomID; 
            Debug.Log("assigned room " + roomIndex);

            customer.assignedRoom = roomIndex; 
            PathDefiner pathToRoom = RoomManager.Instance.rooms[roomIndex].GetComponent<RoomController>().pathToThisRoom;
            customer.pathDefiner = pathToRoom;

            customer.SetState(CustomerController.State.Assigned);

            currentWaitingCustomer = null; 
            SpawnNextCustomer(); 
            checkinCollider.SetActive(false);
            ActivateLowestInactiveCash();
        }
        else
        {
            Debug.Log("No Vacant Room!");
        }    
    }

    public void ActivateLowestInactiveCash()
    {
        int lowestInactiveIndex = individualCash.FindIndex(go => !go.activeInHierarchy);

        if (lowestInactiveIndex != -1)
        {
            individualCash[lowestInactiveIndex].SetActive(true);
        }
        else
        {
        }
    }
}