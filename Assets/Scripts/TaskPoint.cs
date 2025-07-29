using UnityEngine;

public class TaskPoint : MonoBehaviour
{
    private bool isOccupied = false;
    private GameObject occupier;

    public bool IsOccupied => isOccupied;

    public bool TryOccupy(GameObject who)
    {
        if (isOccupied) return false;

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

