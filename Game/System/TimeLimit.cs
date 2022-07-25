using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// ゲームの制限時間を図るスクリプト.
/// </summary>

public class TimeLimit : MonoBehaviour
{
    /////////////////////////////////////////
    /// 参照するスクリプト.
    /////////////////////////////////////////
    private CountDown   countScript; // CountDownスクリプト.
    private PauseScript pauseScript; // PauseScriptスクリプト.
    private GetJewel    getScript;   // Getjewelスクリプト.

    /////////////////////////////////////////
    /// サウンド.
    /////////////////////////////////////////
    private AudioSource audioSource;
    public  AudioClip whistle;         // ホイッスル.
    public  AudioClip spawn;           // 出現.

    /////////////////////////////////////////
    /// 使用する変数.
    /////////////////////////////////////////
    private Slider     timeSlider;       // 制限時間を表示するバー.
    private float      time;             // 現在の残り時間.
    private float      decreaseTime;     // 制限時間をリセットする時に最大値から減らす時間.
    private float      resetNum;         // 制限時間をリセットした回数.
    private bool       dangerousFlag;    // 制限時間が迫っているフラグ.
    private bool       soulSpawnFlag;    // Soulを出現させたフラグ.
    private GameObject soulObject;       // Soulのオブジェクト.
    private GameObject overUI;           // タイムオーバーした時に表示するUI
    [HideInInspector] public bool waitFrag;         // 宝石をすべて取得した状態で、次の宝石が設置されるまでの待ち時間に、時間を減らさないためのフラグ.
    [HideInInspector] public bool timeOverFrag;     // タイムオーバーフラグ.

    /////////////////////////////////////////
    /// 定数.
    /////////////////////////////////////////
    private const float MAX_TIME_LIMIT = 60f;       // 制限時間の最大値.
    private const float MIN_TIME_LIMIT = 30f;       // 制限時間の最小値.
    private const float DECREASE_TIME  = 2;         // 一度リセットするたびに減らす固定の時間.
    private const int   SOUL_SPAWN_NUM = 5;         // Soulを出現させるリセット回数.

    void Awake()
    {
        countScript = GameObject.Find("SceneManager").GetComponent<CountDown>();                // CountDownスクリプト.
        pauseScript = GameObject.Find("SceneManager").GetComponent<PauseScript>();              // PauseScriptスクリプト.
        getScript   = GameObject.FindGameObjectWithTag("Player").GetComponent<GetJewel>();      // GetJewelスクリプト.

        audioSource = GetComponent<AudioSource>();                  // AudioSource

        timeSlider    = GameObject.FindGameObjectWithTag("TimeLimit").GetComponent<Slider>();      // バーの取得.
        time          = MAX_TIME_LIMIT;                              // 制限時間をリセット.
        decreaseTime  = 0;                                           // リセット時に減らす時間をリセット.
        resetNum      = 0;                                           // リセットした回数.
        dangerousFlag = false;                                       // 制限時間が迫っているフラグ.
        soulSpawnFlag = false;                                       // Soulを出現させたフラグ.
        soulObject    = GameObject.FindGameObjectWithTag("Soul");    // Soulのオブジェクト.
        overUI = GameObject.FindGameObjectWithTag("TIMEOVER");       // タイムオーバーした時に表示するUI
        waitFrag      = false;                                       // 待つフラグをリセット.
        timeOverFrag  = false;                                       // タイムオーバーフラグをリセット.

        timeSlider.value = time;                                    // バーのゲージ量をリセット.
        overUI.SetActive(false);                                    // UIを非表示に.
        soulObject.SetActive(false);                                // Soulを非アクティブに.
    }

    void Update()
    {

        // カウントダウン中ならスルー.
        if (!countScript.startFrag)
        {
            return;
        }

        // ゲームオーバーならスルー.
        if (timeOverFrag)
        {
            return;
        }

        // ポーズ中ならスルー.
        if (pauseScript.pauseFrag)
        {
            return;
        }

        // フラグが立っていたらスルー.
        if (waitFrag)
        {
            return;
        }

        // 制限時間を減らす.
        time -= Time.deltaTime;
        // ゲージを調整する.
        timeSlider.value = time / (MAX_TIME_LIMIT - decreaseTime);

        // 制限時間が10秒を切ったら.
        if (dangerousFlag == false && time <= 10.0f)
        {
            // カチコチ音を鳴らす.
            audioSource.Play();
            // フラグを立てる.
            dangerousFlag = true;
        }

        // 制限時間が0以下になったらフラグを立ててシーン遷移.
        if(time <= 0)
        {
            // フラグを立てる.
            timeOverFrag = true;
            // Uiを表示する.
            overUI.SetActive(true);
            // pauseScriptのフラグを立てる.
            pauseScript.timeOverFrag = true;
            // カチコチ音を止める.
            audioSource.Stop();
            // 効果音を鳴らす.
            audioSource.PlayOneShot(whistle, 0.1f);
            // 5秒後にシーン遷移.
            Invoke("SceneChange", 5.0f);
        }
    }

    // 制限時間をリセットする関数.
    public void TimeLimitReset()
    {
        // 次のリセット時の制限時間を計算.
        float limitTime = MAX_TIME_LIMIT - decreaseTime - DECREASE_TIME;
        // 制限時間が最小値以上なら減らす時間を増加.
        if(limitTime >= MIN_TIME_LIMIT)
        {
            // 減らす時間を増加.
            decreaseTime += DECREASE_TIME;
        }

        // 制限時間をリセット.
        time = MAX_TIME_LIMIT - decreaseTime;
        // 制限時間をリセットした回数をプラス.
        resetNum++;

        // カチコチ音を止める.
        audioSource.Stop();

        // 出現フラグが立っておらず、リセット回数が一定以上になったら
        if (soulSpawnFlag == false && resetNum > SOUL_SPAWN_NUM)
        {
            // Soulを出現.
            soulObject.SetActive(true);
            // 効果音を鳴らす.
            audioSource.PlayOneShot(spawn, 1.0f);
            // 出現フラグを立てる.
            soulSpawnFlag = true;
        }

        // フラグを下ろす.
        waitFrag = false;
    }

    /////////////////////////////////////////
    /// シーン遷移.
    /////////////////////////////////////////
    void SceneChange()
    {
        //イベントに登録.
        SceneManager.sceneLoaded += GameOverSceneLoaded;

        //シーン切り替え.
        SceneManager.LoadScene("ResultScene");
    }

    void GameOverSceneLoaded(Scene next, LoadSceneMode mode)
    {
        // 今回のスコアを保存.
        PlayerPrefs.SetInt("SCORE", getScript.totalGetJewel);

        // イベントから削除
        SceneManager.sceneLoaded -= GameOverSceneLoaded;
    }
}
