using System;
using UnityEngine;

public static class EventHandler
{
    public static event Action<Vector2Int> SlotClicked;
    public static void CallSlotClicked(Vector2Int pos)
    {
        SlotClicked?.Invoke(pos);
    }
}


