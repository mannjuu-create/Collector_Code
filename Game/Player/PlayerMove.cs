using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの移動をするスクリプト.
/// </summary>

public class PlayerMove : MonoBehaviour
{
    /////////////////////////////////////////
    /// サウンド.
    /////////////////////////////////////////
    private AudioSource audioSource;
    public  AudioClip bound;         // キノコで跳ねる.
    public  AudioClip landing;       // 着地.

    /////////////////////////////////////////
    /// コンポーネント.
    /////////////////////////////////////////
    [HideInInspector] public Rigidbody rb;      // Rigitbody.
    private Animator anim;                      // アニメーション.

    enum JumpState
    {
        GROUND = 1,
        UP     = 2,
        DOWN   = 3,
        BOUND  = 4
    }

    /////////////////////////////////////////
    /// 使用する変数.
    /////////////////////////////////////////
    [HideInInspector] public Vector2 velocity;     // 移動方向.
    private Vector2 rotateVec;               // 回転用の移動方向.
    private float   wide;                    // 移動入力取得.
    private float   moveSpeed;               // 移動速度.
    private bool    preIsGround;             // 直前の接地判定
    private bool    isGround;                // 接地判定.
    private float   applySpeed;              // 振り向きの適用速度.

    private JumpState jumpState;             // ジャンプの状態.
    private Vector2   jumpVec;               // ジャンプのベクトル.
    private float     jumpTime;              // ジャンプの経過時間.
    private bool      jumpKey;               // ジャンプキー.
    private bool      keyLook;               // キー入力を受け付けない.

    private Vector3 spherePos;               // 接地判定用Sphereの位置.
    private float   sphereRadius;            // 接地判定用Sphereの半径.
    private Vector3 rightBoxPos;             // 右側の当たり判定用のBoxの位置.
    private Vector3 leftBoxPos;              // 左側の当たり判定用のBoxの位置.
    private Vector3 boxSize;                 // 当たり判定に使うBoxの大きさ.

    /////////////////////////////////////////
    /// 定数.
    /////////////////////////////////////////
    private const float MOVESPEED        = 1000f;          // 移動速度.
    private const float SPEED_LIMIT_X    = 8f;             // X軸の移動速度上限.
    private const float SPEED_LIMIT_Y    = 30f;            // Y軸の移動速度上限.
    private const float DECELERATION     = 0.5f;           // X軸の減速率.
    private const float GRAVITY          = 20f;            // 重力.
    private const float JUMP_LOWER_LIMIT = 0.03f;          // ジャンプ時間の下限.
    private const float JUMP_POWER       = 1.25f;          // ジャンプ力.
    private const float BOUND_POWER      = 20f;            // キノコで跳ねた時の力.
    private const float APPLYSPEED       = 3f;             // 振り向きの適用速度.

    /////////////////////////////////////////
    /// 初期化.
    /////////////////////////////////////////
    void Start()
    {
        // AudioSource
        audioSource = GetComponent<AudioSource>();

        rb   = this.GetComponent<Rigidbody>();  // Rigitbody.
        anim = this.GetComponent<Animator>();   // アニメーション.
        anim.SetBool("start", true);

        velocity    = Vector2.zero;      // 移動方向.
        rotateVec   = Vector2.zero;      // 回転用の移動方向.
        wide        = 0;                 // 移動入力取得（左右）.
        preIsGround = false;             // 直前の接地判定.
        isGround    = false;             // 接地判定.
        moveSpeed   = MOVESPEED;         // 移動速度.
        applySpeed  = APPLYSPEED;        // 振り向きの適用速度.

        jumpState = JumpState.GROUND;   // ジャンプの状態.
        jumpVec   = Vector2.zero;       // ジャンプのベクトル.
        jumpTime  = 0;                  // ジャンプの経過時間.
        jumpKey   = false;              // ジャンプキー.
        keyLook   = false;              // キー入力を受け付けない.

        spherePos    = Vector3.zero;                                            // 接地判定用Sphereの位置.
        sphereRadius = transform.lossyScale.x * 0.4f;                           // 接地判定用Sphereの半径.
        rightBoxPos  = Vector3.zero;                                            // 右側の当たり判定用のBoxの位置.
        leftBoxPos   = Vector3.zero;                                            // 左側の当たり判定用のBoxの位置.
        boxSize      = new Vector3(0.1f, transform.lossyScale.y * 0.5f, 1);     // 当たり判定に使うBoxの大きさ.
    }

