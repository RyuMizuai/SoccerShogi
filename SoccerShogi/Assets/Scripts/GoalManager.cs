using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalManager : MonoBehaviour
{
    public static List<Vector2Int> firstGoalPosList = new List<Vector2Int>(); // ���S�[���̈ʒu
    public static List<Vector2Int> secondGoalPosList = new List<Vector2Int>(); // ���S�[���̈ʒu

    public static void SetGoalPos()
    {
        // �S�[���̗��[��x���W�����߂�(�Q�[���o�����X�����ĕύX����)
        int a = Mathf.CeilToInt(BoardManager.centerPos.x) - 1;  // ���[
        int b = Mathf.FloorToInt(BoardManager.centerPos.x) + 1; // �E�[

        for (int x = a; x <= b; x++)
        {
            // �S�[���̍��W��ݒ�
            firstGoalPosList.Add(new Vector2Int(x, BoardManager.boardBottom - 1));  // ���̃S�[��
            secondGoalPosList.Add(new Vector2Int(x, BoardManager.boardTop + 1));    // ���̃S�[��
        }
    }

    // �󂯎�������W���S�[���ʒu�����肷��
    public static (bool Exists, string Name) GoalExistsAtPos(Vector2 v)
    {
        Vector2Int pos = new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));

        // �S�[������ƁC�ǂ���̃v���C���[�̃S�[������Ԃ�
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
