using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    static string startScene = "StartScene";
    static string allCoatScene = "9_9";
    string harfCoatScene = "";
    static string endScene = "EndScene";

    public void StartButton()
    {
        LoadPlayScene();
        SoundManager.soundManager.MakeGameStartSound();
    }

    public static void LoadPlayScene()
    {
        SceneManager.LoadScene(allCoatScene);
    }

    public static void LoadStartScene()
    {
        SceneManager.LoadScene(startScene);
    }
}
