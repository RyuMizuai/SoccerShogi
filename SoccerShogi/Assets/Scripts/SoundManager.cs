using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip motionSound;       // ���ł��̉�
    public AudioClip shortWhistleSound; // �Z���J�̉�
    public AudioClip longWhistleSound;  // �����J�̉�
    public AudioClip goalSound;         // �S�[���̐�
    public AudioClip cheerSound;        // ����
    public AudioClip passSound;     // �{�[�����R�鉹


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

    // �����J�n�̓J��炷
    public void MakeGameStartSound()
    {
        audioSource.PlayOneShot(longWhistleSound);
    }

    // �����I���̓J��炷
    public IEnumerator MakeGameEndSound()
    {
        // �����ҏW���o���Ȃ���������S����������
        audioSource.PlayOneShot(shortWhistleSound);
        yield return new WaitForSeconds(0.8f);
        audioSource.PlayOneShot(shortWhistleSound);
        yield return new WaitForSeconds(0.8f);
        audioSource.PlayOneShot(shortWhistleSound);
        yield return new WaitForSeconds(0.15f);
        audioSource.PlayOneShot(longWhistleSound);
    }

    // �S�[�����̃T�E���h��炷
    public void MakeGoalSound()
    {
        audioSource.PlayOneShot(goalSound);
        audioSource.PlayOneShot(cheerSound);
    }

    public void MakePassSound()
    {
        audioSource.PlayOneShot(passSound);
    }
}
