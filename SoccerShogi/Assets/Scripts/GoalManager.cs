using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    public static List<Vector2Int> firstGoalPosList = new List<Vector2Int>(); // 先手ゴールの位置
    public static List<Vector2Int> secondGoalPosList = new List<Vector2Int>(); // 後手ゴールの位置

    public static void SetGoalPos()
    {
        // ゴールの両端のx座標を求める(ゲームバランスを見て変更する)
        int a = Mathf.CeilToInt(BoardManager.centerPos.x) - 1;  // 左端
        int b = Mathf.FloorToInt(BoardManager.centerPos.x) + 1; // 右端

        for (int x = a; x <= b; x++)
        {
            // ゴールの座標を設定
            firstGoalPosList.Add(new Vector2Int(x, BoardManager.boardBottom - 1));  // 先手のゴール
            secondGoalPosList.Add(new Vector2Int(x, BoardManager.boardTop + 1));    // 後手のゴール
        }
    }

    // 受け取った座標がゴール位置か判定する
    public static (bool Exists, string Name) GoalExistsAtPos(Vector2 v)
    {
        Vector2Int pos = new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));

        // ゴール判定と，どちらのプレイヤーのゴールかを返す
        if (firstGoalPosList.Contains(pos))
        {
            return (true, GameManager.firstPlayerLayer);
        }
        else if (secondGoalPosList.Contains(pos))
        {
            return (true, GameManager.secondPlayerLayer);
        }
        return (false, null);
    }

}
