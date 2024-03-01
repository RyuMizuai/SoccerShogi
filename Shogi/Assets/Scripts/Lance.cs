using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lance : Piece  // 香車
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
            for (int d = 1; d <= boardTop - boardBottom; d++)
            {
                CalculateXY(a, d, pointList);
                if (pointList.Count == 0) continue; // 動けるマスがなければスキップ

                Vector2Int v = pointList[pointList.Count - 1];  // 最後に追加された座標
                // 駒かボールがあればそこでストップ
                if (PieceExistsAtPos(v).Item1 || gameManager.BallExistsAtPos(v))
                {
                    break;
                }
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

    public override Vector2 GetPieceStandPos()
    {
        return ps.lancePos;
    }

    public override void CountUp()
    {
        ps.lanceCount++;
    }

    public override void CountDown()
    {
        ps.lanceCount--;
    }

    public override int GetCount()
    {
        return ps.lanceCount;
    }

    public override GameObject GetCountText()
    {
        return ps.lanceCountText;
    }
}
