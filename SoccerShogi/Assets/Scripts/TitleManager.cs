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

    // �X�^�[�g�{�^��
    public void StartButton()
    {
        // ���ʉ���炷
        SoundManager.soundManager.MakeBrightSelectSound();
        SoundManager.soundManager.MakeCheerSound();

        // �������Ԃ������Ă���`���[�g���A����ʂ��Ăяo��
        StartCoroutine(StartButtonCoroutine());
    }

    private IEnumerator StartButtonCoroutine()
    {
        yield return new WaitForSeconds(2.0f);
        TutorialScreen.tutorialScreen.ActiveScreen();       // �`���[�g���A����ʕ\��
    }

    // �^�C�g����ʓǂݍ���
    public static void LoadTitleScene()
    {
        SceneManager.LoadScene(titleScene);
    }

    // �ΐ��ʓǂݍ���
    public static void LoadPlayScene()
    {
        SceneManager.LoadScene(allCoatScene);
    }

    // �ݒ��ʓǂݍ���
    public static void LoadSettingScene()
    {
        SceneManager.LoadScene(settingScene);
    }

}
