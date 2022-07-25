using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulBullet : ObjectBase
{
    // プレイヤー.
    private GameObject player;
    // 生存している時間.
    private float arriveTime;

    // 弾速.
    private const float SPEED = 500f;
    // 消滅するまでの時間.
    private const float DESTROY_TIME = 10f;

    /////////////////////////////////
    /// 初期化.
    /////////////////////////////////
    protected override void Start()
    {
        base.Start();

        // プレイヤー.
        player = GameObject.FindGameObjectWithTag("Player");
        // 生存している時間.
        arriveTime = 0;

        // Rigidbodyを取得.
        Rigidbody rb = this.GetComponent<Rigidbody>();

        // 目的地を設定.
        Vector3 destination = player.transform.position - this.transform.position;
        destination.Normalize();

        // 弾速は現在のプレイヤーの位置によって設定
        rb.AddForce(destination * SPEED);
    }

    /////////////////////////////////
    /// 更新.
    /////////////////////////////////
    protected override void Update()
    {
        base.Update();

        if (updateState == UpdateState.WAIT)
        {
            return;
        }

        // 生存時間を加算.
        arriveTime += Time.deltaTime;
        // 生存時間が一定時間を超えたら消滅.
        if (arriveTime > DESTROY_TIME)
        {
            Destroy(this.gameObject);
        }
    }
}
