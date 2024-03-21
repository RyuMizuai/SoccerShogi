using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingSript : MonoBehaviour
{
    // ���O����͂���t�B�[���h
    [SerializeField]
    private TMP_InputField firstPlayerNameInput;

    [SerializeField]
    private TMP_InputField secondPlayerNameInput;

    // �v���C���[�̖��O
    public static string firstPlayerName;
    public static string secondPlayerName;

    // �ő�X�R�A
    public static int maxScore = 1;

    // �X�R�A�ݒ�{�^��
    [SerializeField]
    private Button[] scoreButtons;

    // �X�R�A�{�^���̐�
    private int scoreButtonCount;

    // �F
    Color32 white = new Color32(255, 255, 255, 255);    // ��
    Color32 gray = new Color32(200, 200, 200, 255);     // �O���[
    Color32 pressedColor = new Color32(255, 230, 150, 255);    // �x�[�W��


    // ��Փx�ݒ�{�^��
    [SerializeField]
    private Button difButton;

    private void Start()
    {
        scoreButtonCount = scoreButtons.Length;

        // �[�̃{�^���̐F��ς���
        scoreButtons[0].image.color = pressedColor;
        difButton.image.color = pressedColor;

        maxScore = 1;
    }

    // �ݒ芮��
    public void FinishSetting()
    {
        // ���O��ێ����ăV�[����ύX
        /*if (firstPlayerNameInput.text != "" && secondPlayerNameInput.text != "")
        {
            firstPlayerName = firstPlayerNameInput.text;
            secondPlayerName = secondPlayerNameInput.text;
        }
        else
        {
            // �G���[�����o��
            Debug.Log("���O����͂��Ă�������");
        }*/

        firstPlayerName = (firstPlayerNameInput.text != "") ? firstPlayerNameInput.text : "���Ȃ�";
        secondPlayerName = (secondPlayerNameInput.text != "") ? secondPlayerNameInput.text : "������";

        TitleManager.LoadPlayScene();
        SoundManager.soundManager.MakeGameStartSound();
    }

    // �X�R�A�{�^��
    public void ClickScoreButon(int index)
    {
        // �{�^���ԍ��̕ϐ����󂯎��
        // �F��ύX
        for (int i = 1; i <= scoreButtonCount; i++)
        {
            // �������{�^���Ȃ�O���[�ɂ���
            if (i == index)
            {
                scoreButtons[i - 1].image.color = pressedColor;
            }
            // �����łȂ���Δ��ɂ���
            else
            {
                scoreButtons[i - 1].image.color = white;
            }
        }

        maxScore = index;   // �ő�X�R�A��ݒ�
    }
}
