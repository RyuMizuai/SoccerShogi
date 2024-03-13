using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    public static List<Vector2Int> goalPosList = new List<Vector2Int>(); // ゴールの位置

    public static void SetGoalPos()
    {
        // ゴールの両端のx座標を求める(ゲームバランスを見て変更する)
        int a = Mathf.CeilToInt(BoardManager.centerPos.x) - 1; // 左端
        int b = Mathf.FloorToInt(BoardManager.centerPos.x) + 1;  // 右端
        Debug.Log(a);
        Debug.Log(b);

        for (int x = a; x <= b; x++)
        {
            // ゴールの座標を設定
            goalPosList.Add(new Vector2Int(x, BoardManager.boardTop + 1));      // 先手のゴール
            goalPosList.Add(new Vector2Int(x, BoardManager.boardBottom - 1));   // 後手のゴール
        }
    }

    // 受け取った座標がゴール位置か判定する
    public static bool GoalExistsAtPos(Vector2 v)
    {
        return goalPosList.Contains(new Vector2Int((int)v.x, (int)v.y));
    }

}
