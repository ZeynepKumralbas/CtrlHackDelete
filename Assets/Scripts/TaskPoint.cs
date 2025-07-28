using UnityEngine;

public class TaskPoint : MonoBehaviour
{
    private GameObject currentOccupant = null;

    public bool TryOccupy(GameObject npc)
    {
        // Eğer boşsa bu NPC alsın
        if (currentOccupant == null)
        {
            currentOccupant = npc;
            Debug.Log($"{npc.name} -> {gameObject.name} görevini aldı.");
            return true;
        }

        // Zaten bu NPC geldiyse, tekrar izin ver
        if (currentOccupant == npc)
        {
            return true;
        }

        // Doluysa reddet
        return false;
    }

    public void Release(GameObject npc)
    {
        if (currentOccupant == npc)
        {
            Debug.Log($"{npc.name} -> {gameObject.name} görevini bıraktı.");
            currentOccupant = null;
        }
    }

    public bool IsOccupied()
    {
        return currentOccupant != null;
    }
}