    /////////////////////////////////////////
    /// 移動処理.
    /////////////////////////////////////////
    public void Moving()
    {
        // 走っているフラグを一旦下ろす.
        anim.SetBool("run", false);

        // 入力.
        {
            // 左右移動.
            wide = Input.GetAxis("Horizontal");         //左右矢印キーの値(-1.0~1.0)
            velocity = new Vector2(wide, 0);

            // 壁にぶつかっていたら速度を消す
            if (wide > 0)
            {
                if (IsRightForBlockJudge())
                {
                    velocity.x = 0;
                }
            }
            else if (wide < 0)
            {
                if (IsLeftForBlockJudge())
                {
                    velocity.x = 0;
                }
            }

            // 接地判定.
            preIsGround = isGround;
            isGround = IsGroundJudge();
            if (isGround == false)
            {
                // アニメーションのfallフラグセット.
                anim.SetBool("fall", true);
            }

            Stay();
            // ジャンプのキー入力取得
            if (Input.GetKey(KeyCode.Space))
            {
                if (!keyLook)
                {
                    jumpKey = true;
                }

                else
                {
                    jumpKey = false;
                }
            }

            else
            {
                jumpKey = false;
                keyLook = false;
            }
            Jump();
        }

        //////////////////////////////////////////
        // 横に移動していて、地上にいるなら回転する.
        if (velocity.x != 0 &&
            isGround == true)
        {
            // 走っているフラグを立てる.
            anim.SetBool("run", true);

            // 回転用の移動方向を更新.
            rotateVec = new Vector2(velocity.x, 0);
            // プレイヤーの回転(transform.rotation)の更新
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                                  Quaternion.LookRotation(rotateVec),
                                                  applySpeed);
        }

