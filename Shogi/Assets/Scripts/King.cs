using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece   // â§è´
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
        for (int a = 0; a < 360; a = a + 45)
        {
            CalculateXY(a, pointList);
        }
    }

    public override void CalculateDribblePos(List<Vector2Int> pointList)
    {
        CalculateMovePos(pointList);
    }

    public override Vector2 GetPieceStandPos()
    {
        return ps.kingPos;
    }

    public override void CountUp()
    {
        ps.kingCount++;
    }

    public override void CountDown()
    {
        ps.kingCount--;
    }

    public override int GetCount()
    {
        return ps.kingCount;
    }

    public override GameObject GetCountText()
    {
        return ps.kingCountText;
    }
}
