using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ステージ上に宝石を生成するスクリプト.
/// </summary>

public class JewelMaker : ObjectBase
{
    [HideInInspector] public int jewelNum;     // 現在の宝石の数.
    private GameObject       jewel;            // 宝石のPrefab.
    private GameObject       jewelStalker;     // 宝石の位置を示すUI.
    private GameObject       stalkerList;      // UIの親オブジェクト.
    private float            waitTime;         // 待機している時間.
    private List<int>        numbersList;      // 宝石をセットする時に位置の番号として使用するリスト.
    private List<Vector3>    jewelPosList;     // 宝石を出現させる位置のリスト.

    private const float WAITTIME      = 0.5f;   // 次の宝石を出すまでの待機時間.
    private const int   MAX_JEWEL_NUM = 4;      // 宝石の最大個数.

    protected override void Start()
    {
        base.Start();

        jewel        = (GameObject)Resources.Load("Jewel");              // 宝石のPrefab.
        jewelStalker = (GameObject)Resources.Load("JewelStalker");       // 宝石の位置を示すUI.
        stalkerList  = GameObject.FindGameObjectWithTag("StalkerList");  // stalkerList.
        jewelNum     = 0;                                                // 現在の宝石の数.
        waitTime     = 0;                                                // 待機している時間.

        numbersList  = new List<int>();                     // 宝石をセットする時に位置の番号として使用するリスト.
        jewelPosList = new List<Vector3>();                 // 宝石を出現させる位置のリスト.
        GetChildren();                                      // 位置のリストに子のオブジェクトから位置を取得.

        // 最初に一回宝石を配置.
        SetJewel();
    }

    protected override void Update()
    {
        // 親クラスのUpdate();
        base.Update();

        // UpdateStateがWAITならスルー.
        if (updateState == UpdateState.WAIT)
        {
            return;
        }

        // 出現中の宝石が１以上ならスルー.
        if (jewelNum > 0)
        {
            return;
        }

        // TimeLimitスクリプトの待つフラグを立てる.
        timeScript.waitFrag = true;
        // 待機時間を経過させる.
        waitTime += Time.deltaTime;

        // 待機時間が一定を過ぎたら宝石を出す.
        if (waitTime >= WAITTIME)
        {
            // 宝石を設置.
            SetJewel();
            // 待機時間をリセット.
            waitTime = 0;
        }
    }

    // 子オブジェクトから位置の取得.
    void GetChildren()
    {
        // 子オブジェクトの数だけ繰り返す.
        for (int i = 0; i < this.transform.childCount; i++)
        {
            // 子オブジェクトを取得し、リストに追加.
            jewelPosList.Add(this.transform.GetChild(i).transform.position);
        }
    }

    // 宝石をランダムにステージに配置.
    void SetJewel()
    {
        // 位置の追加を子オブジェクトの数だけ繰り返す.
        for (int i = 0; i < this.transform.childCount; i++)
        {
            // 番号用リストに番号を追加.
            numbersList.Add(i);
        }

        // ステージに出す最大個数まで繰り返す.
        for (int i = 0; i < MAX_JEWEL_NUM; i++)
        {
            // 要素（番号）をランダムに取得.
            int num   = Random.Range(0, numbersList.Count);
            int index = numbersList[num];

            // 宝石を番号の場所へ設置する.
            GameObject cloneJewel = Instantiate(jewel, jewelPosList[index], Quaternion.identity);
            // 宝石の位置を示すUIを作成.
            GameObject cloneStalker = Instantiate(jewelStalker);
            // 位置のUIに宝石を渡す.
            TargetIndicator script = cloneStalker.GetComponent<TargetIndicator>();
            script.target = cloneJewel.transform;
            // UIの親にstalkerListを設定.
            cloneStalker.transform.SetParent(stalkerList.transform, false);

            // 現在の宝石の数をプラスする.
            jewelNum++;

            // 要素を消去.
            numbersList.RemoveAt(num);
        }

        // 制限時間リセットの関数を呼び出す.
        timeScript.TimeLimitReset();
        // 最後に残った要素も全消去する.
        numbersList.Clear();
    }
}
