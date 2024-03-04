using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickObject : MonoBehaviour, IPointerClickHandler
{
    private int boardLeft;     // 左端
    private int boardRight;    // 右端
    private int boardBottom;   // 下端
    private int boardTop;      // 上端

    private static readonly string pieceTag = "Piece"; // Pieceタグ
    private static readonly string pointTag = "Point"; // Pointタグ
    private static readonly string ballTag = "Ball"; // Ballタグ

    [System.NonSerialized]
    public bool isSelecting = false;  // 選択中フラグ

    public static GameObject selectingPiece = null;     // 選択している駒のオブジェクト
    public static GameObject oldSelectingPiece = null;  // 前に選択していた駒のオブジェクト
    public static Vector2 selectingPiecePos;
    private static Piece selectingPieceScript;

    private GameManager gameManager;    // GameManagerの入れ物

    private GameObject ballObject;      // ボールのオブジェクト

    private void Awake()
    {
        // 盤の初期化
        boardLeft = GameManager.boardLeft;
        boardRight = GameManager.boardRight;
        boardBottom = GameManager.boardBottom;
        boardTop = GameManager.boardTop;
    }

    private void Start()
    {
        gameManager = GameManager.gameManager;
        ballObject = gameManager.ballObject;
    }

    private void Update()
    {
        if (GameManager.gameState == "Playing")
        {
            // カーソル移動させる(無くてもいい)
            if (isSelecting)
            {
                Vector3 mousePos = Input.mousePosition;
                mousePos.z = 10;    // カーソルはスクリーン座標のためz座標を10にする
                Vector3 pos = Camera.main.ScreenToWorldPoint(mousePos);  // カーソル位置をワールド座標に変換
                // カーソルが盤上の場合のみ追従させる(無くてもいい)
                //if (pos.x > boardLeft - 0.5 && pos.x < boardRight + 0.5 && pos.y > boardBottom - 0.5 && pos.y < boardTop + 0.5)
                //{
                    // ボールを持っていないか，ドリブル中の場合
                    if (!selectingPieceScript.isHoldingBall || GameManager.isDribbling)
                    {
                        // 選択中の駒をカーソル移動させる
                        transform.position = pos;
                    }
                    // パス中の場合
                    else if (GameManager.isPassing)
                    {
                        // ボールをカーソル移動させる
                        gameManager.ballObject.transform.position = pos;
                    }
                //}
            }
        }
    }

    // クリック
    public void OnPointerClick(PointerEventData eventData)
    {
        GameObject obj = eventData.pointerPress;    // クリックされたオブジェクト
        
        // プレイ中
        if (GameManager.gameState == "Playing")
        {
            // 駒の場合
            if (obj.CompareTag(pieceTag))
            {
                ClickPiece(obj);    // クリック
            }
            // ボールの場合
            else if (obj.CompareTag(ballTag))
            {
                // パスまたはドリブル中の場合
                if (GameManager.isPassing || GameManager.isDribbling)
                {
                    CancelOperation(selectingPiece); // 選択解除
                }
                // 親がいる場合(つまりボールが保持中クリックした場合)
                else if (transform.root.gameObject != null)
                {
                    ClickPiece(transform.root.gameObject);  // 親の駒をクリック
                }
            }
            // Pointの場合
            else if (obj.CompareTag(pointTag))
            {
                // 駒，またはボールを移動させる
                PieceController pc = selectingPiece.GetComponent<PieceController>();
                Piece piece = selectingPiece.GetComponent<Piece>();

                // ボール保持中の場合
                if (piece.isHoldingBall)
                {
                    // パス
                    if (GameManager.isPassing)
                    {
                        selectingPiece.transform.DetachChildren();  // 子のボールを削除
                        int posX = Mathf.RoundToInt(obj.transform.position.x);
                        int posY = Mathf.RoundToInt(obj.transform.position.y);
                        Vector2Int newBallPos = new Vector2Int(posX, posY);
                        ballObject.transform.position = (Vector2)newBallPos;
                        piece.isHoldingBall = false;    // ボール保持オフ
                        GameManager.isPassing = false;  // パス中オフ

                        // 移動先に駒がある場合ボールを保持させる
                        if (piece.PieceExistsAtPos(newBallPos).Item1)
                        {
                            GameObject pieceObj = piece.PieceExistsAtPos(newBallPos).Item2;
                            ballObject.transform.SetParent(pieceObj.transform);                 // 駒の子にする
                            ballObject.transform.localPosition = BallController.ballLocalPos;   // ボールの位置を駒に対して少しずらす
                            pieceObj.GetComponent<Piece>().isHoldingBall = true;                // ボール保持をtrueにする

                        }
                    }
                    // ドリブル
                    else if (GameManager.isDribbling)
                    {
                        GameManager.isButtonClicked = true;
                        pc.MovePiece(obj.transform.position);   // 駒をpointまで動かす
                        GameManager.isDribbling = false;        // ドリブル中オフ
                    }
                    gameManager.InactiveActionButton(); // ボタンを非表示
                }
                // 普通に動かす場合
                else
                {
                    if (pc != null)
                    {
                        GameManager.isButtonClicked = true;
                        pc.MovePiece(obj.transform.position); // 駒をpointまで動かす
                    }
                }

                DeselectPiece(selectingPiece);                          // 選択解除
                DeleteAllPoints();                                      // point削除
                selectingPiecePos = selectingPiece.transform.position;  // 駒の位置を保持
                BallController.ballWorldPos = ballObject.transform.position;  // ボールの位置を保持
                selectingPieceScript = null;


                // ターン終了
                gameManager.TurnEnd();
            }
        }
    }

    // 駒を選択する
    private void SelectPiece(GameObject pieceObj)
    {
        if (pieceObj != null)
        {
            pieceObj.GetComponent<ClickObject>().isSelecting = true;   // 選択中にする
            pieceObj.GetComponent<SpriteRenderer>().sortingOrder = 3;   // orderを変更
        }
    }

    // 駒の選択解除
    private void DeselectPiece(GameObject pieceObj)
    {
        if (pieceObj != null)
        {
            pieceObj.GetComponent<ClickObject>().isSelecting = false;   // フラグを下ろす
            pieceObj.GetComponent<SpriteRenderer>().sortingOrder = 2;   // orderを戻す
        }
    }

    // Point削除
    public static void DeleteAllPoints()
    {
        // Pointを取ってきてすべて削除
        GameObject[] points = GameObject.FindGameObjectsWithTag(pointTag);
        foreach (GameObject point in points)
        {
            Destroy(point);
        }
    }

    // 操作をキャンセルする
    private void CancelOperation(GameObject obj)
    {
        DeselectPiece(obj);                 // 選択解除
        DeleteAllPoints();                  // Point削除
        gameManager.CancelAction(obj);      // パス，ドリブルフラグと駒とボールの座標を戻す
        gameManager.InactiveActionButton(); // ボタン非表示
    }

    // 入力された駒をクリック
    private void ClickPiece(GameObject obj)
    {
        if (LayerMask.LayerToName(obj.layer) == GameManager.nowPlayer)
        {
            // プレイヤーの駒の場合
            // 駒を選択，または選択解除する

            oldSelectingPiece = selectingPiece; // 後のために保持
            selectingPiece = obj;               // クリックされた駒のオブジェクト
            PieceController pc = selectingPiece.GetComponent<PieceController>();

            if (pc == null)
            {
                Debug.Log("Error");
                return;
            }

            // 選択中の駒をもう一度クリックした場合
            if (isSelecting)
            {
                CancelOperation(selectingPiece); // 選択解除
                // 選択中の駒関連をnullにする
                selectingPiece = null;
                oldSelectingPiece = null;
            }
            // 選択中の駒でない駒をクリックした場合
            else
            {
                // 他の駒を選択していた場合(ボール保持中の駒を選択しているとき)
                if (oldSelectingPiece != null)
                {
                    CancelOperation(oldSelectingPiece);  // 前の駒の選択解除
                }

                SelectPiece(selectingPiece);                            // 選択
                selectingPiecePos = selectingPiece.transform.position;  // 位置を保持
                Piece piece = selectingPiece.GetComponent<Piece>();
                selectingPieceScript = piece;                           // Pieceスクリプトを保持

                // ボールを持っている場合
                if (piece.isHoldingBall)
                {
                    // ボタンを表示
                    gameManager.ActiveActionButton();
                }
                // ボールを持っていない場合，通常の移動範囲を表示
                else
                {
                    foreach (Vector2Int pointPos in pc.pointPosList)
                    {
                        Instantiate(gameManager.pointPrefab, (Vector2)pointPos, Quaternion.identity); // PointのPrefabを作成
                    }
                }
            }
        }
    }
}
