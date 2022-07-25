using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// ステージ選択をするスクリプト.
/// </summary>

public class StageSelect : MenuBase
{
    /////////////////////////////////////////
    /// サウンド.
    /////////////////////////////////////////
    private AudioSource audioSource;
    public  AudioClip select;         // 選択肢変更.
    public  AudioClip decision;       // 決定.

    /////////////////////////////////////////
    /// 変数.
    /////////////////////////////////////////
    private GameObject[] stageList = new GameObject[2];     // タイトルのメニュー.

    /////////////////////////////////////////
    /// 定数.
    /////////////////////////////////////////
    private const int   STAGE_NUM         = 2;      // ステージの数.
    private const float VOLUME            = 0.1f;   // 音量.
    private const float CHANGE_TIME       = 1.0f;   // 拡大縮小の切り替えの時間.
    private const float STEALTH_TIME      = 1.0f;   // 透明になるまでの時間.
    private const float ENLARGEMENT_SPEED = 100.0f; // 決定後の拡大速度.
    private static readonly Vector3 OTHER_OPTION_SIZE = new Vector3(1.3f, 1.3f, 1.0f);     // 他の選択肢の大きさ.
    private static readonly Vector3 DECISION_SIZE     = new Vector3(1.0f, 1.0f, 1.0f);     // 決定直後の大きさ.

    /////////////////////////////////////////
    /// 初期化.
    /////////////////////////////////////////
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // ステージの種類.
        stageList[0] = GameObject.Find("Stage1");
        stageList[1] = GameObject.Find("Stage2");
    }

    /////////////////////////////////////////
    /// 更新.
    /////////////////////////////////////////
    void Update()
    {
        if (!decisionFlag)
        {
            // 選択中のメニューを動かす.
            ScalChange(stageList, CHANGE_TIME);
        }
        else
        {
            // 透明にしていく.
            Stealth(stageList, DECISION_SIZE, STEALTH_TIME, ENLARGEMENT_SPEED);
            return;
        }

        // ステージ選択.
        Select(stageList, STAGE_NUM, KeyCode.LeftArrow, KeyCode.RightArrow, OTHER_OPTION_SIZE, audioSource, select, VOLUME);

        // Spaceキーで実行.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Decision(audioSource, decision, VOLUME, "GoGameScene", 1.0f);
        }
    }

    /////////////////////////////////////////
    /// シーン遷移.
    /////////////////////////////////////////
    void GoGameScene()
    {
        // ステージの番号を保存.
        PlayerPrefs.SetInt("STAGE", menuNum);

        SceneManager.LoadScene("GameScene");
    }
}
