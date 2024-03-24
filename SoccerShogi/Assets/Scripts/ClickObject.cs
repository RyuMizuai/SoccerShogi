using UnityEngine;
using UnityEngine.EventSystems;

public class ClickObject : MonoBehaviour, IPointerClickHandler
{
    private int boardLeft;     // ���[
    private int boardRight;    // �E�[
    private int boardBottom;   // ���[
    private int boardTop;      // ��[

    private static readonly string pieceTag = "Piece"; // Piece�^�O
    private static readonly string pointTag = "Point"; // Point�^�O
    private static readonly string ballTag = "Ball"; // Ball�^�O

    [System.NonSerialized]
    public bool isSelecting = false;  // �I�𒆂��ǂ���

    public static GameObject selectingPiece = null;     // �I�����Ă����̃I�u�W�F�N�g
    public static GameObject oldSelectingPiece = null;  // �O�ɑI�����Ă�����̃I�u�W�F�N�g
    public static Vector2 selectingPiecePos;

    private GameManager gameManager;    // GameManager�̓��ꕨ

    private GameObject ballObject;      // �{�[���̃I�u�W�F�N�g

    private bool isCursor = false;      // �J�[�\���ړ������邩

    private void Awake()
    {
        
    }

    private void Start()
    {
        gameManager = GameManager.gameManager;
        ballObject = BallController.ballObject;

        // �Ղ̏�����
        boardLeft = BoardManager.boardLeft;
        boardRight = BoardManager.boardRight;
        boardBottom = BoardManager.boardBottom;
        boardTop = BoardManager.boardTop;
    }

