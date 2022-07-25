using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// ゲーム開始時のカウントダウンをする処理.
/// カウントダウンが終わったら経過時間を計測する.
/// </summary>

public class CountDown : MonoBehaviour
{
    /////////////////////////////////////////
    /// サウンド.
    /////////////////////////////////////////
    private AudioSource audioSource;
    public  AudioClip   whistle;         // ホイッスル.

    /////////////////////////////////////////
    /// 使用する変数.
    /////////////////////////////////////////
    // 表示テキスト.
    private TextMeshProUGUI countText;
    // START画像.
    private GameObject startImage;
    // カウントダウンの時間.
    private float countdown;
    // カウント表示用の変数.
    private int time;
    // 一度通ったフラグ.
    private bool firstFlag;
    // ゲームを開始したフラグ.
    [HideInInspector] public bool startFrag;

    /////////////////////////////////////////
    /// 定数.
    /////////////////////////////////////////
    private float COUNTDOWN = 3.0f;      // ゲーム開始時のカウントダウンの時間.

    void Awake()
    {
        // AudioSource
        audioSource = GetComponent<AudioSource>();

        // 表示テキスト.
        countText = GameObject.FindWithTag("CountDown").GetComponent<TextMeshProUGUI>();
        // START画像.
        startImage = GameObject.FindGameObjectWithTag("START");
        // START画像を非表示.
        startImage.SetActive(false);
        // カウントダウンの時間.
        countdown = COUNTDOWN;
        // カウント表示用の変数.
        time = 0;
        // 一度通ったフラグ.
        firstFlag = false;
        // ゲーム開始フラグを初期化.
        startFrag = false;
    }

    void Update()
    {
        // フラグが立っているならスルー.
        if(firstFlag)
        {
            return;
        }

        if (startFrag == false)
        {
            // カウントダウンを減らす.
            countdown -= Time.deltaTime;
            // 表示用にプラス１してからint型に変換.
            time = (int)countdown + 1;
            // テキストの更新.
            countText.text = ToSpriteAssetFont.FontConversion(time.ToString());

            // カウントダウンが0秒以下になっていたら
            if (countdown <= 0)
            {
                // テキストを非表示にする.
                countText.enabled = false;
                // 効果音を鳴らす.
                audioSource.PlayOneShot(whistle, 0.1f);
                // START画像を表示する.
                startImage.SetActive(true);
                // フラグを立てる.
                firstFlag = true;
                // １秒後にゲームスタートの関数を呼ぶ.
                Invoke("GameStart", 1.0f);
            }
        }
    }

    private void GameStart()
    {
        // 開始フラグを立てる.
        startFrag = true;
        // START画像を非表示.
        startImage.SetActive(false);
    }
}
