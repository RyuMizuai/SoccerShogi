using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip motionSound;       // 駒を打つ時の音
    public AudioClip shortWhistleSound; // 短い笛の音
    public AudioClip longWhistleSound;  // 長い笛の音
    public AudioClip goalSound;         // ゴールの声
    public AudioClip cheerSound;        // 歓声
    public AudioClip passSound;     // ボールを蹴る音


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

    // 試合開始の笛を鳴らす
    public void MakeGameStartSound()
    {
        audioSource.PlayOneShot(longWhistleSound);
    }

    // 試合終了の笛を鳴らす
    public IEnumerator MakeGameEndSound()
    {
        // 音声編集が出来なかったからゴリ押しした
        audioSource.PlayOneShot(shortWhistleSound);
        yield return new WaitForSeconds(0.8f);
        audioSource.PlayOneShot(shortWhistleSound);
        yield return new WaitForSeconds(0.8f);
        audioSource.PlayOneShot(shortWhistleSound);
        yield return new WaitForSeconds(0.15f);
        audioSource.PlayOneShot(longWhistleSound);
    }

    // ゴール時のサウンドを鳴らす
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
