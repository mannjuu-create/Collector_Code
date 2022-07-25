using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// タイトル画面に戻るスクリプト.
/// </summary>

public class GoTitleScene : MenuBase
{
    /////////////////////////////////////////
    /// サウンド.
    /////////////////////////////////////////
    private AudioSource audioSource;
    public  AudioClip   decision;       // 決定.

    /////////////////////////////////////////
    /// 変数.
    /////////////////////////////////////////
    // TITLE画像.
    private GameObject titleText;

    /////////////////////////////////////////
    /// 定数.
    /////////////////////////////////////////
    private const float VOLUME            = 0.1f;   // 音量.
    private const float CHANGE_TIME       = 1.0f;   // 拡大縮小の切り替えの時間.
    private const float STEALTH_TIME      = 1.0f;   // 透明になるまでの時間.
    private const float ENLARGEMENT_SPEED = 100.0f; // 決定後の拡大速度.
    private static readonly Vector3 DECISION_SIZE = new Vector3(0.5f, 0.5f, 1.0f);     // 決定直後の大きさ.

    /////////////////////////////////////////
    /// 初期化.
    /////////////////////////////////////////
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        titleText = GameObject.Find("TITLE");
    }

    /////////////////////////////////////////
    /// 更新.
    /////////////////////////////////////////
    void Update()
    {
        if (!decisionFlag)
        {
            // 選択中のメニューを動かす.
            ScalChange(titleText, CHANGE_TIME);
        }
        else
        {
            // 透明にしていく.
            Stealth(titleText, DECISION_SIZE, STEALTH_TIME, ENLARGEMENT_SPEED);
            return;
        }

        // Spaceキーで実行.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Decision(audioSource, decision, VOLUME, "GoTitle", 2.0f);
        }
    }

    /////////////////////////////////////////
    /// タイトル画面に戻る.
    /////////////////////////////////////////
    void GoTitle()
    {
        // シーン遷移.
        SceneManager.LoadScene("TitleScene");
    }
}
