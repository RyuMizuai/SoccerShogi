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

    public override void CalculateMovePos(List<Vector2Int> pointPosList)
    {
        for (int a = 0; a < 360; a += 45)
        {
            CalculateXY(a, pointPosList);
        }
    }

    public override void CalculateDribblePos(List<Vector2Int> pointPosList)
    {
        CalculateMovePos(pointPosList);
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
