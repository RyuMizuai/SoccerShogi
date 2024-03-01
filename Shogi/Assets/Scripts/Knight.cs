using System.Collections;
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

    public override void CalculateMovePos(List<Vector2Int> pointList)
    {
        if (!isPromoted)
        {
            float[] x = new float[2] { posX + 1, posX - 1 };
            float[] y = new float[2] { posY + 2, posY + 2 };
            for (int i = 0; i < x.Length; i++)
            {
                Vector2 v = GameManager.RotateCoordinate(new Vector2(x[i], y[i]), transform.rotation, transform.position);
                int vx = Mathf.RoundToInt(v.x);
                int vy = Mathf.RoundToInt(v.y);
                pointList.Add(new Vector2Int(vx, vy));
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
