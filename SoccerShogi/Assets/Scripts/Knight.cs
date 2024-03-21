using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece // Œj”n
{
    protected override void Init()
    {
        stuckPosY = boardTop - 1;
    }

    public override void Set(Vector2 pos)
    {
        posX = pos.x;
        posY = pos.y;
    }

    public override void CalculateMovePos(List<Vector2Int> pointPosList)
    {
        if (!isPromoted)
        {
            Vector2[] vArray = new Vector2[]{ new Vector2(posX + 1, posY + 2), 
                new Vector2(posX - 1, posY + 2) };

            foreach (Vector2 vInt in vArray)
            {
                Vector2 v = GameManager.RotateCoordinate(vInt, transform.rotation, transform.position);
                int vx = Mathf.RoundToInt(v.x);
                int vy = Mathf.RoundToInt(v.y);
                pointPosList.Add(new Vector2Int(vx, vy));
            }
        }
        else
        {
            int[] i = { 0, 45, 90, 135, 180, 270 };
            foreach (int a in i)
            {
                CalculateXY(a, pointPosList);
            }
        }
    }

    public override void CalculateDribblePos(List<Vector2Int> pointPosList)
    {
        CalculateMovePos(pointPosList);
    }

    public override Vector2 GetPieceStandPos()
    {
        return ps.knightPos;
    }

    public override void CountUp()
    {
        ps.knightCount++;
    }

    public override void CountDown()
    {
        ps.knightCount--;
    }

    public override int GetCount()
    {
        return ps.knightCount;
    }

    public override GameObject GetCountText()
    {
        return ps.knightCountText;
    }
}