        // 横移動のみ.
        // 速度ベクトルの長さを1秒でmoveSpeedだけ進むように調整します
        velocity = velocity * moveSpeed * Time.deltaTime;
        Vector3 tmp = new Vector3(velocity.x, 0, 0);
        rb.AddForce(tmp);
        // 入力の値が一定値以下なら減速をかける.
        if (Mathf.Abs(wide) < 0.7f)
        {
            Vector3 result = rb.velocity;
            result.x *= DECELERATION;
            rb.velocity = result;
        }
    }

    public void SpeedLimitter()
    {
        // X軸.
        if (Mathf.Abs(rb.velocity.x) > SPEED_LIMIT_X)
        {
            rb.velocity = new Vector3(rb.velocity.x / 1.1f, rb.velocity.y, 0);
        }

        // Y軸.
        if (Mathf.Abs(rb.velocity.y) > SPEED_LIMIT_Y)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y / 1.1f, 0);
        }
    }

    /////////////////////////////////////////
    /// 着地.
    /////////////////////////////////////////
    void Stay()
    {
        if (preIsGround == false &&
            isGround == true)
        {
            if (rb.velocity.y < 0)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, 0);
            }
            // アニメーションのfallフラグセット.
            anim.SetBool("fall", false);
            // ステートを変更.
            jumpState = JumpState.GROUND;
            // ジャンプ経過時間をリセット.
            jumpTime = 0f;
            // 効果音を鳴らす.
            audioSource.PlayOneShot(landing, 0.5f);
            // キー操作をロックする.
            keyLook = true;
        }
    }

    /////////////////////////////////////////
    /// ジャンプ処理.
    /////////////////////////////////////////
    public void Jump()
    {
        jumpVec = Vector2.zero;

        switch (jumpState)
        {
            // 接地時
            case JumpState.GROUND:
                if (jumpKey)
                {
                    anim.SetTrigger("jump");
                    jumpState = JumpState.UP;
                }
                if (!isGround)
                {
                    jumpState = JumpState.DOWN;
                }
                break;

            // 上昇時
            case JumpState.UP:
                jumpTime += Time.deltaTime;

                if (jumpKey || JUMP_LOWER_LIMIT > jumpTime)
                {
                    jumpVec.y = JUMP_POWER;
                    jumpVec.y -= (GRAVITY * Mathf.Pow(jumpTime, 2));
                }

                else
                {
                    jumpTime += Time.deltaTime; // 落下を早める
                    jumpVec.y = JUMP_POWER;
                    jumpVec.y -= (GRAVITY * Mathf.Pow(jumpTime, 2));
                }

                rb.AddForce(jumpVec, ForceMode.Impulse);

                if (0f > jumpVec.y)
                {
                    jumpState = JumpState.DOWN;
                    jumpVec.y = 0f;
                    jumpTime = 0.1f;
                }
                break;

            // 落下時
            case JumpState.DOWN:
                rb.AddForce(new Vector3(0, -GRAVITY, 0));
                break;

            // キノコではねた時
            case JumpState.BOUND:
                // 効果音を鳴らす.
                audioSource.PlayOneShot(bound, 0.1f);

                rb.AddForce(new Vector3(0, BOUND_POWER, 0), ForceMode.Impulse);
                jumpState = JumpState.DOWN;
                break;

            default:
                break;
        }
    }

    /////////////////////////////////////////
    /// 当たり判定処理.
    /////////////////////////////////////////
    // 接地判定.
    bool IsGroundJudge()
    {
        // Sphereを出す位置（中心）.
        spherePos = transform.position + Vector3.up * 0.3f;

        // 接触している当たり判定を格納.
        Collider[] hitCollider = Physics.OverlapSphere(spherePos, sphereRadius, LayerMask.GetMask("Stage"));

        // 接触しているものがないなら結果を返す.
        if(hitCollider.Length == 0)
        {
            return false;
        }

        // 接触しているものの中にキノコがあったらjumpStateをBOUNDにする.
        for (int i = 0; i < hitCollider.Length; i++)
        {
            if (hitCollider[i].gameObject.tag == "Mushroom")
            {
                Vector3 tmp = new Vector3(rb.velocity.x, 0, 0);
                rb.velocity = tmp;
                jumpState = JumpState.BOUND;

                // 地面ではないのでfalseを返す.
                return false;
            }
        }

        // 結果を返す.
        return true;
    }

    // 右側が壁にぶつかっているか判定.
    bool IsRightForBlockJudge()
    {
        // Boxを出す位置（中心）.
        rightBoxPos = new Vector3(transform.position.x + 0.5f, transform.position.y + 1, transform.position.z);

        // 内側に接しているものがあるか調べる.
        return Physics.CheckBox(rightBoxPos, boxSize, Quaternion.identity, LayerMask.GetMask("Stage"));
    }

    // 左側が壁にぶつかっているか判定.
    bool IsLeftForBlockJudge()
    {
        // Boxを出す位置（中心）.
        leftBoxPos = new Vector3(transform.position.x - 0.5f, transform.position.y + 1, transform.position.z);

        // 内側に接しているものがあるか調べる.
        return Physics.CheckBox(leftBoxPos, boxSize, Quaternion.identity, LayerMask.GetMask("Stage"));
    }

    // 当たり判定の描画.
    //void OnDrawGizmos()
    //{
    //    // Sphereを出す位置（中心）.
    //    var spherePos = transform.position + Vector3.up * 0.3f;
    //    Gizmos.DrawSphere(spherePos, sphereRadius);

    //    // 右側のBoxを出す位置（中心）.
    //    var RBoxPos = new Vector3(transform.position.x + 0.5f, transform.position.y + 1, transform.position.z);
    //    Gizmos.DrawCube(RBoxPos, boxSize);

    //    // 左側のBoxを出す位置（中心）.
    //    var LBoxPos = new Vector3(transform.position.x - 0.5f, transform.position.y + 1, transform.position.z);
    //    Gizmos.DrawCube(LBoxPos, boxSize);
    //}

    /////////////////////////////////////////
    /// アニメーションで呼ぶ関数.
    /////////////////////////////////////////
    // 登場アニメーションのフラグを下ろす.
    public void FinishEntranceAnim()
    {
        anim.SetBool("start", false);
    }
}
