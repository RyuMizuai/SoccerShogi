using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScreen : MonoBehaviour
{
    public static TutorialScreen tutorialScreen;

    [SerializeField]
    private GameObject tutorialPanel;

    [SerializeField]
    private Button nextButton;

    [SerializeField]
    private Button backButton;

    [SerializeField]
    private GameObject closeButton;

    [SerializeField]
    private GameObject[] tutorialPages; // �`���[�g���A����ʂ̃y�[�W
    private int pageCount;              // �y�[�W��
    private int activePageIndex = 0;    // ���ݕ\�����Ă���y�[�W

    // Start is called before the first frame update
    void Start()
    {
        tutorialScreen = GetComponent<TutorialScreen>();
        tutorialPanel.SetActive(false);
        closeButton.SetActive(false);
        pageCount = tutorialPages.Length;
    }

    // �X�N���[����\��
    public void ActiveScreen()
    {
        tutorialPanel.SetActive(true);  // �\��
        TurnPage();                     // �n�߂̃y�[�W���J��
    }

    // ���փ{�^��
    public void NextButton()
    {
        activePageIndex++;  // �y�[�W�𑝂₷
        TurnPage();         // �y�[�W���X�V
        SoundManager.soundManager.MakeBrightSelectSound();
    }

    // �O�փ{�^��
    public void BackButton()
    {
        activePageIndex--;  // �y�[�W�����炷
        TurnPage();         // �y�[�W���X�V
        SoundManager.soundManager.MakeSadSelectSound();
    }

    // �y�[�W���߂���
    public void TurnPage()
    {
        foreach (GameObject page in tutorialPages)
        {
            page.SetActive(false);         // ��x��ʂ��\��
        }

        tutorialPages[activePageIndex].SetActive(true);   // �󂯎�����ԍ��̃y�[�W��\��

        // ��ԑO�̃y�[�W�Ȃ�߂�{�^���������Ȃ�����
        if (activePageIndex == 0)
        {
            backButton.interactable = false;
        }
        else
        {
            backButton.interactable = true;
        }

        if (activePageIndex == pageCount - 1)
        {
            closeButton.SetActive(true);
        }
        else
        {
            closeButton.SetActive(false);
        }
    }

    public void CloseButton()
    {
        SoundManager.soundManager.MakeBrightSelectSound();
        TitleManager.LoadSettingScene();
    }
}
