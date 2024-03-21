using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScenesAtRandom : MonoBehaviour
{
    [SerializeField]
    private string[] sceneNames;    // シーン名

    // Start is called before the first frame update
    private void Start()
    {
        System.Random random = new System.Random();     // 乱数の種
        int index = random.Next(0, sceneNames.Length);  // 乱数発生
        SceneManager.LoadScene(sceneNames[index]);      // シーン読み込み
    }

}
