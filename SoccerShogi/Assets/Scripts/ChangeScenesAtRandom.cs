using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScenesAtRandom : MonoBehaviour
{
    [SerializeField]
    private string[] sceneNames;    // �V�[����

    // Start is called before the first frame update
    private void Start()
    {
        System.Random random = new System.Random();     // �����̎�
        int index = random.Next(0, sceneNames.Length);  // ��������
        SceneManager.LoadScene(sceneNames[index]);      // �V�[���ǂݍ���
    }

}
