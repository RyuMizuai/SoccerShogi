using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour, IPointerUpHandler
{
    Slider volumeSlider;

    AudioSource audioSource;

    // Start is called before the first frame update
    private void Start()
    {
        volumeSlider = GetComponent<Slider>();                          // ���g��Slider
        GameObject audio = GameObject.FindGameObjectWithTag("Audio");   // �V�[�����Audio�I�u�W�F�N�g
        audioSource = audio.GetComponent<AudioSource>();                // AudioSource���擾

        volumeSlider.value = audioSource.volume;                        // �X���C�_�[�̒l�����݂̃{�����[���ɂ���
    }

    // �N���b�N�𗣂����Ƃ��ɐݒ艹��炷
    public void OnPointerUp(PointerEventData eventData)
    {
        SoundManager.soundManager.MakeMotionSound();
    }

    // ���ʂ�ݒ肷��
    public void SetVolume()
    {
        // �v���C���̎�
        if (GameManager.gameState == "Playing")
        {
            audioSource.volume = volumeSlider.value;
        }
    }
}
