using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class GameManager : MonoBehaviour
{
    private static readonly float epsilon = 0.0001f; // ��������

    public static string gameState = "";    // �Q�[���̏��
    public static string nowPlayer = "";    // ���̃v���C���[

    public static bool isButtonClicked = false;         // Button����������
    public static bool isInCheck = false;               // ���肩
    public static bool isSelectingPromotion = false;    // ���邩�I�𒆃t���O
    public static bool isPassing = false; // �p�X����
    public static bool isDribbling = false; // �h���u������

    public static List<GameObject> pieceInCheckList = new List<GameObject>();   // ��������Ă����
    public static List<Vector2Int> intersectingPosList = new List<Vector2Int>();// ����̍��W

    public static string inCheckPlayer = "";    // ��������Ă���v���[���[
    public static string nowSceneName = "";     // ���݂̃V�[����

    [SerializeField]
    private GameObject promoteButtonPanel;  // ����{�^���p�l��

    [SerializeField]
    private Image promotedPieceImage;       // ����{�^���ɕ\������摜

    [SerializeField]
    private Image notPromotedPieceImage;    // ����Ȃ��{�^���ɕ\������摜

    [SerializeField]
    private GameObject actionButtonPanel;   // �A�N�V�����{�^���p�l��

    [SerializeField]
    private GameObject optionPanel;         // �ݒ��ʂ̃p�l��

    [SerializeField]
    private TMP_Text messageText;           // �Q�[���I����̃��b�Z�[�W��Text

    [SerializeField]
    private TMP_Text scoreText;             // �X�R�A��Text�I�u�W�F�N�g
    private static string scoreString = "0 - 0";      // �X�R�A��Text

    [SerializeField]
    private TMP_Text firstPlayerNameText;   // ���̖��O

    [SerializeField]
    private TMP_Text secoondPlayerNameText; // ���̖��O

    private GameObject ballObject;          // �T�b�J�[�{�[���̃I�u�W�F�N�g
    public GameObject pointPrefab;          // Point��Prefab
    public GameObject firstPlayerStand;
    public GameObject secondPlayerStand;

    public static readonly string firstPlayerLayer = "FirstPlayer";     // FirstPlayer���C���[
    public static readonly string secondPlayerLayer = "SecondPlayer";   // SecondPlayer���C���[
    public static readonly string pieceTag = "Piece";                   // Piece�^�O

    [System.NonSerialized]
    public GameObject[] pieces;            // ���ׂĂ̋�̃Q�[���I�u�W�F�N�g

    public static GameManager gameManager;  // GameManager�̓��ꕨ

    [System.NonSerialized]
    public GameObject firstPlayerKingObj;    // ���ʂ̃I�u�W�F�N�g

    [System.NonSerialized]
    public GameObject secondPlayerKingObj;   // ���ʂ̃I�u�W�F�N�g

    private BoardManager boardManager;      // BoardManager

    public static bool isFinishInitialize = false;

    // �v���C���[�̓��_
    private static int firstPlayerScore = 0;
    private static int secondPlayerScore = 0;

    private bool isCPU = true;  // ���肪�R���s���[�^�[���ǂ���


    // ��̖��O�̑Ή��\
    private readonly Dictionary<string, string> pieceNameDictionary = new Dictionary<string, string>()
    {
        { "pawn", "����" },
        { "lance", "����" },
        { "knight", "�j�n" },
        { "silver", "�⏫" },
        { "gold", "����" },
        { "bishop", "�p�s" },
        { "rook", "���" },
        { "king", "����" },
        { "gyoku", "�ʏ�" },
    };



    private void Awake()
    {
        gameManager = this;    // static�ϐ��Ɏ�����ۑ�����        
    }

    private void Start()
    {
        // UI���\��
        promoteButtonPanel.SetActive(false);
        actionButtonPanel.SetActive(false);
        messageText.gameObject.SetActive(false);
        optionPanel.SetActive(false);

        scoreText.text = scoreString;                       // �X�R�A��\��
        boardManager = GetComponent<BoardManager>();        // BoardManager
        nowSceneName = SceneManager.GetActiveScene().name;  // ���݂̃V�[����

        StartCoroutine(InitCoroutine());                    // �Q�[���̏����ݒ�

        firstPlayerNameText.text = SetText.firstPlayerName;
        secoondPlayerNameText.text = SetText.secondPlayerName;
    }

    private IEnumerator InitCoroutine()
    {
        yield return StartCoroutine(boardManager.SetBoard());   // �ՂƋ���쐬
        Init();                                                 // ���̑�������
    }


    // ������
    private void Init()
    {
        pieces = GameObject.FindGameObjectsWithTag(pieceTag);   // �S�Ă̋���擾
        ballObject = BallController.ballObject;                 // �{�[�����擾

        nowPlayer = firstPlayerLayer;                           // ���݂̃v���C���[������ɂ���

        GoalManager.SetGoalPos();                               // �S�[���̍��W���Z�b�g

        isFinishInitialize = true;                              // ����������

        gameState = "Playing";                                  // �Q�[�����
    }
    
    // ���݂̍��W�Ɖ�]����������C��]���̍��W���󂯎���āC��]��̍��W��Ԃ�
    public static Vector2 RotateCoordinate(Vector2 objPosition, Quaternion objRotation, Vector2 axisPos)
    {
        Vector3 newPos = objPosition - axisPos;         // ���s�ړ���̍��W
        Vector3 v = objRotation * newPos;               // ��]��̍��W
        return new Vector2(v.x, v.y) + axisPos;         // ������x���s�ړ�
    }

    // ������̐�����ʂɕ\������
    public static void DisplayPieceCount(GameObject pieceObj)
    {
        Piece piece = pieceObj.GetComponent<Piece>();
        int count = piece.GetCount();                           // ������̐�
        GameObject textObj = piece.GetCountText();              // ������̐���Text
        textObj.transform.rotation = piece.transform.rotation;  // Text����Ɠ��������ɂ���

        // �����2�ȏ゠��Ƃ��\��
        if (count > 1)
        {
            Quaternion pieceRotation = piece.transform.rotation;
            Vector2 piecePos = RotateCoordinate(piece.GetPieceStandPos(), pieceRotation, BoardManager.centerPos);   // ������̈ʒu�̍��W
            Vector2 pos = new Vector2(0.35f, 0.3f) * piece.transform.up.y;  // ���̂���
            Vector2 textPos = piecePos + pos;   // Text�̍��W
            textObj.transform.position = textPos;
            textObj.GetComponent<TMP_Text>().text = count.ToString();
            textObj.SetActive(true);    // �\��
        }
        // �����0��1�̂Ƃ���\��
        else
        {
            textObj.SetActive(false);   // ��\��
        }
    }

    // ����{�^��
    public void PromoteButton()
    {
        Piece piece = ClickObject.selectingPiece.GetComponent<Piece>(); // ���쒆�̋�
        piece.isPromoted = true;
        piece.GetComponent<SpriteRenderer>().sprite = piece.promotedPieceSprite; // �摜�������ւ���
        promoteButtonPanel.SetActive(false);    // �{�^����\��
        isSelectingPromotion = false;           // �t���O�����낷
        isButtonClicked = true;                 // �{�^���N���b�N�t���O�I��
    }
    
    // ���炸�{�^��
    public void NotPromoteButton()
    {
        // �{�^������������
        promoteButtonPanel.SetActive(false);    // �{�^����\��
        isSelectingPromotion = false;           // �t���O�����낷
        isButtonClicked = true;                 // �{�^���N���b�N�t���O�I��
    }

    // ����{�^����\������
    public void ActivePromoteButton()
    {
        Piece piece = ClickObject.selectingPiece.GetComponent<Piece>(); // �I�𒆂̋��Piece���擾
        // �{�^���ɕ\������摜��I�𒆂̋�̂��̂ɍ����ւ���
        promotedPieceImage.sprite = piece.promotedPieceSprite;
        notPromotedPieceImage.sprite = piece.pieceSprite;
        promoteButtonPanel.SetActive(true);
    }

    // �p�X�{�^��
    public void PassButton()
    {
        CancelAction(ClickObject.selectingPiece); // �N���b�N���̋�̃A�N�V������������
        isPassing = true;   // �p�X��
        BallController.CalculatePassPos();      // �p�X�͈̔͂��X�V

        // �p�X�͈̔͂�\��
        foreach (Vector2Int passPos in BallController.passPosList)
        {
            Instantiate(pointPrefab, (Vector2)passPos, Quaternion.identity); // Point��Prefab���쐬
        }
        InactiveActionButton(); // �{�^����\��
    }

    // �h���u���{�^��
    public void DribbleButton()
    {
        CancelAction(ClickObject.selectingPiece); // �N���b�N���̋�̃A�N�V������������
        isDribbling = true; // �h���u����
        PieceController pc = BallController.pieceHoldingBall.GetComponent<PieceController>();

        // �h���u���͈̔͂�\��
        foreach (Vector2Int pointPos in pc.pointPosList)
        {
            Instantiate(pointPrefab, (Vector2)pointPos, Quaternion.identity); // Point��Prefab���쐬
        }
        InactiveActionButton(); // �{�^����\��
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
        if (BallController.pieceHoldingBall != null)
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
        // �I�𒆂̋�
        GameObject piece = ClickObject.selectingPiece;
        Vector2 piecePos = piece.transform.position;
        Quaternion pieceRotation = piece.transform.rotation;

        Vector2 pos = new Vector2(0.75f, 0.3f) * piece.transform.up.y;  // ���̂���
        actionButtonPanel.transform.position = piecePos + pos; ;        // �{�^���̍��W


        actionButtonPanel.SetActive(true);  // �{�^���\��
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

    // �ݒ�{�^��
    public void OptionButton()
    {
        if (nowPlayer == firstPlayerLayer)
        {
            optionPanel.SetActive(true);
        }
    }

    // �ݒ��ʂ���߂�{�^��
    public void ReturnFromOptionButton()
    {
        optionPanel.SetActive(false);
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

            // ������܂��͕ʃv���C���[�̋�̂Ƃ��X�L�b�v
            if (pc.IsInHand() || pieceLayer != playerName) continue;

            foreach (Vector2Int pointPos in pc.pointPosList)
            {
                // Point�Ƒ���̉��̍��W����v�����Ƃ�true��Ԃ�
                if (TwoPositionsEquals(kingObj.transform.position, pointPos))
                {
                    inCheckPlayer = playerName; // ���蒆�̃v���C���[��ێ�
                    return true;
                }
            }
        }
        return false;
    }

    // ��������Ă�����Ԃ�
    private List<GameObject> GetPiecesInCheck()
    {
        List<GameObject> objList = new List<GameObject>();

        // ���肳��Ă���v���C���[�̋�
        GameObject kingObj = GetOpponentKingObject(inCheckPlayer);

        foreach (GameObject piece in pieces)
        {
            string pieceLayer = LayerMask.LayerToName(piece.layer);
            PieceController pc = piece.GetComponent<PieceController>();

            // ������ł��邩�C���蒆�̃v���C���[�̋�łȂ��Ƃ��X�L�b�v
            if (pc.IsInHand() || pieceLayer != inCheckPlayer) continue;
            
            foreach (Vector2 pointPos in pc.pointPosList)
            {
                // Point�Ƒ���̉��̍��W����v�����Ƃ�
                if (TwoPositionsEquals(kingObj.transform.position, pointPos))
                {
                    // List�ǉ�
                    objList.Add(pc.gameObject);
                }
            }
        }
        return objList;
    }

    // ����̍��W���v�Z����
    private List<Vector2Int> GetIntersectingPos()
    {
        List<Vector2Int> vec = new List<Vector2Int>();

        // ���肳��Ă���v���C���[�̋�
        GameObject kingObj = GetOpponentKingObject(inCheckPlayer);

        // ������̏ꍇ�͍�������Ȃ�����
        // ��������Ă���1�̏ꍇ�̂ݍ���̍��W���v�Z
        if (pieceInCheckList.Count == 1)
        {
            GameObject pieceInCheck = pieceInCheckList[0];  // ��������Ă����

            // ���蒆�̋�Ɖ��̊Ԃɉ��蒆�̋��Point������΍���List�ǉ�
            PieceController pc = pieceInCheck.GetComponent<PieceController>();

            foreach (Vector2Int pointPos in pc.pointPosList)
            {
                // ���蒆�̋��Point�����蒆�̋�Ɖ��̓����_���ǂ���
                if (IsInternalDivision(pieceInCheck.transform.position, kingObj.transform.position,pointPos))
                {
                    vec.Add(pointPos); // ����List�ǉ�
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

        // ��������Ă����Ɖ��̈ʒu�֌W�ɂ���ď�����ς���

        // �c�����̂Ƃ� 3�_x���W������
        if (Mathf.Approximately(v.x, v2.x) && Mathf.Approximately(v.x, v3.x))
        {
            return 0 < ty && ty < 1;    // �������Ă��邩
        }

        // �������̂Ƃ� 3�_y���W������
        else if (Mathf.Approximately(v.y, v2.y) && Mathf.Approximately(v.y, v3.y))
        {
            return 0 < tx && tx < 1;    // �������Ă��邩
        }
        // �΂߂̂Ƃ�
        else
        {
            bool equal = Mathf.Approximately(tx, ty);   // v3��v��v2�����񂾒�����ɂ��邩��tx=ty���ǂ���
            return equal && 0 < tx && tx < 1;           // �������Ă��邩(ty�ł�����)
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
                if (pc.pointPosList.Count != 0)
                {
                    return false;   // Point���P�ł������false
                }
            }
        }
        return true;    // 1���Ȃ����true
    }

    // �T�b�J�[�{�[���̃I�u�W�F�N�g�ƍ��W����v������true��Ԃ�
    public bool BallExistsAtPos(Vector2 v)
    {
        return TwoPositionsEquals(ballObject.transform.position, v);
    }

    // 2�̍��W�������������肷��
    public static bool TwoPositionsEquals(Vector2 v1, Vector2 v2)
    {
        return Vector2.SqrMagnitude(v1 - v2) < epsilon;
    }


    private IEnumerator TurnEndCoroutine()
    {
        // ����{�^�����������̂�҂�
        yield return new WaitUntil(() => isButtonClicked);

        // �S�[���`�F�b�N
        (bool isGoal, string goalName) = GoalManager.GoalExistsAtPos(BallController.ballWorldPos);

        if (isGoal)
        {
            // �{�[�����S�[���ɓ����Ă���S�[��
            yield return StartCoroutine(Goal(goalName));
        }

        // �������
        ClickObject.selectingPiece = null;
        ClickObject.oldSelectingPiece = null;

        // ����֌W��List��������
        pieceInCheckList.Clear();
        intersectingPosList.Clear();

        BallController.CalculateDribblePos();   // �h���u���͈̔͂��X�V

        // �����̋��Point�X�V
        foreach (GameObject piece in pieces)
        {
            if (LayerMask.LayerToName(piece.layer) == nowPlayer)
            {
                PieceController pc = piece.GetComponent<PieceController>();
                // ������ʒu���X�V����
                pc.CalculatePointPos();
            }
        }

        isInCheck = IsCheck(nowPlayer); // ������X�V

        // ����`�F�b�N
        if (isInCheck)
        {
            pieceInCheckList = GetPiecesInCheck();      // ���蒆�̋�
            intersectingPosList = GetIntersectingPos(); // ����̍��W
        }

        // ����̋��Point�X�V
        foreach (GameObject piece in pieces)
        {
            if (LayerMask.LayerToName(piece.layer) != nowPlayer)
            {
                PieceController pc = piece.GetComponent<PieceController>();
                // ������ʒu���X�V����
                pc.CalculatePointPos();

                // CPU���[�h�̏ꍇ�C��肪�łĂ�}�X�����ׂĊi�[
                if (isCPU && nowPlayer == firstPlayerLayer)
                {
                    foreach (Vector2Int pointPos in pc.pointPosList)
                    {
                        CPUManager.CPUPos.Add(new Member(pc.gameObject, pointPos));
                    }
                }
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

        // CPU���[�h�Ȃ玩���őł�����
        if (isCPU && nowPlayer == secondPlayerLayer)
        {
            yield return new WaitForSeconds(1.0f);
            CPUManager.MovingCPU();
        }

    }

    // �^�[���I��
    public void TurnEnd()
    {
        StartCoroutine(TurnEndCoroutine());   // �{�^���R���[�`�����Ă�
    }

    // �S�[��
    private IEnumerator Goal(string goalName)
    {
        gameState = "Goal";

        SoundManager.soundManager.MakeGoalSound();  // �S�[���̓J��炷

        bool isOwnGoal = (goalName == nowPlayer);   // �I�E���S�[�����ǂ���

        // �������S�[���ɂ���ē��_�����Z
        if (goalName == firstPlayerLayer)
        {
            secondPlayerScore++;
        }
        else if (goalName == secondPlayerLayer)
        {
            firstPlayerScore++;
        }

        PieceController pc = ClickObject.selectingPiece.GetComponent<PieceController>();
        string type = pc.pieceType.ToString();          // �Ō�Ƀ{�[���������Ă�����
        string typeKanji = pieceNameDictionary[type];   // �����ɕϊ�

        string goalMessage;   // �S�[�������߂����̃��b�Z�[�W
        // �I�E���S�[�����ǂ�����Text��ς���
        if (isOwnGoal)
        {
            goalMessage = "�I�E���S�[���I";
        }
        else
        {
            goalMessage = typeKanji + "�̃S�[���I";
        }

        // �X�R�AText���X�V
        string updateScoreText = firstPlayerScore.ToString() + " - " + secondPlayerScore.ToString();
        scoreString = updateScoreText;

        // �S�[�����b�Z�[�W��\��
        messageText.text = goalMessage;
        messageText.gameObject.SetActive(true);


        yield return new WaitForSeconds(2.0f);      // 2�b�҂�
        SceneManager.LoadScene(nowSceneName);       // �Ֆʂ�����������
       
        
        // �{�[����K�؂Ȉʒu�ɏ�����

    }

    // �Q�[���I��
    public IEnumerator GameOver(string winningPlayer)
    {
        gameState = "GameEnd";  // �Q�[���I��

        StartCoroutine(SoundManager.soundManager.MakeGameEndSound());

        // �������v���C���[�ɉ����ă��b�Z�[�W��Text�ɏo��
        string message = (winningPlayer == firstPlayerLayer) ? "���Ȃ��̏����I" : "���Ȃ��̕���!";
        
        messageText.text = message;
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(3.0f);  // 3�b�҂�
        TitleManager.LoadTitleScene();          // �X�^�[�g�V�[����ǂݍ���
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

}