    private void Update()
    {
        if (isCursor) { 
            if (GameManager.gameState == "Playing")
            {
                // �J�[�\���ړ�������(�����Ă�����)
                if (isSelecting)
                {
                    Vector3 mousePos = Input.mousePosition;
                    mousePos.z = 10;    // �J�[�\���̓X�N���[�����W�̂���z���W��10�ɂ���
                    Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);  // �J�[�\���ʒu�����[���h���W�ɕϊ�
                    // �J�[�\�����Տ�̏ꍇ�̂ݒǏ]������(�����Ă�����)
                    //if (pos.x > boardLeft - 0.5 && pos.x < boardRight + 0.5 && pos.y > boardBottom - 0.5 && pos.y < boardTop + 0.5)
                    //{
                        // �{�[���������Ă��Ȃ����C�h���u�����̏ꍇ
                        if (BallController.pieceHoldingBall != gameObject || GameManager.isDribbling)
                        {
                            // �I�𒆂̋���J�[�\���ړ�������
                            transform.position = pos;
                        }
                        // �p�X���̏ꍇ
                        else if (GameManager.isPassing)
                        {
                            // �{�[�����J�[�\���ړ�������
                            ballObject.transform.position = pos;
                        }
                    //}
                }
            }
        }
    }

    // �N���b�N
    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject obj = eventData.pointerPress;    // �N���b�N���ꂽ�I�u�W�F�N�g
        
        // �v���C��
        if (GameManager.gameState == "Playing")
        {
            // ��̏ꍇ
            if (obj.CompareTag(pieceTag))
            {
                ClickPiece(obj);    // �N���b�N
            }
            // �{�[���̏ꍇ
            else if (obj.CompareTag(ballTag))
            {
                // �p�X�܂��̓h���u�����̏ꍇ
                if (GameManager.isPassing || GameManager.isDribbling)
                {
                    CancelOperation(selectingPiece); // �I������
                }
                // �e������ꍇ(�܂�{�[�����ێ����N���b�N�����ꍇ)
                else if (BallController.pieceHoldingBall != null)
                {
                    ClickPiece(BallController.pieceHoldingBall);  // �e�̋���N���b�N
                }
            }
            // Point�̏ꍇ
            else if (obj.CompareTag(pointTag))
            {
                // ��C�܂��̓{�[�����ړ�������
                PieceController pc = selectingPiece.GetComponent<PieceController>();
                Piece piece = selectingPiece.GetComponent<Piece>();

                // �{�[���ێ����̏ꍇ
                if (BallController.pieceHoldingBall == selectingPiece)
                {
                    // �p�X
                    if (GameManager.isPassing)
                    {
                        selectingPiece.transform.DetachChildren();  // �q�̃{�[�����폜

                        int posX = Mathf.RoundToInt(obj.transform.position.x);
                        int posY = Mathf.RoundToInt(obj.transform.position.y);
                        Vector2Int newBallPos = new Vector2Int(posX, posY);
                        ballObject.transform.position = (Vector2)newBallPos;

                        // �S�[���`�F�b�N
                        (bool isGoal, string goalName) = GoalManager.GoalExistsAtPos(newBallPos);

                        if (isGoal)
                        {
                            SoundManager.soundManager.MakeShootSound(); // �V���[�g�̌��ʉ�
                        }
                        else
                        {
                            SoundManager.soundManager.MakePassSound();  // �p�X�̌��ʉ�
                        }

                        BallController.pieceHoldingBall = null; // �{�[���ێ���null
                        GameManager.isPassing = false;          // �p�X���I�t

                        // �ړ���ɋ����ꍇ�{�[����ێ�������
                        (bool exists, GameObject pieceObj) = piece.PieceExistsAtPos(newBallPos);

                        if (exists)
                        {
                            ballObject.transform.SetParent(pieceObj.transform);                 // ��̎q�ɂ���
                            ballObject.transform.localPosition = BallController.ballLocalPos;   // �{�[���̈ʒu����ɑ΂��ď������炷
                            BallController.pieceHoldingBall = pieceObj;                         // �{�[���ێ�

                        }
                        // �^�[���I��
                        gameManager.TurnEnd();

                    }
                    // �h���u��
                    else if (GameManager.isDribbling)
                    {
                        GameManager.isButtonClicked = true;
                        pc.MovePiece(obj.transform.position);   // ���point�܂œ�����
                        GameManager.isDribbling = false;        // �h���u�����I�t
                    }
                    gameManager.InactiveActionButton(); // �{�^�����\��

                }
                // ���ʂɓ������ꍇ
                else
                {
                    if (pc != null)
                    {
                        GameManager.isButtonClicked = true;
                        pc.MovePiece(obj.transform.position); // ���point�܂œ�����
                    }
                }

                DeselectPiece(selectingPiece);                          // �I������
                DeleteAllPoints();                                      // point�폜
                selectingPiecePos = selectingPiece.transform.position;  // ��̈ʒu��ێ�
                BallController.ballWorldPos = ballObject.transform.position;  // �{�[���̈ʒu��ێ�

            }
        }
    }

    // ���I������
    private void SelectPiece(GameObject pieceObj)
    {
        if (pieceObj != null)
        {
            pieceObj.GetComponent<ClickObject>().isSelecting = true;   // �I�𒆂ɂ���
            pieceObj.GetComponent<SpriteRenderer>().sortingOrder = 3;   // order��ύX
        }
    }

    // ��̑I������
    private void DeselectPiece(GameObject pieceObj)
    {
        if (pieceObj != null)
        {
            pieceObj.GetComponent<ClickObject>().isSelecting = false;   // �t���O�����낷
            pieceObj.GetComponent<SpriteRenderer>().sortingOrder = 2;   // order��߂�
        }
    }

    // Point�폜
    public static void DeleteAllPoints()
    {
        // Point������Ă��Ă��ׂč폜
        GameObject[] points = GameObject.FindGameObjectsWithTag(pointTag);
        foreach (GameObject point in points)
        {
            Destroy(point);
        }
    }

    // ������L�����Z������
    private void CancelOperation(GameObject pieceObj)
    {
        DeselectPiece(pieceObj);            // �I������
        gameManager.CancelAction(pieceObj); // �p�X�C�h���u���t���O�Ƌ�ƃ{�[���̍��W��߂� point���폜
        gameManager.InactiveActionButton(); // �{�^����\��
    }

    // ���͂��ꂽ����N���b�N
    private void ClickPiece(GameObject obj)
    {
        if (LayerMask.LayerToName(obj.layer) == GameManager.nowPlayer)
        {
            // �v���C���[�̋�̏ꍇ
            // ���I���C�܂��͑I����������

            oldSelectingPiece = selectingPiece; // ��̂��߂ɕێ�
            selectingPiece = obj;               // �N���b�N���ꂽ��̃I�u�W�F�N�g
            PieceController pc = selectingPiece.GetComponent<PieceController>();

            if (pc == null)
            {
                Debug.Log("Error");
                return;
            }

            // �I�𒆂̋��������x�N���b�N�����ꍇ
            if (isSelecting)
            {
                // �������I�����Ă����ꍇ�C���J�[�\���I���̎�
                if (pc.IsInHand() && isCursor)
                {
                    // �ꎞ�I�Ɍ��炵��������̕\����߂�
                    selectingPiece.GetComponent<Piece>().CountUp();
                    GameManager.DisplayPieceCount(selectingPiece);   // ��̃J�E���g��\��
                }

                CancelOperation(selectingPiece); // �������
                // �I�𒆂̋�֘A��null�ɂ���
                selectingPiece = null;
                oldSelectingPiece = null;
            }
            // �I�𒆂̋�łȂ�����N���b�N�����ꍇ
            else
            {
                // ���̋��I�����Ă����ꍇ(�J�[�\���I���Ȃ�{�[���ێ����̋��I�����Ă����Ƃ��̂�)
                if (oldSelectingPiece != null)
                {
                    CancelOperation(oldSelectingPiece);  // �O�̋�̑I������
                }

                SelectPiece(selectingPiece);                            // �I��
                selectingPiecePos = selectingPiece.transform.position;  // �ʒu��ێ�

                // ������łȂ����J�[�\���I�t�Ȃ�C�킩��₷���悤�ɋ���������炷
                if (isCursor == false)
                {
                    Vector3 pos = new Vector2(0.0f, 0.1f) * transform.up.y; // ��̂���
                    transform.position += pos;                              // �������炷
                }

                // �{�[���������Ă���ꍇ
                if (BallController.pieceHoldingBall == selectingPiece)
                {
                    // �{�^����\��
                    gameManager.ActiveActionButton();
                }
                // �{�[���������Ă��Ȃ��ꍇ�C�ʏ�̈ړ��͈͂�\��
                else
                {
                    foreach (Vector2Int pointPos in pc.pointPosList)
                    {
                        Instantiate(gameManager.pointPrefab, (Vector2)pointPos, Quaternion.identity); // Point��Prefab���쐬
                    }
                }

                // �������I�������ꍇ�C���J�[�\���I���̂Ƃ�
                if (pc.IsInHand() && isCursor)
                {
                    // �ꎞ�I�Ɏ�����̕\�������炷
                    selectingPiece.GetComponent<Piece>().CountDown();
                    GameManager.DisplayPieceCount(selectingPiece);   // ��̃J�E���g��\��
                }
            }
        }
    }
}
