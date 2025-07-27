using UnityEngine;

public class TaskPoint : MonoBehaviour
{
    public static TaskPoint Instance;

    private bool isOccupied = false;
    private GameObject occupier;

    public bool IsOccupied => isOccupied;

    public string roomName;

    public bool TryOccupy(GameObject who)
    {
        if (isOccupied) return false;

        Instance = this;

        isOccupied = true;
        occupier = who;
        return true;
    }

    public void Release(GameObject who)
    {
        if (isOccupied && occupier == who)
        {
            isOccupied = false;
            occupier = null;
        }
    }
}

