using UnityEngine;
using System.Collections;

public class CustomerController : MonoBehaviour
{
    public enum State { MoveToWaiting, Waiting, Assigned };
    public int assignedRoom;
    public State currentState;
    public GameObject checkinCollider;

    private Transform target;
    private float moveSpeed = 20.0f;

    public PathDefiner pathDefiner;
    private int currentWaypointIndex = 0;

    private void Update()
    {
        if (currentState == State.MoveToWaiting)
        {
            MoveToTarget();
        }
    }

    public void SetState(State newState)
    {
        currentState = newState;

        if (currentState == State.Assigned)
        {
            currentWaypointIndex = 0; 
            if (pathDefiner != null)
            {
                target = pathDefiner.waypoints[currentWaypointIndex];
            }
            StartCoroutine(MoveToAssignedRoom());
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private void MoveToTarget()
    {
        if (target == null) return;

        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            SetState(State.Waiting);
            SoundManager.Instance.PlayReceptionBellSound();
            checkinCollider.SetActive(true);
        }
    }

    private IEnumerator MoveToAssignedRoom()
    {
        RoomController room = RoomManager.Instance.rooms[assignedRoom];
        room.SetRoomState(RoomController.RoomState.Occupied);

        while (currentWaypointIndex < pathDefiner.waypoints.Length)
        {
            Vector3 target = pathDefiner.waypoints[currentWaypointIndex].position;

            while (Vector3.Distance(transform.position, target) > 0.1f)
            {
                Vector3 direction = (target - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);

                lookRotation *= Quaternion.Euler(0, 180, 0);

                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * moveSpeed);

                transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

                yield return null;
            }

            currentWaypointIndex++;
        }

        if (currentWaypointIndex == pathDefiner.waypoints.Length)
        {
            Vector3 lastTarget = pathDefiner.waypoints[currentWaypointIndex - 1].position;

            while (Vector3.Distance(transform.position, lastTarget) > 0.1f)
            {
                Vector3 direction = (lastTarget - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);

                lookRotation *= Quaternion.Euler(0, 180, 0);

                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * moveSpeed);

                transform.position = Vector3.MoveTowards(transform.position, lastTarget, moveSpeed * Time.deltaTime);

                yield return null;
            }
        }

        room.OnOccupiedActions();
        StartCoroutine(room.StartOccupiedSequence(this.gameObject));
    }


}
