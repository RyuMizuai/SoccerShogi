using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingSript : MonoBehaviour
{
    // 名前を入力するフィールド
    [SerializeField]
    private TMP_InputField firstPlayerNameInput;

    [SerializeField]
    private TMP_InputField secondPlayerNameInput;

    // プレイヤーの名前
    public static string firstPlayerName;
    public static string secondPlayerName;

    // 最大スコア
    public static int maxScore = 1;

    // スコア設定ボタン
    [SerializeField]
    private Button[] scoreButtons;

    // スコアボタンの数
    private int scoreButtonCount;

    // 色
    Color32 white = new Color32(255, 255, 255, 255);    // 白
    Color32 gray = new Color32(200, 200, 200, 255);     // グレー
    Color32 pressedColor = new Color32(255, 230, 150, 255);    // ベージュ


    // 難易度設定ボタン
    [SerializeField]
    private Button difButton;

    private void Start()
    {
        scoreButtonCount = scoreButtons.Length;

        // 端のボタンの色を変える
        scoreButtons[0].image.color = pressedColor;
        difButton.image.color = pressedColor;

        maxScore = 1;
    }

    // 設定完了
    public void FinishSetting()
    {
        // 名前を保持してシーンを変更
        /*if (firstPlayerNameInput.text != "" && secondPlayerNameInput.text != "")
        {
            firstPlayerName = firstPlayerNameInput.text;
            secondPlayerName = secondPlayerNameInput.text;
        }
        else
        {
            // エラー文を出力
            Debug.Log("名前を入力してください");
        }*/

        firstPlayerName = (firstPlayerNameInput.text != "") ? firstPlayerNameInput.text : "あなた";
        secondPlayerName = (secondPlayerNameInput.text != "") ? secondPlayerNameInput.text : "あいて";

        TitleManager.LoadPlayScene();
        SoundManager.soundManager.MakeGameStartSound();
    }

    // スコアボタン
    public void ClickScoreButon(int index)
    {
        // ボタン番号の変数を受け取る
        // 色を変更
        for (int i = 1; i <= scoreButtonCount; i++)
        {
            // 押したボタンならグレーにする
            if (i == index)
            {
                scoreButtons[i - 1].image.color = pressedColor;
            }
            // そうでなければ白にする
            else
            {
                scoreButtons[i - 1].image.color = white;
            }
        }

        maxScore = index;   // 最大スコアを設定
    }
}
