using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    protected int boardLeft = 1;     // ���[
    protected int boardRight = 9;    // �E�[
    protected int boardBottom = 1;   // ���[
    protected int boardTop = 9;      // ��[

    public Sprite pieceSprite;          // ��̉摜
    public Sprite promotedPieceSprite;  // ����̉摜

    protected float posX;       // ���x���W��ۊǂ���
    protected float posY;       // ���y���W��ۊǂ���
    protected int stuckPosY;    // �����Ȃ��Ȃ�}�X�̍��W

    protected PieceStand ps;                // PieceStand�̓��ꕨ
    private GameObject[] pieces;          // ���ׂĂ̋�̃Q�[���I�u�W�F�N�g

    private string firstPlayerLayer = "FirstPlayer";  //First Player���C���[
    private string secondPlayerLayer = "SecondPlayer";    // SecondPlayer���C���[
    private string thisLayer = "";          // ��̃��C���[

    [System.NonSerialized]
    public bool isPromoted = false; // �����Ă��邩




    abstract protected void Init(); // �������p���\�b�h

    // Set���\�b�h
    abstract public void Set(Vector2 pos);

    // ���������W���v�Z����
    abstract public void CalculateMovePos(List<Vector2Int> pointPosList);

    // �h���u���œ�������W���v�Z����
    abstract public void CalculateDribblePos(List<Vector2Int> pointPosList);

    // ���̍��W��Ԃ�
    abstract public Vector2 GetPieceStandPos();

    // �J�E���g�𑝂₷
    abstract public void CountUp();
    // �J�E���g�����炷
    abstract public void CountDown();

    // ������̐����󂯎��
    abstract public int GetCount();
    // ������̐���Text���󂯎��
    abstract public GameObject GetCountText();



    private void Awake()
    {
        // ������
        Init();
        
    }

    private void Start()
    {
        boardLeft = BoardManager.boardLeft;
        boardRight = BoardManager.boardRight;
        boardBottom = BoardManager.boardBottom;
        boardTop = BoardManager.boardTop;

        SetPieceStand();
        StartCoroutine(Coroutine());
    }

    private IEnumerator Coroutine()
    {
        // GameManager�̏����������܂ő҂�
        yield return new WaitUntil(() => GameManager.isFinishInitialize);
        pieces = GameManager.gameManager.pieces;    // ����擾
    }


    // �w��̍��W�ɑ��̋���邩�𔻒肷��
    public (bool, GameObject) PieceExistsAtPos(Vector2Int v)
    {
        foreach (GameObject piece in pieces)
        {
            if (piece == this.gameObject) continue; // ���g�Ɠ����Ȃ�X�L�b�v

            // ���W����v������true�Ƌ�̃I�u�W�F�N�g��Ԃ�
            if (GameManager.TwoPositionsEquals(piece.transform.position, v))
            {
                return (true, piece);
            }
        }
        return (false, null);
    }

    // ���͂��ꂽ�p�x�Ƌ��������ƂɁC�������}�X�̍��W���v�Z����
    protected void CalculateXY(float a, float d, List<Vector2Int> pointPosList)
    {
        float x = Mathf.Round(posX + d * Mathf.Cos(a * Mathf.Deg2Rad)); // x���W
        float y = Mathf.Round(posY + d * Mathf.Sin(a * Mathf.Deg2Rad)); // y���W
        // �����͈͂������Ă�������ɍ��킹��
        Vector2 v = GameManager.RotateCoordinate(new Vector2(x, y), transform.rotation, transform.position);
        pointPosList.Add(new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y)));
    }

    protected void CalculateXY(float a, List<Vector2Int> pointPosList)
    {
        float x = Mathf.Round(posX + Mathf.Cos(a * Mathf.Deg2Rad));
        float y = Mathf.Round(posY + Mathf.Sin(a * Mathf.Deg2Rad));
        Vector2 v = GameManager.RotateCoordinate(new Vector2(x, y), transform.rotation, transform.position);
        pointPosList.Add(new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y)));
    }

    public void SetPieceStand()
    {
        // ���C���[�łǂ�����ps���Ăяo�������f
        thisLayer = LayerMask.LayerToName(gameObject.layer);
        if (thisLayer == firstPlayerLayer)
        {
            ps = GameManager.gameManager.firstPlayerStand.GetComponent<PieceStand>();
        }
        else if (thisLayer == secondPlayerLayer)
        {
            ps = GameManager.gameManager.secondPlayerStand.GetComponent<PieceStand>();
        }
    }

    // �����Ȃ��Ȃ�}�X�̍��W��Ԃ�
    public int GetStuckPosY()
    {
        Vector2 v = new Vector2(0, stuckPosY);
        // ��̌����ɂ���č��W�𔽓]
        Vector2 stuckPos = GameManager.RotateCoordinate(v, transform.rotation, BoardManager.centerPos);
        return Mathf.RoundToInt(stuckPos.y);
    }
}

