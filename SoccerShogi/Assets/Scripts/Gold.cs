using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gold : Piece    // ã‡è´
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
        int[] i = { 0, 45, 90, 135, 180, 270 };
        foreach (int a in i)
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
        return ps.goldPos;
    }

    public override void CountUp()
    {
        ps.goldCount++;
    }

    public override void CountDown()
    {
        ps.goldCount--;
    }

    public override int GetCount()
    {
        return ps.goldCount;
    }

    public override GameObject GetCountText()
    {
        return ps.goldCountText;
    }
}
