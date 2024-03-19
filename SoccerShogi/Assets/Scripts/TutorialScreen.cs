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
    private GameObject[] tutorialPages; // チュートリアル画面のページ
    private int pageCount;              // ページ数
    private int activePageIndex = 0;    // 現在表示しているページ

    // Start is called before the first frame update
    void Start()
    {
        tutorialScreen = GetComponent<TutorialScreen>();
        tutorialPanel.SetActive(false);
        closeButton.SetActive(false);
        pageCount = tutorialPages.Length;
    }

    // スクリーンを表示
    public void ActiveScreen()
    {
        tutorialPanel.SetActive(true);  // 表示
        TurnPage();                     // 始めのページを開く
    }

    // 次へボタン
    public void NextButton()
    {
        activePageIndex++;  // ページを増やす
        TurnPage();         // ページを更新
        SoundManager.soundManager.MakeBrightSelectSound();
    }

    // 前へボタン
    public void BackButton()
    {
        activePageIndex--;  // ページを減らす
        TurnPage();         // ページを更新
        SoundManager.soundManager.MakeSadSelectSound();
    }

    // ページをめくる
    public void TurnPage()
    {
        foreach (GameObject page in tutorialPages)
        {
            page.SetActive(false);         // 一度画面を非表示
        }

        tutorialPages[activePageIndex].SetActive(true);   // 受け取った番号のページを表示

        // 一番前のページなら戻るボタンを押せなくする
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
