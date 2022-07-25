using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// タイトル画面のメニューを制御するスクリプト.
/// </summary>

public class TitleMenu : MenuBase
{
    /////////////////////////////////////////
    /// サウンド.
    /////////////////////////////////////////
    private AudioSource audioSource;
    public AudioClip select;         // 選択肢変更.
    public AudioClip decision;       // 決定.

    /////////////////////////////////////////
    /// 変数.
    /////////////////////////////////////////
    // メニュー一覧.
    enum MENU
    {
        Start = 0,     // ゲームを再開.
        Finish = 1,     // ゲームをやり直す.
    }
    private GameObject[] titleMenu = new GameObject[2];     // タイトルのメニュー.

    /////////////////////////////////////////
    /// 定数.
    /////////////////////////////////////////
    private const int       TITLE_MENU_NUM       = 2;      // ポーズメニューの数.
    private const int       RANKING_NUM          = 10;     // ランキングの最大数.
    private const float     VOLUME               = 0.1f;   // 音量.
    private const float     CHANGE_TIME          = 1.0f;   // 拡大縮小の切り替えの時間.
    private const float     STEALTH_TIME         = 1.0f;   // 透明になるまでの時間.
    private const float     ENLARGEMENT_SPEED    = 100.0f; // 決定後の拡大速度.
    private static readonly Vector3 OTHER_OPTION_SIZE = new Vector3(0.7f, 0.7f, 1.0f);     // 他の選択肢の大きさ.
    private static readonly Vector3 DECISION_SIZE     = new Vector3(0.5f, 0.5f, 1.0f);     // 決定直後の大きさ.

    /////////////////////////////////////////
    /// 初期化.
    /////////////////////////////////////////
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // タイトルのメニュー.
        titleMenu[0] = GameObject.Find("START");
        titleMenu[1] = GameObject.Find("FINISH");
    }

    /////////////////////////////////////////
    /// 更新.
    /////////////////////////////////////////
    void Update()
    {
        if (!decisionFlag)
        {
            // 選択中のメニューを動かす.
            ScalChange(titleMenu, CHANGE_TIME);
        }
        else
        {
            // 透明にしていく.
            Stealth(titleMenu, DECISION_SIZE, STEALTH_TIME, ENLARGEMENT_SPEED);
            return;
        }

        // メニューを選択する.
        Select(titleMenu, TITLE_MENU_NUM, KeyCode.UpArrow, KeyCode.DownArrow, OTHER_OPTION_SIZE, audioSource, select, VOLUME);
        // Spaceキーで実行.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Decision(audioSource, decision, VOLUME, "MenuExecution", 2.0f);
        }
    }

    /////////////////////////////////////////
    /// 選択したメニューを実行.
    /////////////////////////////////////////
    void MenuExecution()
    {
        switch (menuNum)
        {
            // ゲームを開始.
            case (int)MENU.Start:
                audioSource.PlayOneShot(decision);
                GameStart();
                break;

            // ゲームを終了.
            case (int)MENU.Finish:
                Finish();
                break;

            default:
                break;
        }
    }

    /////////////////////////////////////////
    /// ゲーム開始.
    /////////////////////////////////////////
    void GameStart()
    {
        SceneManager.LoadScene("StageSelectScene");
    }

    /////////////////////////////////////////
    /// ゲーム終了.
    /////////////////////////////////////////
    void Finish()
    {
        // ゲームを終了.
        Application.Quit();
    }
}
