using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip motionSound;   // 駒を打つ時の音
    public AudioClip shortWhistleSound; // 短い笛の音
    public AudioClip longWhistleSound;  // 長い笛の音


    AudioSource audioSource;

    public static SoundManager soundManager;    // 自身を保持する変数

    private void Awake()
    {
        if (soundManager == null)
        {
            soundManager = this;    // static変数に自分を保存する
            // シーンが変わってもゲームオブジェクトを破棄しない
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);    // ゲームオブジェクトを破棄
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();  // 自身のコンポーネントを取得
    }

    // 駒を打つ音を鳴らす
    public void MakeMotionSound()
    {
        audioSource.PlayOneShot(motionSound);
    }

    // 試合終了の笛を鳴らす
    public void MakeGameEndSound()
    {

    }

}
