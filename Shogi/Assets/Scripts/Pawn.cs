using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece   // ����
{
    protected override void Init()
    {
        stuckPosY = boardTop;
    }

    public override void Set(Vector2 pos)
    {
        posX = pos.x;
        posY = pos.y;
    }

    public override void CalculateMovePos(List<Vector2Int> pointList)
    {
        if (!isPromoted)
        {
            int a = 90;
            CalculateXY(a, pointList);
        }
        else
        {
            int[] i = { 0, 45, 90, 135, 180, 270 };
            foreach (int a in i)
            {
                CalculateXY(a, pointList);
            }
        }
    }

    public override void CalculateDribblePos(List<Vector2Int> pointList)
    {
        CalculateMovePos(pointList);
    }

    public override Vector2 GetPieceStandPos()
    {
        return ps.pawnPos;
    }

    public override void CountUp()
    {
        ps.pawnCount++;
    }

    public override void CountDown()
    {
        ps.pawnCount--;
    }

    public override int GetCount()
    {
        return ps.pawnCount;
    }

    public override GameObject GetCountText()
    {
        return ps.pawnCountText;
    }
}