using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    public static List<Vector2Int> goalPosList = new List<Vector2Int>(); // �S�[���̈ʒu

    public static void SetGoalPos()
    {
        // �S�[���̗��[��x���W�����߂�(�Q�[���o�����X�����ĕύX����)
        int a = Mathf.CeilToInt(BoardManager.centerPos.x) - 1; // ���[
        int b = Mathf.FloorToInt(BoardManager.centerPos.x) + 1;  // �E�[
        Debug.Log(a);
        Debug.Log(b);

        for (int x = a; x <= b; x++)
        {
            // �S�[���̍��W��ݒ�
            goalPosList.Add(new Vector2Int(x, BoardManager.boardTop + 1));      // ���̃S�[��
            goalPosList.Add(new Vector2Int(x, BoardManager.boardBottom - 1));   // ���̃S�[��
        }
    }

    // �󂯎�������W���S�[���ʒu�����肷��
    public static bool GoalExistsAtPos(Vector2 v)
    {
        return goalPosList.Contains(new Vector2Int((int)v.x, (int)v.y));
    }

}
