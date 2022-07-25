using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの基盤スクリプト.
/// </summary>

public class PlayerBase : CharactorBase
{
    /////////////////////////////////////////
    /// 参照するスクリプト.
    /////////////////////////////////////////
    // PlayerMoveスクリプト.
    private PlayerMove moveScript;
    // CameraShakeスクリプト.
    private CameraShake cameraShake;

    /////////////////////////////////////////
    /// サウンド.
    /////////////////////////////////////////
    private AudioSource audioSource;
    public  AudioClip   damage;         // 被ダメージ音.

    /////////////////////////////////////////
    /// 変数.
    /////////////////////////////////////////
    private GameObject       lifeUIPos;     // ライフのUIを置く場所のオブジェクト（親）.
    private GameObject       lifeUI;        // ライフのUIのPrefab.
    private List<GameObject> lifeList;      // ライフのUIのリスト.
    private int              damageCount;   // ダメージを受けた回数.
    private bool             isDamage;      // ダメージを受けているかのフラグ.
    private float            damageTime;    // ダメージを受けている時間.

    /////////////////////////////////////////
    /// 定数.
    /////////////////////////////////////////
    private const float DURATION        = 0.3f;      // カメラが揺れる時間.
    private const float MAGNITUDE       = 0.6f;      // カメラが揺れる範囲（球の半径）.
    private const float MAX_HP          = 3;         // 最大体力.
    private const float MOVESPEED       = 5;         // 移動速度.
    private const float APPLYSPEED      = 0.01f;     // 振り向きの適用速度.
    private const float LIFE_DISTANCE   = 200f;      // ライフUI同士の間隔.
    private const float DAMAGE_TIME_MAX = 0.5f;      // ダメージを受けた後の無敵時間.

    /////////////////////////////////////////
    // 初期化.
    /////////////////////////////////////////
    protected override void Start()
    {
        // 親クラスの初期化.
        base.Start();

        // 参照するスクリプト.
        moveScript  = this.GetComponent<PlayerMove>();                                              // PlayerMoveスクリプト.
        cameraShake = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<CameraShake>(); // CameraShakeスクリプト.

        // AudioSource
        audioSource = GetComponent<AudioSource>();

        // 変数.
        lifeUIPos   = GameObject.FindGameObjectWithTag("LifeUIPos");  // ライフのUIを置く場所のオブジェクト（親）.
        lifeUI      = (GameObject)Resources.Load("PlayerLife");       // ライフのUI.
        lifeList    = new List<GameObject>();                         // ライフのリスト.
        damageCount = 0;                                              // ダメージを受けた回数.
        isDamage    = false;                                          // ダメージを受けているかのフラグ.
        damageTime  = 0;                                              // ダメージを受けている時間.

        // 親クラスから継承した変数.
        anim       = this.GetComponent<Animator>();   // アニメーション.
        applySpeed = APPLYSPEED;                      // 振り向きの適用速度.
        HP         = MAX_HP;                          // 現在のHP.

        // ライフの数値分UIを出す.
        for(int i = 0; i < MAX_HP; i++)
        {
            // ずらす分のベクトルを計算.
            Vector3 pos = new Vector3(i * LIFE_DISTANCE, 0f, 0f);
            // リストに追加.
            lifeList.Add(Instantiate(lifeUI, lifeUIPos.transform.position + pos, Quaternion.identity));
            // 親を設定.
            lifeList[i].transform.SetParent(lifeUIPos.transform, false);
            // 被ダメージ時に表示する用の子のUiを非アクティブにする.
            lifeList[i].transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    /////////////////////////////////////////
    // 更新.
    /////////////////////////////////////////
    protected override void Update()
    {
        // 死亡しているならスルー.
        if (updateState == UpdateState.DEATH)
        {
            return;
        }

        // 親クラスのUpdate();
        base.Update();

        // UpdateStateがWAITならスルー.
        if (updateState == UpdateState.WAIT)
        {
            return;
        }

        //////////////////////
        /// ダメージ関係.
        if (isDamage)
        {
            // ダメージを受けている時間を加算.
            damageTime += Time.deltaTime;
            // 一定時間経過したらフラグを下す.
            if (damageTime > DAMAGE_TIME_MAX)
            {
                isDamage = false;
            }
        }
    }

    void FixedUpdate()
    {
        // 死亡しているならスルー.
        if (updateState == UpdateState.DEATH)
        {
            return;
        }

        // UpdateStateがWAITならスルー.
        if (updateState == UpdateState.WAIT)
        {
            return;
        }

        //////////////////////
        /// 移動関係.
        moveScript.Moving();
        moveScript.SpeedLimitter();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "SoulBullet")
        {
            Damage();
        }
    }

    /////////////////////////////////////////
    // 被ダメージ.
    /////////////////////////////////////////
    protected override void Damage()
    {
        if(updateState == UpdateState.DEATH)
        {
            return;
        }

        if(isDamage)
        {
            return;
        }

        base.Damage();

        // 効果音を鳴らす.
        audioSource.PlayOneShot(damage);

        // ライフUIの子要素をアクティブにする.
        lifeList[damageCount].transform.GetChild(0).gameObject.SetActive(true);
        // カメラを揺らすコルーチンを呼び出し.
        StartCoroutine(cameraShake.Shake(DURATION, MAGNITUDE));
        // ダメージを受けた回数を加算.
        damageCount++;
        // ダメージを受けたフラグを立てる.
        isDamage = true;
        // ダメージを受けている時間をリセット.
        damageTime = 0;

        // HPが0以下になったらシーン遷移.
        if (HP <= 0)
        {
            anim.SetBool("fall", false);
            // 死亡アニメーションのセット.
            anim.SetTrigger("death");
            // 5秒後にシーン遷移.
            Invoke("SceneChange", 5.0f);
        }
    }
}
