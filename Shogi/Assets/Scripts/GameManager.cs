using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static readonly Vector2Int centerPos = new Vector2Int(5, 5);  // �Ղ̒��S�̍��W
    public static readonly int boardLeft = 1;     // ���[
    public static readonly int boardRight = 9;    // �E�[
    public static readonly int boardBottom = 1;   // ���[
    public static readonly int boardTop = 9;      // ��[

    private readonly float epsilon = 0.0001f; // ��������

    public static string gameState = "";    // �Q�[���̏��
    public static string nowPlayer = "";    // ���̃v���C���[

    public static bool isButtonClicked = false;         // Button����������
    public static bool isInCheck = false;               // ���肩
    public static bool isSelectingPromotion = false;    // ���邩�I�𒆃t���O
    public static bool isPassing = false; // �p�X����
    public static bool isDribbling = false; // �h���u������

    public static List<GameObject> piecesInCheck = new List<GameObject>();      // ��������Ă����
    public static List<Vector2Int> intersectingPos = new List<Vector2Int>();    // ����̍��W

    public static string inCheckPlayer = "";    // ��������Ă���v���[���[
    public static string nowSceneName = "";     // ���݂̃V�[����

    [SerializeField]
    private GameObject promoteButtonPanel;  // ����{�^���p�l��

    [SerializeField]
    private GameObject actionButtonPanel;   // �A�N�V�����{�^���p�l��

    [SerializeField]
    private GameObject firstPlayerKingObj;       // ���ʂ̃I�u�W�F�N�g

    [SerializeField]
    private GameObject secondPlayerKingObj;        // ���ʂ̃I�u�W�F�N�g

    public GameObject ballObject;           // �T�b�J�[�{�[���̃I�u�W�F�N�g
    public GameObject pointPrefab;          // Point��Prefab

    private readonly string firstPlayerLayer = "FirstPlayer";  // FirstPlayer���C���[
    private readonly string secondPlayerLayer = "SecondPlayer";    // SecondPlayer���C���[
    private string pieceTag = "Piece";      // Piece�^�O

    private GameObject[] pieces;            // ���ׂĂ̋�̃Q�[���I�u�W�F�N�g

    [System.NonSerialized]
    public bool isCalledFromInCheck = false;    // IsCheck����Ă΂ꂽ��(�v���P)

    public static GameManager gameManager;  // GameManager�̓��ꕨ

    public Text messageText;                // �Q�[���I����̃��b�Z�[�W��Text

    public static List<Vector2Int> goalPosList = new List<Vector2Int>(); // �S�[���̈ʒu

    private void Awake()
    {
        gameManager = this;
    }

    private void Start()
    {
        pieces = GameObject.FindGameObjectsWithTag(pieceTag);
        promoteButtonPanel.SetActive(false);                // �{�^����\��
        actionButtonPanel.SetActive(false);                 // �{�^����\��
        messageText.gameObject.SetActive(false);            // Text��\��
        gameState = "Playing";                              // �Q�[�����
        nowPlayer = firstPlayerLayer;                       // ���݂̃v���C���[������ɂ���
        nowSceneName = SceneManager.GetActiveScene().name;  // ���݂̃V�[����
        goalPosList.Add(new Vector2Int(5, 10));
    }
    
    // ���݂̍��W�Ɖ�]����������C��]���̍��W���󂯎���āC��]��̍��W��Ԃ�
    public static Vector2 RotateCoordinate(Vector2 objPosition, Quaternion objRotation, Vector2 axisPos)
    {
        Vector3 newPos = objPosition - axisPos;         // ���s�ړ���̍��W
        Vector3 v = objRotation * newPos;               // ��]��̍��W
        return new Vector2(v.x, v.y) + axisPos;         // ������x���s�ړ�

    }

    // ������̐�����ʂɕ\������
    public static void DisplayPieceCount(Piece piece)
    {
        int count = piece.GetCount();                           // ������̐�
        GameObject textObj = piece.GetCountText();              // ������̐���Text
        textObj.transform.rotation = piece.transform.rotation;  // Text����Ɠ��������ɂ���

        // �����2�ȏ゠��Ƃ��\��
        if (count > 1)
        {
            Quaternion pieceRotation = piece.transform.rotation;
            Vector2 piecePos = RotateCoordinate(piece.GetPieceStandPos(), pieceRotation, centerPos);    // ��̍��W
            Vector2 pos = RotateCoordinate(new Vector2(0.25f, 0.2f), pieceRotation, new Vector2(0.0f, 0.0f)); // ���̂���
            Vector2 textPos = piecePos + pos;   // Text�̍��W
            textObj.transform.position = textPos;
            textObj.GetComponent<Text>().text = count.ToString();
            textObj.SetActive(true);    // �\��
        }
        // �����0��1�̂Ƃ���\��
        else
        {
            textObj.SetActive(false);   // ��\��
        }
    }

    // ����{�^��
    public void PromotePieceButton()
    {
        Piece piece = ClickObject.selectingPiece.GetComponent<Piece>(); // ���쒆�̋�
        piece.isPromoted = true;
        piece.GetComponent<SpriteRenderer>().sprite = piece.promotedPieceSprite; // �摜�������ւ���
        promoteButtonPanel.SetActive(false);    // �{�^����\��
        isSelectingPromotion = false;           // �t���O�����낷
        ClickObject.selectingPiece = null;      // ����I��
        isButtonClicked = true;                 // �{�^���N���b�N�t���O�I��
    }
    
    // ���炸�{�^��
    public void NotPromotePieceButton()
    {
       // �{�^������������
        promoteButtonPanel.SetActive(false);    // �{�^����\��
        isSelectingPromotion = false;           // �t���O�����낷
        ClickObject.selectingPiece = null;      // ����I��
        isButtonClicked = true;                 // �{�^���N���b�N�t���O�I��
    }

    // ����{�^����\������
    public void ActivePromoteButton()
    {
        promoteButtonPanel.SetActive(true);
    }

    // �p�X�{�^��
    public void PassButton()
    {
        CancelAction(ClickObject.selectingPiece); // �N���b�N���̋�̃A�N�V������������
        isPassing = true;   // �p�X��

        foreach (Vector2Int passPos in BallController.passPosList)
        {
            Instantiate(pointPrefab, new Vector3(passPos.x, passPos.y), Quaternion.identity); // Point��Prefab���쐬
        }
    }

    // �h���u���{�^��
    public void DribbleButton()
    {
        CancelAction(ClickObject.selectingPiece); // �N���b�N���̋�̃A�N�V������������
        isDribbling = true; // �h���u����        

        foreach (Vector2Int dribblePos in BallController.dribblePosList)
        {
            Instantiate(pointPrefab, new Vector3(dribblePos.x, dribblePos.y), Quaternion.identity); // Point��Prefab���쐬
        }
    }

    // �A�N�V������߂�
    public void CancelAction(GameObject obj)
    {
        // �t���O�����낷
        isPassing = false;
        isDribbling = false;
        // ��ƃ{�[���̍��W�����̈ʒu�ɖ߂�
        obj.transform.position = ClickObject.selectingPiecePos;
        // �ێ�����Ă���ꍇ
        if (ballObject.transform.root.gameObject != ballObject)
        {
            // �{�[������̏�����ɔz�u
            ballObject.transform.localPosition = BallController.ballLocalPos;
        }
        // �e�����Ȃ��ꍇ
        else
        {
            // ���̈ʒu�ɔz�u
            ballObject.transform.position = BallController.ballWorldPos;
        }
        
        // Point��S�폜
        ClickObject.DeleteAllPoints();
    }

    // �p�X�{�^���ƃh���u���{�^����\������
    public void ActiveActionButton()
    {
        actionButtonPanel.SetActive(true);
    }

    // �p�X�{�^���ƃh���u���{�^�����\���ɂ���
    public void InactiveActionButton()
    {
        actionButtonPanel.SetActive(false);
    }

    // ���g���C�{�^��
    public void RetryButton()
    {
        SceneManager.LoadScene(nowSceneName);   // ���݂̃V�[����ǂݍ���
    }


    // ���͂��ꂽ�v���[���[����������Ă��邩�ǂ������肷��
    public bool IsCheck(string playerName)
    {
        inCheckPlayer = "Default"; // ���蒆�̃v���C���[��������
        // ����ʂ̃I�u�W�F�N�g
        GameObject kingObj = GetOpponentKingObject(playerName);

        foreach (GameObject piece in pieces)
        {
            string pieceLayer = LayerMask.LayerToName(piece.layer);
            PieceController pc = piece.GetComponent<PieceController>();
            // ���͂��ꂽ�v���C���[�̋�ŔՏ�̂��̂��擾
            if (!pc.IsInHand() && pieceLayer == playerName)
            {
                for (int i = 0; i < pc.pointList.Count; i++)
                {
                    Vector3 v = new Vector3(pc.pointList[i].x, pc.pointList[i].y);
                    // Point�̍��W�̂����ꂩ������̉��ƈ�v�����Ƃ�true��Ԃ�
                    if (Vector3.SqrMagnitude(kingObj.transform.position - v) < epsilon)
                    {
                        inCheckPlayer = playerName; // ���蒆�̃v���C���[
                        return true;
                    }
                }
            }
        }
        return false;
    }

    // ��������Ă�����Ԃ�
    private List<GameObject> GetPiecesInCheck()
    {
        List<GameObject> obj = new List<GameObject>();
        // ���肳��Ă���v���C���[�̋�
        GameObject kingObj = GetOpponentKingObject(inCheckPlayer);

        foreach (GameObject piece in pieces)
        {
            string pieceLayer = LayerMask.LayerToName(piece.layer);
            PieceController pc = piece.GetComponent<PieceController>();
            // ���蒆�̃v���C���[�̋�ŔՏ�̂��̂��擾
            if (!pc.IsInHand() && pieceLayer == inCheckPlayer)
            {
                for (int i = 0; i < pc.pointList.Count; i++)
                {
                    Vector3 v = new Vector3(pc.pointList[i].x, pc.pointList[i].y);
                    // Point�̍��W�̂����ꂩ������̉��ƈ�v�����Ƃ�
                    if (Vector3.SqrMagnitude(kingObj.transform.position - v) < epsilon)
                    {
                        // List�ǉ�
                        obj.Add(pc.gameObject);
                    }
                }
            }
        }
        return obj;
    }

    // ����̍��W���v�Z����
    private List<Vector2Int> GetIntersectingPos()
    {
        List<Vector2Int> vec = new List<Vector2Int>();
        // ���肳��Ă���v���C���[�̋�
        GameObject kingObj = GetOpponentKingObject(inCheckPlayer);
        
        foreach (GameObject pieceInCheck in piecesInCheck)  //���蒆�̋�
        {
            // ���蒆�̋�Ɖ��̊Ԃɉ��蒆�̋��Point������΍���List�ǉ�
            PieceController pc = pieceInCheck.GetComponent<PieceController>();
            for (int i = 0; i < pc.pointList.Count; i++)
            {
                Vector2Int v = pc.pointList[i];

                // ���蒆�̋��Point�����蒆�̋�Ɖ��̓����_���ǂ���
                if (IsInternalDivision(pieceInCheck.transform.position, kingObj.transform.position, v))
                {
                    vec.Add(v); // ����List�ǉ�
                }
            }
        }
        return vec;
    }

    // v3��v��v2�̓����_���ǂ���
    private bool IsInternalDivision(Vector2 v, Vector2 v2, Vector2 v3)
    {
        // 0���Z���
        if (v == v2) return false;

        float tx = Mathf.InverseLerp(v.x, v2.x, v3.x);  // x�����̓����_
        float ty = Mathf.InverseLerp(v.y, v2.y, v3.y);  // y�����̓����_

        // �c�����̂Ƃ�
        if (Mathf.Approximately(v.x, v2.x))
        {
            bool equal = Mathf.Approximately(v.x, v3.x);    // 3�_x���W��������
            return equal && 0 < ty && ty < 1;                  // �������Ă��邩
        }
        // �������̂Ƃ�
        else if (Mathf.Approximately(v.y, v2.y))
        {
            bool equal = Mathf.Approximately(v.y, v3.y);    // 3�_y���W��������
            return equal && 0 < tx && tx < 1;                  // �������Ă��邩
        }
        // �΂߂̂Ƃ�
        else
        {
            bool equal = Mathf.Approximately(tx, ty);   // v3��v��v2�����񂾒�����ɂ��邩��tx=ty���ǂ���
            return equal && 0 < tx && tx < 1;              // �������Ă��邩(ty�ł�����)
        }
    }

    // ����ʂ��擾
    public GameObject GetOpponentKingObject(string playerName)
    {
        if (playerName == firstPlayerLayer)
        {
            return secondPlayerKingObj;
        }
        else if (playerName == secondPlayerLayer)
        {
            return firstPlayerKingObj;
        }
        else
        {
            Debug.Log("Player Name Error");
            return null;
        }
    }

    // �l�݂𔻒肷��
    public bool IsCheckmate()
    {
        foreach (GameObject piece in pieces)
        {
            if (LayerMask.LayerToName(piece.layer) != nowPlayer)
            {
                // ����̂��ׂĂ̋�����Ȃ������玩���̏���
                PieceController pc = piece.GetComponent<PieceController>();
                if (pc.pointList.Count != 0)
                {
                    return false;   // Point���P�ł������false
                }
            }
        }
        return true;    // 1���Ȃ����true
    }

    public IEnumerator PromoteButtonCoroutine()
    {
        // ����{�^�����������̂�҂�
        yield return new WaitUntil(() => isButtonClicked);

        // �������
        ClickObject.selectingPiece = null;
        ClickObject.oldSelectingPiece = null;

        piecesInCheck.Clear();  // ���蒆�̋��������

        // bc���擾
        BallController bc = ballObject.GetComponent<BallController>();
        bc.CalculateDribblePos();   // �h���u���͈̔͂��X�V
        bc.CalculatePassPos();      // �p�X�͈̔͂��X�V

        // ����`�F�b�N
        if (isInCheck = IsCheck(nowPlayer))
        {
            piecesInCheck = GetPiecesInCheck();     // ���蒆�̋�
            intersectingPos = GetIntersectingPos(); // ����̍��W
        }

        // ������Point�̍X�V
        foreach (GameObject piece in pieces)
        {
            if (LayerMask.LayerToName(piece.layer) != nowPlayer)
            {
                PieceController pc = piece.GetComponent<PieceController>();
                // ������ʒu���X�V����
                pc.CalculatePointPos(pc.pointList);
                
            }
        }
        

        // �l�݃`�F�b�N
        if (isInCheck)
        {
            // ���肪�ǂ����������Ȃ�������l��
            if (IsCheckmate())
            {
                StartCoroutine(GameOver(nowPlayer));    // ���݂̃v���C���[�̏���
            }
        }

        // ����I���@����̃^�[����
        if (nowPlayer == firstPlayerLayer)
        {
            nowPlayer = secondPlayerLayer;
        }
        else if (nowPlayer == secondPlayerLayer)
        {
            nowPlayer = firstPlayerLayer;
        }
       
    }

    // �^�[���I��
    public void TurnEnd()
    {
        StartCoroutine(PromoteButtonCoroutine());   // �{�^���R���[�`�����Ă�
    }


    // �Q�[���I��
    public IEnumerator GameOver(string winningPlayer)
    {
        gameState = "GameEnd";  // �Q�[���I��

        // �������v���C���[�ɉ����ă��b�Z�[�W��Text�ɏo��
        string message;
        if (winningPlayer == firstPlayerLayer)
        {
            message = "You Win!";
        }
        else
        {
            message = "You Lose!";
        }
        messageText.text = message;
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(3.0f);  // 3�b�҂�
        TitleManager.LoadStartScene();          // �X�^�[�g�V�[����ǂݍ���
    }

    // �����{�^��
    public void GiveUpButton()
    {
        if (nowPlayer == firstPlayerLayer)
        {
            StartCoroutine(GameOver(secondPlayerLayer));
        }
        else
        {
            StartCoroutine(GameOver(firstPlayerLayer));
        }
    }

    // �T�b�J�[�{�[���̃I�u�W�F�N�g�ƍ��W����v������true��Ԃ�
    public bool BallExistsAtPos(Vector2Int v)
    {
        return Vector3.SqrMagnitude(ballObject.transform.position - (Vector3Int)v) < epsilon;
    }

    // �󂯎�������W���S�[���ʒu�����肷��
    public bool GoalExistsAtPos(Vector2Int v)
    {
        foreach(Vector2Int goalPos in goalPosList)
        {
            if (Vector3.SqrMagnitude(ballObject.transform.position - (Vector3Int)v) < epsilon)
            {
                return true;
            }
        }
        return false;
    }

    
}

    
