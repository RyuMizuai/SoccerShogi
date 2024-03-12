using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip motionSound;   // ���ł��̉�
    public AudioClip shortWhistleSound; // �Z���J�̉�
    public AudioClip longWhistleSound;  // �����J�̉�


    AudioSource audioSource;

    public static SoundManager soundManager;    // ���g��ێ�����ϐ�

    private void Awake()
    {
        if (soundManager == null)
        {
            soundManager = this;    // static�ϐ��Ɏ�����ۑ�����
            // �V�[�����ς���Ă��Q�[���I�u�W�F�N�g��j�����Ȃ�
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);    // �Q�[���I�u�W�F�N�g��j��
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();  // ���g�̃R���|�[�l���g���擾
    }

    // ���ł���炷
    public void MakeMotionSound()
    {
        audioSource.PlayOneShot(motionSound);
    }

    // �����I���̓J��炷
    public void MakeGameEndSound()
    {

    }

}
