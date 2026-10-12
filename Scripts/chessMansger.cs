using System;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

public class ChessManager : MonoBehaviour
{
    // 得到空照片
    [SerializeField] private Sprite Nonephoto;
    // 实例化格子
    [SerializeField] private Square[] slots;
    // 当前阵营 
    public Camp currentCamp;
    private Vector2Int curPos;
    private bool isSelectChess;
    private bool isGameOver;
    /* 单例模式 */
    private static ChessManager instance;
    private ChessManager() { }
    public static ChessManager Instance()
    {
        if (instance == null) instance = new ChessManager();
        return instance;
    }
    /* 单例模式 */

    private void Awake()
    {
        isSelectChess = false;
        curPos = new Vector2Int(-1, -1);
        currentCamp = Camp.Red;
        isGameOver = false;
        InitIndex();
    }
    // 坐标 位置转换
    public Vector2Int GetIndex(int index)
    {
        return new Vector2Int(index % 9, index / 9);
    }
    public int GetNum(Vector2Int pos)
    {
        return pos.y * 9 + pos.x;
    }
    // 初始化
    public void InitIndex()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].index = GetIndex(i);
        }
    }
    // 事件呼叫
    private void OnEnable()
    {
        EventHandler.SlotClicked += OnSlotClicked;
    }
    private void OnDisable()
    {
        EventHandler.SlotClicked -= OnSlotClicked;
    }
    private void OnSlotClicked(Vector2Int pos) // 选择后触发事件
    {
        if (isGameOver) return;
        if (isSelectChess)
        {
            if (pos == curPos)
            {
                isSelectChess = false;
                curPos = new Vector2Int(-1, -1);
                Debug.Log( "已取消点击");
                return;
            }
            if (slots[GetNum(pos)].camp == currentCamp)
            {
                Debug.Log("无法移动到己方棋子上！");
            }
            Debug.Log(curPos);
            MoveChess(pos);
        }
        else
        {

            if (slots[GetNum(pos)].camp == currentCamp)
            {
                curPos = pos;
                isSelectChess = true;
            }
            else
            {
                return;
            }
        }
    }
    private void MoveChess(Vector2Int pos) // 判断当前位置棋子类型并执行移动
    {
        switch (slots[GetNum(curPos)].chessType)
        {
            case ChessType.Check: // 将军
                CheckMove(pos);
                break;
            case ChessType.Soldier: // 兵
                SoldierMove(pos);
                break;
            case ChessType.Satute: // 炮
                SatuteMove(pos);
                break;
            case ChessType.Elephant: // 象
                ElephantMove(pos);
                break;
            case ChessType.Horse: // 马
                HorseMove(pos);
                break;
            case ChessType.Car: // 车
                CarMove(pos);
                break;
            case ChessType.Bachelor: // 士
                BachelorMove(pos);
                break;
        }
    }
    private bool MoveTheChess(Vector2Int movePos) // 将格子移动
    {
        //保存目标格子原有信息
        ChessType targetType = slots[GetNum(movePos)].chessType;
        Camp targetCamp = slots[GetNum(movePos)].camp;

        slots[GetNum(movePos)].camp = slots[GetNum(curPos)].camp;
        slots[GetNum(movePos)].chessType = slots[GetNum(curPos)].chessType;
        slots[GetNum(curPos)].camp = Camp.None;
        slots[GetNum(curPos)].chessType = ChessType.None;
        slots[GetNum(movePos)].icon.GetComponent<Image>().sprite = slots[GetNum(curPos)].icon.GetComponent<Image>().sprite;
        slots[GetNum(curPos)].icon.GetComponent<Image>().sprite = Nonephoto;
        //判断是否吃掉将/帅
        if(targetType == ChessType.Check)
        {
            isGameOver = true;
            Debug.Log($"游戏结束!，{currentCamp}获胜！");
            return true;
        }
        return false;
    }
    // 各类棋子移动
    private void CheckMove(Vector2Int pos) // 将军移动
    {
        if (currentCamp == Camp.Red)
        {
            Debug.Log("当前为红方");
            if (pos.x < 3 || pos.x > 5)
            {
                Debug.Log("帅并不能走出方格！");
                return;
            }
            else if (pos.y > 9 || pos.y < 7)
            {
                Debug.Log("帅并不能走出方格！");
                return;
            }
            else if (pos.x != curPos.x && pos.y != curPos.y)
            {
                Debug.Log("帅不能斜侧移动！");
                return;
            }
            else
            {
                if (pos.x == curPos.x)
                {
                    if (pos.y == curPos.y + 1 || pos.y == curPos.y - 1)
                    {
                        bool ended = MoveTheChess(pos);
                        if (ended) { isSelectChess = false; return; }
                        currentCamp = Camp.Black;
                        isSelectChess = false;
                        return;
                    }
                    Debug.Log("一次只能移动一个");
                    return;
                }
                else if (curPos.y == pos.y)
                {
                    if (pos.x == curPos.x + 1 || pos.x == curPos.x - 1)
                    {
                        bool ended = MoveTheChess(pos);
                        if (ended) { isSelectChess = false; return; }
                        currentCamp = Camp.Black;
                        isSelectChess = false;
                        return;
                    }
                    Debug.Log("一次只能移动一个");
                    return;
                }
            }
        }
        else if (currentCamp == Camp.Black)
        {
            Debug.Log("当前为黑色！");
            if (pos.x < 3 || pos.x > 5)
            {
                Debug.Log("将并不能走出方格！");
                return;
            }
            else if (pos.y > 2 || pos.y < 0)
            {
                Debug.Log("将并不能走出方格！");
                return;
            }
            else if (pos.x != curPos.x && pos.y != curPos.y)
            {
                Debug.Log("将不能斜侧移动！");
                return;
            }
            else
            {
                if (pos.x == curPos.x)
                {
                    if (pos.y == curPos.y + 1 || pos.y == curPos.y - 1)
                    {
                        bool ended = MoveTheChess(pos);
                        if (ended) { isSelectChess = false; return; }
                        currentCamp = Camp.Red;
                        isSelectChess = false;
                        return;
                    }
                    Debug.Log("一次只能移动一个");
                    return;
                }
                else if (curPos.y == pos.y)
                {
                    if (pos.x == curPos.x + 1 || pos.x == curPos.x - 1)
                    {
                        bool ended = MoveTheChess(pos);
                        if (ended) { isSelectChess = false; return; }
                        currentCamp = Camp.Red;
                        isSelectChess = false;
                        return;
                    }
                    Debug.Log("一次只能移动一个");
                    return;
                }
            }
        }
    }
    private void SoldierMove(Vector2Int pos) // 小兵移动
    {
        if (currentCamp == Camp.Red)
        {
            if (curPos.y > 4)
            {
                if (pos.x == curPos.x && pos.y == curPos.y - 1)
                {
                    bool ended = MoveTheChess(pos);
                    if (ended) { isSelectChess = false; return; }
                    isSelectChess = false;
                    currentCamp = Camp.Black;
                }
                else
                {
                    Debug.Log("兵在过河前只能前进");
                }
                return;
            }
            else
            {
                if (pos.y > curPos.y)
                {
                    Debug.Log("兵无法向后移动");
                    return;
                }
                else if (pos.x != curPos.x && pos.y != curPos.y)
                {
                    Debug.Log("兵无法斜侧移动");
                    return;
                }
                else
                {
                    if (pos.x == curPos.x)
                    {
                        if (pos.y == curPos.y - 1)
                        {
                            bool ended = MoveTheChess(pos);
                            if (ended) { isSelectChess = false; return; }
                            isSelectChess = false;
                            currentCamp = Camp.Black;
                            return;
                        }
                    }
                    else if (pos.y == curPos.y)
                    {
                        if (pos.x == curPos.x + 1 || pos.x == curPos.x - 1)
                        {
                            bool ended = MoveTheChess(pos);
                            if (ended) { isSelectChess = false; return; }
                            isSelectChess = false;
                            currentCamp = Camp.Black;
                            return;
                        }
                    }
                }
            }
        }
        else if (currentCamp == Camp.Black)
        {
            if (curPos.y < 5)
            {
                if (pos.x == curPos.x && pos.y == curPos.y + 1)
                {
                    bool ended = MoveTheChess(pos);
                    if (ended) { isSelectChess = false; return; }
                    isSelectChess = false;
                    currentCamp = Camp.Red;
                }
                else
                {
                    Debug.Log("卒在过河前只能前进");
                }
                return;
            }
            else
            {
                if (pos.y < curPos.y)
                {
                    Debug.Log("卒无法向后移动");
                    return;
                }
                else if (pos.x != curPos.x && pos.y != curPos.y)
                {
                    Debug.Log("卒无法斜侧移动");
                    return;
                }
                else
                {
                    if (pos.x == curPos.x)
                    {
                        if (pos.y == curPos.y + 1)
                        {
                            bool ended = MoveTheChess(pos);
                            if (ended) { isSelectChess = false; return; }
                            isSelectChess = false;
                            currentCamp = Camp.Red;
                            return;
                        }
                    }
                    else if (pos.y == curPos.y)
                    {
                        if (pos.x == curPos.x + 1 || pos.x == curPos.x - 1)
                        {
                            bool ended = MoveTheChess(pos);
                            if (ended) { isSelectChess = false; return; }
                            isSelectChess = false;
                            currentCamp = Camp.Red;
                            return;
                        }
                    }
                }
            }
        }
    }
    private void SatuteMove(Vector2Int pos) // 炮移动
    {
        if (pos.x != curPos.x && pos.y != curPos.y)
        {
            Debug.Log("炮不能斜侧移动");
            return;
        }
        else if (pos.x == curPos.x)
        {
            int ma = Math.Max(pos.y, curPos.y);
            int mi = Math.Min(pos.y, curPos.y);
            Debug.Log(ma + " " + mi);
            int num = 0;
            for (int i = mi + 1; i < ma; i++)
            {
                Vector2Int temp = new Vector2Int(curPos.x, i);
                if (slots[GetNum(temp)].camp != Camp.None)
                {
                    num++;
                }
            }
            if (num > 1)
            {
                Debug.Log("炮不能跳过两个以上棋子");
                return;
            }
            if (num == 1)
            {
                if (currentCamp == Camp.Red)
                {
                    if (slots[GetNum(pos)].camp == Camp.Black)
                    {
                        bool ended = MoveTheChess(pos);
                        if (ended) { isSelectChess = false; return; }
                        currentCamp = Camp.Black;
                        isSelectChess = false;
                        return;
                    }
                    Debug.Log("无法移动到当前位置");
                }
                if (currentCamp == Camp.Black)
                {
                    if (slots[GetNum(pos)].camp == Camp.Red)
                    {
                        bool ended = MoveTheChess(pos);
                        if (ended) { isSelectChess = false; return; }
                        currentCamp = Camp.Red;
                        isSelectChess = false;
                        return;
                    }
                    Debug.Log("无法移动到当前位置");
                }
            }
            if (num == 0 && slots[GetNum(pos)].camp == Camp.None)
            {
                bool ended = MoveTheChess(pos);
                if (ended) { isSelectChess = false; return; }
                if (currentCamp == Camp.Red) currentCamp = Camp.Black;
                else if (currentCamp == Camp.Black) currentCamp = Camp.Red;
                isSelectChess = false;
                return;
            }
        }
        else if (curPos.y == pos.y)
        {
            int ma = Math.Max(pos.x, curPos.x);
            int mi = Math.Min(pos.x, curPos.x);
            Debug.Log(ma + " " + mi);
            int num = 0;
            for (int i = mi + 1; i < ma; i++)
            {
                Vector2Int temp = new Vector2Int(i, curPos.y);
                if (slots[GetNum(temp)].camp != Camp.None)
                {
                    num++;
                }
            }
            Debug.Log("炮跳过" + num);
            if (num > 1)
            {
                Debug.Log("炮不能跳过两个以上棋子");
                return;
            }
            if (num == 1)
            {
                if (currentCamp == Camp.Red)
                {
                    if (slots[GetNum(pos)].camp == Camp.Black)
                    {
                        bool ended = MoveTheChess(pos);
                        if (ended) { isSelectChess = false; return; }
                        currentCamp = Camp.Black;
                        isSelectChess = false;
                        return;
                    }
                    Debug.Log("无法移动到当前位置");
                }
                if (currentCamp == Camp.Black)
                {
                    if (slots[GetNum(pos)].camp == Camp.Red)
                    {
                        bool ended = MoveTheChess(pos);
                        if (ended) { isSelectChess = false; return; }
                        currentCamp = Camp.Red;
                        isSelectChess = false;
                        return;
                    }
                    Debug.Log("无法移动到当前位置");
                }
            }
            if (num == 0 && slots[GetNum(pos)].camp == Camp.None)
            {
                bool ended = MoveTheChess(pos);
                if (ended) { isSelectChess = false; return; }
                if (currentCamp == Camp.Red) currentCamp = Camp.Black;
                else if (currentCamp == Camp.Black) currentCamp = Camp.Red;
                isSelectChess = false;
                return;
            }
        }
    }
    private void CarMove(Vector2Int pos) // 车移动
    {
        if (pos.x != curPos.x && pos.y != curPos.y)
        {
            Debug.Log("车不能斜向移动");
            return;
        }
        else if (pos.x == curPos.x)
        {
            int ma = Math.Max(pos.y, curPos.y);
            int mi = Math.Min(pos.y, curPos.y);
            int num = 0;
            for (int i = mi + 1; i < ma; i++)
            {
                Vector2Int temp = new Vector2Int(curPos.x, i);
                if (slots[GetNum(temp)].camp != Camp.None)
                {
                    num++;
                }
            }
            if (num == 0)
            {
                if (currentCamp == Camp.Red && slots[GetNum(pos)].camp != currentCamp)
                {
                    bool ended = MoveTheChess(pos);
                    if (ended) { isSelectChess = false; return; }
                    isSelectChess = false;
                    currentCamp = Camp.Black;
                    return;
                }
                if (currentCamp == Camp.Black && slots[GetNum(pos)].camp != currentCamp)
                {
                    bool ended = MoveTheChess(pos);
                    if (ended) { isSelectChess = false; return; }
                    isSelectChess = false;
                    currentCamp = Camp.Red;
                    return;
                }
                return;
            }
            return;
        }
        else if (pos.y == curPos.y)
        {
            int ma = Math.Max(pos.x, curPos.x);
            int mi = Math.Min(pos.x, curPos.x);
            int num = 0;
            for (int i = mi + 1; i < ma; i++)
            {
                Vector2Int temp = new Vector2Int(i, curPos.y);
                if (slots[GetNum(temp)].camp != Camp.None)
                {
                    num++;
                }
            }
            if (num == 0)
            {
                if (currentCamp == Camp.Red && slots[GetNum(pos)].camp != currentCamp)
                {
                    bool ended = MoveTheChess(pos);
                    if (ended) { isSelectChess = false; return; }
                    isSelectChess = false;
                    currentCamp = Camp.Black;
                    return;
                }
                if (currentCamp == Camp.Black && slots[GetNum(pos)].camp != currentCamp)
                {
                    bool ended = MoveTheChess(pos);
                    if (ended) { isSelectChess = false; return; }
                    isSelectChess = false;
                    currentCamp = Camp.Red;
                    return;
                }
                return;
            }
            return;
        }
    }
    private void ElephantMove(Vector2Int pos) // 象移动
    {
        if (slots[GetNum(pos)].camp != currentCamp)
        {
            if (pos.x == curPos.x + 2 && pos.y == curPos.y + 2)
            {
                if (pos.x == curPos.x + 1 && pos.y == curPos.y + 1)
                {
                    Debug.Log("堵象眼");
                    return;
                }

                if (currentCamp == Camp.Red && pos.y > 4)
                {
                    bool ended = MoveTheChess(pos);
                    if (ended) { isSelectChess = false; return; }
                    isSelectChess = false;
                    currentCamp = Camp.Black;
                    return;
                }
                else if (currentCamp == Camp.Black && pos.y < 5)
                {
                    bool ended = MoveTheChess(pos);
                    if (ended) { isSelectChess = false; return; }
                    isSelectChess = false;
                    currentCamp = Camp.Red;
                    return;
                }
                else
                {
                    Debug.Log("不合法");
                }
            }
            if (pos.x == curPos.x - 2 && pos.y == curPos.y - 2)
            {
                if (pos.x == curPos.x - 1 && pos.y == curPos.y - 1)
                {
                    Debug.Log("堵象眼");
                    return;
                }
                if (currentCamp == Camp.Red && pos.y > 4)
                {
                    bool ended = MoveTheChess(pos);
                    if (ended) { isSelectChess = false; return; }
                    isSelectChess = false;
                    currentCamp = Camp.Black;
                    return;
                }
                else if (currentCamp == Camp.Black && pos.y < 5)
                {
                    bool ended = MoveTheChess(pos);
                    if (ended) { isSelectChess = false; return; }
                    isSelectChess = false;
                    currentCamp = Camp.Red;
                    return;
                }
                else
                {
                    Debug.Log("不合法");
                }
            }
            if (pos.x == curPos.x - 2 && pos.y == curPos.y + 2)
            {
                if (pos.x == curPos.x - 1 && pos.y == curPos.y + 1)
                {
                    Debug.Log("堵象眼");
                    return;
                }
                if (currentCamp == Camp.Red && pos.y > 4)
                {
                    bool ended = MoveTheChess(pos);
                    if (ended) { isSelectChess = false; return; }
                    isSelectChess = false;
                    currentCamp = Camp.Black;
                    return;
                }
                else if (currentCamp == Camp.Black && pos.y < 5)
                {
                    bool ended = MoveTheChess(pos);
                    if (ended) { isSelectChess = false; return; }
                    isSelectChess = false;
                    currentCamp = Camp.Red;
                    return;
                }
                else
                {
                    Debug.Log("不合法");
                }
            }
            if (pos.x == curPos.x + 2 && pos.y == curPos.y - 2)
            {
                if (pos.x == curPos.x + 1 && pos.y == curPos.y - 1)
                {
                    Debug.Log("堵象眼");
                    return;
                }
                if (currentCamp == Camp.Red && pos.y > 4)
                {
                    bool ended = MoveTheChess(pos);
                    if (ended) { isSelectChess = false; return; }
                    isSelectChess = false;
                    currentCamp = Camp.Black;
                    return;
                }
                else if (currentCamp == Camp.Black && pos.y < 5)
                {
                    bool ended = MoveTheChess(pos);
                    if (ended) { isSelectChess = false; return; }
                    isSelectChess = false;
                    currentCamp = Camp.Red;
                    return;
                }
                else
                {
                    Debug.Log("不合法");
                }
            }
        }
    }
    private void HorseMove(Vector2Int pos) // 马移动
    {
        Debug.Log(pos + " " + curPos);
        if (pos.x == curPos.x + 1 && pos.y == curPos.y + 2)
        {
            Vector2Int front = new Vector2Int(curPos.x, curPos.y + 1);
            Debug.Log(front);
            if (slots[GetNum(front)].camp != Camp.None)
            {
                Debug.Log("堵马脚");
                return;
            }
            if (slots[GetNum(pos)].camp != currentCamp)
            {
                bool ended = MoveTheChess(pos);
                if (ended) { isSelectChess = false; return; }
                isSelectChess = false;
                if (currentCamp == Camp.Red) currentCamp = Camp.Black;
                else if (currentCamp == Camp.Black) currentCamp = Camp.Red;
                return;
            }
        }
        if (pos.x == curPos.x - 1 && pos.y == curPos.y - 2)
        {
            Vector2Int front = new Vector2Int(curPos.x, curPos.y - 1);
            Debug.Log(front);
            if (slots[GetNum(front)].camp != Camp.None)
            {
                Debug.Log("堵马脚");
                return;
            }
            if (slots[GetNum(pos)].camp != currentCamp)
            {
                bool ended = MoveTheChess(pos);
                if (ended) { isSelectChess = false; return; }
                isSelectChess = false;
                if (currentCamp == Camp.Red) currentCamp = Camp.Black;
                else if (currentCamp == Camp.Black) currentCamp = Camp.Red;
                return;
            }
        }
        if (pos.x == curPos.x - 1 && pos.y == curPos.y + 2)
        {
            Vector2Int front = new Vector2Int(curPos.x, curPos.y + 1);
            Debug.Log(front);
            if (slots[GetNum(front)].camp != Camp.None)
            {
                Debug.Log("堵马脚");
                return;
            }
            if (slots[GetNum(pos)].camp != currentCamp)
            {
                bool ended = MoveTheChess(pos);
                if (ended) { isSelectChess = false; return; }
                isSelectChess = false;
                if (currentCamp == Camp.Red) currentCamp = Camp.Black;
                else if (currentCamp == Camp.Black) currentCamp = Camp.Red;
                return;
            }
        }
        if (pos.x == curPos.x + 1 && pos.y == curPos.y - 2)
        {
            Vector2Int front = new Vector2Int(curPos.x, curPos.y - 1);
            Debug.Log(front);
            if (slots[GetNum(front)].camp != Camp.None)
            {
                Debug.Log("堵马脚");
                return;
            }
            if (slots[GetNum(pos)].camp != currentCamp)
            {
                bool ended = MoveTheChess(pos);
                if (ended) { isSelectChess = false; return; }
                isSelectChess = false;
                if (currentCamp == Camp.Red) currentCamp = Camp.Black;
                else if (currentCamp == Camp.Black) currentCamp = Camp.Red;
                return;
            }
        }

        if (pos.x == curPos.x + 2 && pos.y == curPos.y + 1)
        {
            Vector2Int front = new Vector2Int(curPos.x + 1, curPos.y);
            Debug.Log(front);
            if (slots[GetNum(front)].camp != Camp.None)
            {
                Debug.Log("堵马脚");
                return;
            }
            if (slots[GetNum(pos)].camp != currentCamp)
            {
                bool ended = MoveTheChess(pos);
                if (ended) { isSelectChess = false; return; }
                isSelectChess = false;
                if (currentCamp == Camp.Red) currentCamp = Camp.Black;
                else if (currentCamp == Camp.Black) currentCamp = Camp.Red;
                return;
            }
        }
        if (pos.x == curPos.x - 2 && pos.y == curPos.y - 1)
        {
            Vector2Int front = new Vector2Int(curPos.x - 1, curPos.y);
            Debug.Log(front);
            if (slots[GetNum(front)].camp != Camp.None)
            {
                Debug.Log("堵马脚");
                return;
            }
            if (slots[GetNum(pos)].camp != currentCamp)
            {
                bool ended = MoveTheChess(pos);
                if (ended) { isSelectChess = false; return; }
                isSelectChess = false;
                if (currentCamp == Camp.Red) currentCamp = Camp.Black;
                else if (currentCamp == Camp.Black) currentCamp = Camp.Red;
                return;
            }
        }
        if (pos.x == curPos.x - 2 && pos.y == curPos.y + 1)
        {
            Vector2Int front = new Vector2Int(curPos.x - 1, curPos.y);
            Debug.Log(front);
            if (slots[GetNum(front)].camp != Camp.None)
            {
                Debug.Log("堵马脚");
                return;
            }
            if (slots[GetNum(pos)].camp != currentCamp)
            {
                bool ended = MoveTheChess(pos);
                if (ended) { isSelectChess = false; return; }
                isSelectChess = false;
                if (currentCamp == Camp.Red) currentCamp = Camp.Black;
                else if (currentCamp == Camp.Black) currentCamp = Camp.Red;
                return;
            }
        }
        if (pos.x == curPos.x + 2 && pos.y == curPos.y - 1)
        {
            Vector2Int front = new Vector2Int(curPos.x + 1, curPos.y);
            Debug.Log(front);
            if (slots[GetNum(front)].camp != Camp.None)
            {
                Debug.Log("堵马脚");
                return;
            }
            if (slots[GetNum(pos)].camp != currentCamp)
            {
                bool ended = MoveTheChess(pos);
                if (ended) { isSelectChess = false; return; }
                isSelectChess = false;
                if (currentCamp == Camp.Red) currentCamp = Camp.Black;
                else if (currentCamp == Camp.Black) currentCamp = Camp.Red;
                return;
            }
        }
    }
    private void BachelorMove(Vector2Int pos) // 士移动
    {
        if (currentCamp == Camp.Red)
        {
            if (pos.x < 3 || pos.x > 5)
            {
                Debug.Log("士只能在方格内走！");
                return;
            }
            if (pos.y <= 6)
            {
                Debug.Log("士只能在方格内走！");
                return;
            }

            if (pos.x == curPos.x + 1 && pos.y == curPos.y + 1)
            {
                bool ended = MoveTheChess(pos);
                if (ended) { isSelectChess = false; return; }
                if (currentCamp == Camp.Red) currentCamp = Camp.Black;
                else if (currentCamp == Camp.Black) currentCamp = Camp.Red;
                isSelectChess = false;
                return;
            }
            if (pos.x == curPos.x - 1 && pos.y == curPos.y - 1)
            {
                bool ended = MoveTheChess(pos);
                if (ended) { isSelectChess = false; return; }
                if (currentCamp == Camp.Red) currentCamp = Camp.Black;
                else if (currentCamp == Camp.Black) currentCamp = Camp.Red;
                isSelectChess = false;
                return;
            }
            if (pos.x == curPos.x - 1 && pos.y == curPos.y + 1)
            {
                bool ended = MoveTheChess(pos);
                if (ended) { isSelectChess = false; return; }
                if (currentCamp == Camp.Red) currentCamp = Camp.Black;
                else if (currentCamp == Camp.Black) currentCamp = Camp.Red;
                isSelectChess = false;
                return;
            }
            if (pos.x == curPos.x + 1 && pos.y == curPos.y - 1)
            {
                bool ended = MoveTheChess(pos);
                if (ended) { isSelectChess = false; return; }
                if (currentCamp == Camp.Red) currentCamp = Camp.Black;
                else if (currentCamp == Camp.Black) currentCamp = Camp.Red;
                isSelectChess = false;
                return;
            }
            return;
        }
        if (currentCamp == Camp.Black)
        {
            if (pos.x < 3 || pos.x > 5)
            {
                Debug.Log("士只能在方格内走！");
                return;
            }
            if (pos.y > 2)
            {
                Debug.Log("士只能在方格内走！");
                return;
            }
            if (pos.x == curPos.x + 1 && pos.y == curPos.y + 1)
            {
                bool ended = MoveTheChess(pos);
                if (ended) { isSelectChess = false; return; }
                if (currentCamp == Camp.Red) currentCamp = Camp.Black;
                else if (currentCamp == Camp.Black) currentCamp = Camp.Red;
                isSelectChess = false;
                return;
            }
            if (pos.x == curPos.x - 1 && pos.y == curPos.y - 1)
            {
                bool ended = MoveTheChess(pos);
                if (ended) { isSelectChess = false; return; }
                if (currentCamp == Camp.Red) currentCamp = Camp.Black;
                else if (currentCamp == Camp.Black) currentCamp = Camp.Red;
                isSelectChess = false;
                return;
            }
            if (pos.x == curPos.x - 1 && pos.y == curPos.y + 1)
            {
                bool ended = MoveTheChess(pos);
                if (ended) { isSelectChess = false; return; }
                if (currentCamp == Camp.Red) currentCamp = Camp.Black;
                else if (currentCamp == Camp.Black) currentCamp = Camp.Red;
                isSelectChess = false;
                return;
            }
            if (pos.x == curPos.x + 1 && pos.y == curPos.y - 1)
            {
                bool ended = MoveTheChess(pos);
                if (ended) { isSelectChess = false; return; }
                if (currentCamp == Camp.Red) currentCamp = Camp.Black;
                else if (currentCamp == Camp.Black) currentCamp = Camp.Red;
                isSelectChess = false;
                return;
            }
            return;
        }
    }


}


