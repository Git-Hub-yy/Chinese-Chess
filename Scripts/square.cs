using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;


public class Square : MonoBehaviour
{
    public Camp camp;
    public ChessType chessType;
    public GameObject Select;
    public GameObject icon;
    public Vector2Int index;

    public void SlotOnClicked()
    {
        EventHandler.CallSlotClicked(index);
    }
}

