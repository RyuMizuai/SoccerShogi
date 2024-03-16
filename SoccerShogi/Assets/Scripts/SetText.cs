using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SetText : MonoBehaviour
{
    // 名前を入力するフィールド
    public TMP_InputField firstPlayerNameInput;
    public TMP_InputField secondPlayerNameInput;

    // プレイヤーの名前
    public static string firstPlayerName = "名前";
    public static string secondPlayerName;

    // 設定完了
    public void FinishSetting()
    {
        // 正しく入力できていれば名前を保持してシーンを変更
        if (firstPlayerNameInput.text != "" && secondPlayerNameInput.text != "")
        {
            firstPlayerName = firstPlayerNameInput.text;
            secondPlayerName = secondPlayerNameInput.text;
            SceneManager.LoadScene("AllCoat");
            SoundManager.soundManager.MakeGameStartSound();
        }
        else
        {
            // エラー文を出力
            Debug.Log("名前を入力してください");
        }
    }
}
