using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// プレイヤーと宝石が当たった時のスクリプト.
/// </summary>

public class GetJewel : MonoBehaviour
{
    /////////////////////////////////////////
    /// 参照するスクリプト.
    /////////////////////////////////////////
    // JewelMakerスクリプト.
    private JewelMaker makerScript;

    /////////////////////////////////////////
    /// サウンド.
    /////////////////////////////////////////
    private AudioSource audioSource;
    public  AudioClip   get;         // 宝石を取得.

    /////////////////////////////////////////
    /// 変数.
    /////////////////////////////////////////
    // 取得した宝石の合計.
    [HideInInspector] public int totalGetJewel;
    // 取得した宝石の合計を表示するテキスト.
    private TextMeshProUGUI scoreText;

    void Start()
    {
        // スクリプトの取得.
        makerScript = GameObject.FindGameObjectWithTag("JewelMaker").GetComponent<JewelMaker>();

        // AudioSource
        audioSource = GetComponent<AudioSource>();

        // 取得した宝石の合計.
        totalGetJewel = 0;
        // 表示テキスト.
        scoreText = GameObject.FindWithTag("Score").GetComponent<TextMeshProUGUI>();
        // テキストを初期化.
        scoreText.text = ToSpriteAssetFont.FontConversion(totalGetJewel.ToString());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Jewel")
        {
            // JewelMakerの宝石の現在値を減少.
            makerScript.jewelNum--;
            // 取得した宝石の合計を増加.
            totalGetJewel++;
            // 効果音を鳴らす.
            audioSource.PlayOneShot(get, 0.1f);
            // テキストの更新.
            scoreText.text = ToSpriteAssetFont.FontConversion(totalGetJewel.ToString());
            // 取得した宝石を消去.
            Destroy(other.gameObject);
        }
    }
}
