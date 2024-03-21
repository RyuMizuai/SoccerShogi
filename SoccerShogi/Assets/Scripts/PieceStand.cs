using UnityEngine;

public class PieceStand : MonoBehaviour
{
    // éùÇøãÓÇÃç¿ïW
    [System.NonSerialized]
    public Vector2 pawnPos,
                   lancePos,
                   knightPos,
                   silverPos,
                   goldPos,
                   bishopPos,
                   rookPos,
                   kingPos;

    // éùÇøãÓÇÃêî
    [System.NonSerialized]
    public int pawnCount,
               lanceCount,
               knightCount,
               silverCount,
               goldCount,
               bishopCount,
               rookCount,
               kingCount;

    // éùÇøãÓÇÃêîÇÃText
    public GameObject pawnCountText,
                      lanceCountText,
                      knightCountText,
                      silverCountText,
                      goldCountText,
                      bishopCountText,
                      rookCountText,
                      kingCountText;

    // ç¿ïWÇ∆countÇÃèâä˙âª
    private void Start()
    {
        pawnPos = new Vector2(10.5f, 1.0f);
        lancePos = new Vector2(10.5f, 2.0f);
        knightPos = new Vector2(10.5f, 3.0f);
        silverPos = new Vector2(10.5f, 4.0f);
        goldPos = new Vector2(10.5f, 5.0f);
        bishopPos = new Vector2(10.5f, 6.0f);
        rookPos = new Vector2(10.5f, 7.0f);
        kingPos = new Vector2(10.5f, 8.0f);

        pawnCount = 0;
        lanceCount = 0;
        knightCount = 0;
        silverCount = 0;
        goldCount = 0;
        bishopCount = 0;
        rookCount = 0;
        kingCount = 0;

        pawnCountText.SetActive(false);
        lanceCountText.SetActive(false);
        knightCountText.SetActive(false);
        silverCountText.SetActive(false);
        goldCountText.SetActive(false);
        bishopCountText.SetActive(false);
        rookCountText.SetActive(false);
        kingCountText.SetActive(false);

    }
}
