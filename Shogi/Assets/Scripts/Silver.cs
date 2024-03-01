using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Silver : Piece  // ã‚è´
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
        if (!isPromoted)
        {
            int[] i = { 45, 90, 135, 225, 325 };
            foreach (int a in i)
            {
                CalculateXY(a, pointList);
            }
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
        return ps.silverPos;
    }

    public override void CountUp()
    {
        ps.silverCount++;
    }

    public override void CountDown()
    {
        ps.silverCount--;
    }

    public override int GetCount()
    {
        return ps.silverCount;
    }

    public override GameObject GetCountText()
    {
        return ps.silverCountText;
    }
}
