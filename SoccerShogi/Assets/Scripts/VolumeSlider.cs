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
        volumeSlider = GetComponent<Slider>();                          // 自身のSlider
        GameObject audio = GameObject.FindGameObjectWithTag("Audio");   // シーン上のAudioオブジェクト
        audioSource = audio.GetComponent<AudioSource>();                // AudioSourceを取得

        volumeSlider.value = audioSource.volume;                        // スライダーの値を現在のボリュームにする
    }

    // クリックを離したときに設定音を鳴らす
    public void OnPointerUp(PointerEventData eventData)
    {
        SoundManager.soundManager.MakeMotionSound();
    }

    // 音量を設定する
    public void SetVolume()
    {
        // プレイ中の時
        if (GameManager.gameState == "Playing")
        {
            audioSource.volume = volumeSlider.value;
        }
    }
}
