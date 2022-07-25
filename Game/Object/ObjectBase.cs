using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectBase : MonoBehaviour
{
	/////////////////////////////////////////
	/// 参照するスクリプト.
	/////////////////////////////////////////
	protected CountDown   countScript; // CountDownスクリプト.
	protected PauseScript pauseScript; // PauseScriptスクリプト.
	protected TimeLimit   timeScript;  // TimeLimitスクリプト.
	private   GetJewel    getScript;   // Getjewelスクリプト.

	protected bool firstFlag;            // ゲーム開始時にupdateStateを一度だけEXECUTIONにする為のフラグ.
	private   bool changeFlag;           // 切り替えたかどうかのフラグ.
	private   bool prePauseFlag;		 // 直前のPauseFlagの状態.

	protected enum UpdateState
	{
		WAIT,       // 待機.
		EXECUTION,  // 実行.
		DEATH,		// 死亡.
	}
	protected UpdateState updateState;

	/////////////////////////////////////////
	/// 初期化.
	/////////////////////////////////////////
	protected virtual void Start()
	{
		// CountDownスクリプト.
		countScript = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<CountDown>();
		// PauseScriptスクリプト.
		pauseScript = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<PauseScript>();
		// TimeLimitスクリプト.
		timeScript  = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<TimeLimit>();
		// GetJewelスクリプト.
		getScript   = GameObject.FindGameObjectWithTag("Player").GetComponent<GetJewel>();

		// 一度目かどうかのフラグ.
		firstFlag    = true;
		// 切り替えたかどうかのフラグ.
		changeFlag   = false;
		// 直前のPauseFlagの状態.
		prePauseFlag = pauseScript.pauseFrag;

		// 待機状態にする.
		updateState = UpdateState.WAIT;
	}

	/////////////////////////////////////////
	/// 更新.
	/////////////////////////////////////////
	protected virtual void Update()
	{
		// カウントダウン中ならスルー.
		if (!countScript.startFrag)
		{
			return;
		}

		// タイムオーバーならupdateStateを変更してスルー.
		if (timeScript.timeOverFrag)
		{
			updateState = UpdateState.WAIT;
			// このオブジェクトがプレイヤーならRigidbodyを保存する.
			if (this.gameObject.tag == "Player")
			{
				gameObject.GetComponent<Rigidbody>().Pause(gameObject);
			}
			return;
		}

		// ゲームが開始されて一度目なら実行.
		if (firstFlag)
        {
			updateState = UpdateState.EXECUTION;
			firstFlag = false;
		}

		// PauseFlagの状態が直前と違っていたら.
		if(prePauseFlag != pauseScript.pauseFrag)
        {
			// フラグの状態を更新.
			prePauseFlag = pauseScript.pauseFrag;
			// 切り替えたフラグを下ろす.
			changeFlag   = false;
		}

		// 切り替えたフラグが立っていたらスルー.
		if (changeFlag)
		{
			return;
		}

		// 一時停止フラグが立っていたらupdateStateを待機にする.
		if (pauseScript.pauseFrag)
        {
			updateState = UpdateState.WAIT;
			// このオブジェクトがプレイヤーか弾ならRigidbodyを保存する.
			if (this.gameObject.tag == "Player" || this.gameObject.tag == "SoulBullet")
            {
				gameObject.GetComponent<Rigidbody>().Pause(gameObject);
			}
		}
		else
        {
			updateState = UpdateState.EXECUTION;
			// このオブジェクトがプレイヤーか弾ならRigidbodyを読み込む.
			if (this.gameObject.tag == "Player" || this.gameObject.tag == "SoulBullet")
			{
				gameObject.GetComponent<Rigidbody>().Resume(gameObject);
			}
		}

		changeFlag = true;
		Debug.Log(this.gameObject);
		Debug.Log(pauseScript.pauseFrag);
	}

	/////////////////////////////////////////
	/// シーン遷移.
	/////////////////////////////////////////
	protected void SceneChange()
	{
		//イベントに登録.
		SceneManager.sceneLoaded += GameOverSceneLoaded;

		//シーン切り替え.
		SceneManager.LoadScene("ResultScene");
	}

	private void GameOverSceneLoaded(Scene next, LoadSceneMode mode)
	{
		// 今回のスコアを保存.
		PlayerPrefs.SetInt("SCORE", getScript.totalGetJewel);

		// イベントから削除
		SceneManager.sceneLoaded -= GameOverSceneLoaded;
	}
}
