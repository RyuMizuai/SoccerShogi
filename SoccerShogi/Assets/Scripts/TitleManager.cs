using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    static string startScene = "StartScene";
    string allCoatScene = "9_9";
    string harfCoatScene = "";
    static string endScene = "EndScene";

    public void StartButton()
    {
        SceneManager.LoadScene(allCoatScene);
    }
   
    public static void LoadStartScene()
    {
        SceneManager.LoadScene(startScene);
    }
}
