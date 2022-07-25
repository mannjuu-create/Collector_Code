using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 一時停止するスクリプト.
/// </summary>

public class PauseScript : MenuBase
{
    /////////////////////////////////////////
    /// 参照するスクリプト.
    /////////////////////////////////////////
    protected CountDown countScript; // CountDownスクリプト.

    /////////////////////////////////////////
    /// サウンド.
    /////////////////////////////////////////
    private AudioSource audioSource;
    public AudioClip select;         // 選択肢変更.
    public AudioClip decision;       // 決定.

    /////////////////////////////////////////
    /// 変数.
    /////////////////////////////////////////
    // カメラを格納する変数.
    private Camera mainCamera;        // メインカメラ.
    private Camera subCamera;         // サブカメラ.

    // メニュー一覧.
    enum MENU
    {
        Continue = 0,   // ゲームを再開.
        Retry = 1,      // ゲームをやり直す.
        Exit = 2        // タイトルに戻る.
    }
    private GameObject[] pauseMenu = new GameObject[3];     // タイトルのメニュー.

    private GameObject pauseUI;                     // ポーズした時に表示するUI.
    [HideInInspector] public bool isSwitch;         // 切り替えたフラグ（StalkerSwitch.cs）で使用.
    [HideInInspector] public bool pauseFrag;        // 一時停止中かどうかのフラグ.
    [HideInInspector] public bool timeOverFrag;     // タイムオーバー後はポーズできないようにするためのフラグ.

    /////////////////////////////////////////
    /// 定数.
    /////////////////////////////////////////
    private const float     VOLUME                = 0.2f;   // 音量.
    private const int       PAUSE_MENU_NUM        = 3;      // ポーズメニューの数.
    private const float     CHANGE_TIME           = 1.0f;   // 拡大縮小の切り替えの時間.
    private const float     STEALTH_TIME          = 1.0f;   // 透明になるまでの時間.
    private const float     ENLARGEMENT_SPEED     = 100.0f; // 決定後の拡大速度.
    private static readonly Vector3 OTHER_OPTION_SIZE = new Vector3(1.5f, 1.5f, 1.0f);     // 他の選択肢の大きさ.
    private static readonly Vector3 DECISION_SIZE     = new Vector3(0.5f, 0.5f, 1.0f);     // 決定直後の大きさ.

    /////////////////////////////////////////
    /// 初期化.
    /////////////////////////////////////////
    void Awake()
    {
        // CountDownスクリプト.
        countScript = this.GetComponent<CountDown>();

        // AudioSource
        audioSource = GetComponent<AudioSource>();

        // カメラの格納.
        mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();       // メインカメラ.
        subCamera  = GameObject.Find("Sub Camera").GetComponent<Camera>();        // サブカメラ.
        // サブカメラは最初に非アクティブにする.
        subCamera.gameObject.SetActive(false);

        // ポーズメニュー.
        pauseUI      = GameObject.FindGameObjectWithTag("Pause");   // ポーズUI.
        pauseMenu[0] = GameObject.Find("Continue");
        pauseMenu[1] = GameObject.Find("Retry");
        pauseMenu[2] = GameObject.Find("EXIT");
        pauseUI.SetActive(false);   // ポーズUIを非アクティブに.

        pauseFrag    = false;      // 一時停止フラグ.
        timeOverFrag = false;      // タイムオーバーフラグ.
    }

    /////////////////////////////////////////
    /// 更新.
    /////////////////////////////////////////
    void Update()
    {
        // カウントダウン中は一時停止出来ないようにする.
        if (countScript.startFrag == false)
        {
            return;
        }

        // タイムオーバー後はスルー.
        if(timeOverFrag)
        {
            return;
        }

        if (decisionFlag)
        {
            // 透明にしていく.
            Stealth(pauseMenu, DECISION_SIZE, STEALTH_TIME, ENLARGEMENT_SPEED);
            return;
        }

        // ゲームプレイ中にFキーが押されたらポーズ画面にする.
        if (!pauseFrag && Input.GetKeyDown(KeyCode.F))
        {
            // サブカメラをアクティブに切り替え.
            subCamera.gameObject.SetActive(true);
            //　ポーズUIをアクティブに切り替え.
            pauseUI.SetActive(true);
            //　一時停止フラグを立てる.
            pauseFrag = true;
        }
        // ポーズ中なら.
        else if(pauseUI.activeSelf)
        {
            // メニューを選択する.
            Select(pauseMenu, PAUSE_MENU_NUM, KeyCode.UpArrow, KeyCode.DownArrow, OTHER_OPTION_SIZE, audioSource, select, VOLUME);
            // 選択中のメニューを動かす.
            ScalChange(pauseMenu, CHANGE_TIME);

            // Spaceキーで実行.
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Decision(audioSource, decision, VOLUME, "MenuExecution", 2.0f);
            }
        }
    }

    /////////////////////////////////////////
    /// 選択したメニューを実行.
    /////////////////////////////////////////
    void MenuExecution()
   {
        switch(menuNum)
        {
            // ゲームを再開.
            case (int)MENU.Continue:
                Continue();
                break;
            // ゲームをやり直す.
            case (int)MENU.Retry:
                Retry();
                break;
            // タイトルに戻る.
            case (int)MENU.Exit:
                Exit();
                break;
            default:
                break;
        }
   }

    /////////////////////////////////////////
    /// ゲームを再開.
    /////////////////////////////////////////
    void Continue()
    {
        // 大きさ.
        pauseMenu[menuNum].transform.localScale = new Vector3(1.5f, 1.5f, 1.0f);
        // 透明度.
        Color color = image.color;
        color.a = 1.0f;
        image.color = color;

        /// 非アクティブに切り替え.
        subCamera.gameObject.SetActive(false);  // サブカメラ.
        pauseUI.SetActive(false);               //　ポーズUI.

        /// フラグを下ろす.
        isSwitch     = false;       // 切り替えたフラグ.
        pauseFrag    = false;       // 一時停止フラグ.
        decisionFlag = false;       // 決定フラグ.
        firstFlag    = false;       // 実行後一回目に通ったフラグ.
    }

    /////////////////////////////////////////
    /// ゲームをやり直す.
    /////////////////////////////////////////
    void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /////////////////////////////////////////
    /// タイトルに戻る.
    /////////////////////////////////////////
    void Exit()
    {
        SceneManager.LoadScene("TitleScene");
    }
}
