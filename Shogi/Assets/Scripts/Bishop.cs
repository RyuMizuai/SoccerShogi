using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece // �p�s
{
    protected override void Init()
    {
        stuckPosY = boardTop + 1;
    }

    public override void Set(Vector2 pos)
    {
        posX = pos.x;
        posY = pos.y;
    }

    public override void CalculateMovePos(List<Vector2Int> pointList)
    {
        for (int a = 45; a < 360; a += 90)
        {
            for (int d = 1; d <= boardRight - boardLeft; d++)
            {
                CalculateXY(a, Mathf.Sqrt(2) * d, pointList); // �΂߂̓����̓��[�g2�Œ���
                if (pointList.Count == 0) continue; // ������}�X���Ȃ���΃X�L�b�v

                Vector2Int v = pointList[pointList.Count - 1];  // �Ō�ɒǉ����ꂽ���W
                // ��{�[��������΂����ŃX�g�b�v
                if (PieceExistsAtPos(v).Item1 || gameManager.BallExistsAtPos(v))
                {
                    break;
                }
            }
        }
        if (isPromoted)
        {
            for (int a = 0; a < 360; a += 90)
            {
                CalculateXY(a, pointList);
            }
        }
    }

    public override void CalculateDribblePos(List<Vector2Int> pointList)
    {
        for (int a = 45; a < 360; a += 90)
        {
            CalculateXY(a, Mathf.Sqrt(2), pointList); // �΂߂̓����̓��[�g2�Œ���
        }
        if (isPromoted)
        {
            for (int a = 0; a < 360; a += 90)
            {
                CalculateXY(a, pointList);
            }
        }
    }

    public override Vector2 GetPieceStandPos()
    {
        return ps.bishopPos;
    }

    public override void CountUp()
    {
        ps.bishopCount++;
    }

    public override void CountDown()
    {
        ps.bishopCount--;
    }

    public override int GetCount()
    {
        return ps.bishopCount;
    }

    public override GameObject GetCountText()
    {
        return ps.bishopCountText;
    }
}