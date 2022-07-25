using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ranking : MonoBehaviour
{
    private TextMeshProUGUI nowScoreText;        // 今回のスコアのテキスト.
    private int             nowScore;            // 今回のスコア.
    private TextMeshProUGUI beforeRankingText;   // ランキングの前半のテキスト.
    private TextMeshProUGUI afterRankingText;    // ランキングの後半のテキスト.
    private int[]           scoreRanking;        // スコアのランキング.

    private const int RANKING_NUM = 10;          // ランキングの最大数.

    /////////////////////////////////
    /// 初期化.
    /////////////////////////////////
    void Start()
    {
        // 今回のスコアのテキストを取得.
        nowScoreText = GameObject.FindGameObjectWithTag("NowScore").GetComponent<TextMeshProUGUI>();
        // 今回のスコアのロード.
        nowScore = PlayerPrefs.GetInt("SCORE", 0);
        // 今回のスコアのテキストの表示を入れ替える.
        nowScoreText.text = ToSpriteAssetFont.FontConversion(nowScore.ToString());

        // ランキングの前半のテキストを取得.
        beforeRankingText = GameObject.FindGameObjectWithTag("BeforeRanking").GetComponent<TextMeshProUGUI>();
        // ランキングの後半のテキストを取得.
        afterRankingText = GameObject.FindGameObjectWithTag("AfterRanking").GetComponent<TextMeshProUGUI>();
        // ランキングを取得.
        GetRanking();
        // ランキングの更新.
        UpdateRanking(nowScore);
        // ランキングの表示.
        DrawRanking();
    }

    /////////////////////////////////
    /// 削除時の処理.
    /////////////////////////////////
    void OnDestroy()
    {
        // ランキングを保存.
        SaveRanking();
        PlayerPrefs.Save();
    }

    /////////////////////////////////
    /// ランキングの取得.
    /////////////////////////////////
    void GetRanking()
    {
        // スコアのランキングを初期化.
        scoreRanking = new int[RANKING_NUM];
        for(int i = 0; i< RANKING_NUM; i++)
        {
            // ランキングに取得した値を格納する.
            scoreRanking[i] = PlayerPrefs.GetInt("Ranking" + (i + 1), 0);
        }
    }

    /////////////////////////////////
    /// ランキングの更新.
    /////////////////////////////////
    void UpdateRanking(int newScore)
    {
        // 一時保存するための変数.
        int tmp = 0;
        // ランキングの最大数繰り返す.
        for (int i = 0; i < RANKING_NUM; i++)
        {
            // 今回のスコアがランキングのスコアよりも高かったら入れ替える.
            if (scoreRanking[i] < newScore)
            {
                tmp = scoreRanking[i];
                scoreRanking[i] = newScore;
                newScore = tmp;
            }
        }
    }

    /////////////////////////////////
    /// ランキングの表示.
    /////////////////////////////////
    void DrawRanking()
    {
        // 最初にテキストの中身を初期化.
        beforeRankingText.text = "";
        afterRankingText.text = "";
        // 前半と後半分けるため、ランキングの半数繰り返す.
        for (int i = 0; i < (RANKING_NUM / 2); i++)
        {
            beforeRankingText.text += ToSpriteAssetFont.FontConversion((i + 1) + ":" + scoreRanking[i].ToString());
            beforeRankingText.text += "\n";
        }
        // 後半も同様に繰り返す.
        for (int i = RANKING_NUM / 2; i < RANKING_NUM; i++)
        {
            afterRankingText.text += ToSpriteAssetFont.FontConversion((i + 1) + ":" + scoreRanking[i].ToString());
            afterRankingText.text += "\n";
        }
    }

    /////////////////////////////////
    /// ランキングの保存.
    /////////////////////////////////
    void SaveRanking()
    {
        if(scoreRanking == null)
        {
            return;
        }
        for (int i = 0; i < RANKING_NUM; i++)
        {
            PlayerPrefs.SetInt("Ranking" + (i + 1), scoreRanking[i]);
        }
    }

    /////////////////////////////////
    /// ランキングの削除.
    /////////////////////////////////
    void DeleteRanking()
    {
        for(int i = 0; i < RANKING_NUM; i++)
        {
            PlayerPrefs.DeleteKey("Ranking" + (i + 1));
        }
    }
}
