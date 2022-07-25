using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全キャラクターの基盤となるスクリプト.
/// </summary>

public class CharactorBase : ObjectBase
{
    /////////////////////////////////////////
    /// コンポーネント.
    /////////////////////////////////////////
    protected Animator anim; // アニメーション.

    /////////////////////////////////////////
    /// 使用する変数.
    /////////////////////////////////////////
    protected float applySpeed; // 振り向きの適用速度.
    protected float HP;         // 現在のHP.

    /////////////////////////////////////////
    /// 初期化.
    /////////////////////////////////////////
    protected override void Start()
    {
        base.Start();
    }

    /////////////////////////////////////////
    ///更新.
    /////////////////////////////////////////
    protected override void Update()
    {
        // 死亡しているならスルー.
        if (updateState == UpdateState.DEATH)
        {
            return;
        }

        base.Update();
    }

    /////////////////////////////////////////
    /// 被ダメージ.
    /////////////////////////////////////////
    protected virtual void Damage()
    {
        // hpを減らす.
        HP--;

        // 体力が0を下回ったら
        if (HP <= 0)
        {
            // updateStateをDEATHへ変更.
            updateState = UpdateState.DEATH;
        }
    }
}
