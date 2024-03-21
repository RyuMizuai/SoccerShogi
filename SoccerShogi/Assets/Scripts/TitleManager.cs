using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleManager : MonoBehaviour
{
    private static string titleScene = "Start";
    private static string settingScene = "Setting";
    private static string allCoatScene = "AllCoat";
    private string harfCoatScene = "";
    private static string endScene = "EndScene";

    // スタートボタン
    public void StartButton()
    {
        // 効果音を鳴らす
        SoundManager.soundManager.MakeBrightSelectSound();
        SoundManager.soundManager.MakeCheerSound();

        // 少し時間をおいてからチュートリアル画面を呼び出す
        StartCoroutine(StartButtonCoroutine());
    }

    private IEnumerator StartButtonCoroutine()
    {
        yield return new WaitForSeconds(2.0f);
        TutorialScreen.tutorialScreen.ActiveScreen();       // チュートリアル画面表示
    }

    // タイトル画面読み込み
    public static void LoadTitleScene()
    {
        SceneManager.LoadScene(titleScene);
    }

    // 対戦画面読み込み
    public static void LoadPlayScene()
    {
        SceneManager.LoadScene(allCoatScene);
    }

    // 設定画面読み込み
    public static void LoadSettingScene()
    {
        SceneManager.LoadScene(settingScene);
    }

}
