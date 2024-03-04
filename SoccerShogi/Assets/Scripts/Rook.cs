using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece   // 飛車
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
        for (int a = 0; a < 360; a += 90)
        {
            for (int d = 1; d <= boardRight - boardLeft; d++)
            {
                CalculateXY(a, d, pointPosList);
                if (pointPosList.Count == 0) continue; // 動けるマスがなければスキップ

                Vector2Int v = pointPosList[^1];  // 最後に追加された座標
                // 駒かボールがあればそこでストップ
                if (PieceExistsAtPos(v).Item1 || gameManager.BallExistsAtPos(v))
                { 
                    break;
                }
            }
        }
        if (isPromoted)
        {
            for (int a = 45; a < 360; a += 90)
            {
                CalculateXY(a, pointPosList);
            }
        }
    }

    public override void CalculateDribblePos(List<Vector2Int> pointPosList)
    {
        for (int a = 0; a < 360; a += 90)
        {
            CalculateXY(a,pointPosList);    
        }
        if (isPromoted)
        {
            for (int a = 45; a < 360; a += 90)
            {
                CalculateXY(a, pointPosList);
            }
        }
    }
    public override Vector2 GetPieceStandPos()
    {
        return ps.rookPos;
    }

    public override void CountUp()
    {
        ps.rookCount++;
    }

    public override void CountDown()
    {
        ps.rookCount--;
    }

    public override int GetCount()
    {
        return ps.rookCount;
    }

    public override GameObject GetCountText()
    {
        return ps.rookCountText;
    }
}
