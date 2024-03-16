using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUManager : MonoBehaviour
{
    public static List<Member> CPUPos = new List<Member>();

    public static void MovingCPU()
    {
        int count = CPUPos.Count;
        System.Random random = new System.Random();
        int rndIndex = random.Next(0, count - 1);
        PieceController pc = CPUPos[rndIndex].Object.GetComponent<PieceController>();
        pc.MovePiece(CPUPos[rndIndex].Position);
    }

}

public class Member
{
    public GameObject Object { get; set; }
    public Vector2Int Position { get; set; }

    public Member(GameObject gameObject, Vector2Int position)
    {
        Object = gameObject;
        Position = position;
    }
}