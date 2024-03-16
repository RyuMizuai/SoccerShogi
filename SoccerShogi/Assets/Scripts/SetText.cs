using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class SetText : MonoBehaviour
{
    // ���O����͂���t�B�[���h
    public TMP_InputField firstPlayerNameInput;
    public TMP_InputField secondPlayerNameInput;

    // �v���C���[�̖��O
    public static string firstPlayerName = "���O";
    public static string secondPlayerName;

    // �ݒ芮��
    public void FinishSetting()
    {
        // ���������͂ł��Ă���Ζ��O��ێ����ăV�[����ύX
        if (firstPlayerNameInput.text != "" && secondPlayerNameInput.text != "")
        {
            firstPlayerName = firstPlayerNameInput.text;
            secondPlayerName = secondPlayerNameInput.text;
            SceneManager.LoadScene("AllCoat");
            SoundManager.soundManager.MakeGameStartSound();
        }
        else
        {
            // �G���[�����o��
            Debug.Log("���O����͂��Ă�������");
        }
    }
}
