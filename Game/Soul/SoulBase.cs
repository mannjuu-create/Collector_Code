using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulBase : ObjectBase
{
	// 弾のPrefab.
	private GameObject bulletPrefab;

	// 変数.
	private Vector3 changeTmpPos;       // 現在の位置を初期化.
	private Vector3 velocity;           // 移動速度を初期化.
	private float   interval;			// 弾を撃ってから待機している時間.

	// 定数.
	private const float SPEED_X      = 0.05f;		// 横移動の速度.
	private const float LEFT_END     = -30f;		// 移動の左端.
	private const float RIGHT_END    = 30f;			// 移動の右端.
	private const float PROPORTION   = 100f;		// 縦移動がそのままでは大きすぎるので割る.
	private const float INTERVAL	 = 2.5f;		// 次の弾を打つまでの時間.

	/////////////////////////////////
	/// 初期化.
	/////////////////////////////////
	protected override void Start()
    {
		base.Start();

		// 弾のPrefabを取得.
		bulletPrefab = (GameObject)Resources.Load("Bullet");

		// 現在の位置を初期化.
		changeTmpPos = Vector3.zero;
		// 移動速度を初期化.
		velocity = new Vector3(SPEED_X, 0, 0);
		// 待機している時間を初期化.
		interval = 0;
	}

	/////////////////////////////////
	/// 更新.
	/////////////////////////////////
	protected override void Update()
    {
		base.Update();
		if(updateState == UpdateState.WAIT)
        {
			return;
        }

		Move();

		Shot();
	}

	private void Move()
    {
		// 現在の位置を保存.
		changeTmpPos = transform.position;
		// 画面端から出そうになっていたら位置を画面内に戻してX軸の移動方向を反転.
		if (transform.position.x < LEFT_END)
		{
			velocity.x = -velocity.x;
			changeTmpPos.x = LEFT_END;
		}
		else if (transform.position.x > RIGHT_END)
		{
			velocity.x = -velocity.x;
			changeTmpPos.x = RIGHT_END;
		}
		// 現在の位置を実際の位置に代入.
		transform.position = changeTmpPos;
		// 縦移動を計算.
		velocity.y = Mathf.Sin(Time.time) / PROPORTION;

		// 実際の位置に移動量を加算.
		transform.position += velocity;
	}

	private void Shot()
	{
		// 時間を経過.
		interval += Time.deltaTime;

		// 待機時間が一定に達していないならスルー.
		if(interval < INTERVAL)
        {
			return;
        }

		// Prefabから弾を複製する.
		GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

		// 経過時間をリセット.
		interval = 0;
	}
}
