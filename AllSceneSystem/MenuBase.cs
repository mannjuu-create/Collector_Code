using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// メニューの切り替え、実行を行うスクリプト.
/// </summary>

public class MenuBase : MonoBehaviour
{
    protected int   menuNum;          // 現在のメニューの番号.
    private   float time;             // 経過時間.
    private   float changeSpeed;      // 拡大縮小を行う変数.
    private   bool  returnFlag;       // 拡大縮小が切り替わったフラグ.
    private   bool  changeFlag;       // 選択中のメニューが切り替わったフラグ.
    protected bool  decisionFlag;     // 決定したフラグ.
    protected bool  firstFlag;        // 決定して一回目に通ったフラグ.
    private   float afterTime;        // 決定後の経過時間.
    protected Image image;            // 決定したメニューの画像.

    /////////////////////////////////////////
    /// 初期化.
    /////////////////////////////////////////
    void Awake()
    {
        menuNum      = 0;           // 現在のメニューの番号.
        time         = 0;           // 経過時間.
        changeSpeed  = 0;           // 拡大縮小を行う変数.
        returnFlag   = true;        // 拡大縮小が切り替わったフラグ.
        changeFlag   = false;       // 選択中のメニューが切り替わったフラグ.
        decisionFlag = false;       // 決定したフラグ.
        firstFlag    = false;       // 決定して一回目に通ったフラグ.
        afterTime    = 0;           // 決定後の経過時間.
    }

    /////////////////////////////////////////
    /// 選択中のメニューを拡大縮小するスクリプト.
    /////////////////////////////////////////
    // メニュー、拡大縮小を切り替える時間.
    protected void ScalChange(GameObject[] menu, float changeTime)
    {
        changeSpeed = Time.deltaTime * 0.3f;

        if (time < 0)
        {
            returnFlag = true;
        }
        if (time > changeTime)
        {
            returnFlag = false;
        }

        if (returnFlag == true)
        {
            time += Time.deltaTime;
            menu[menuNum].transform.localScale += new Vector3(changeSpeed, changeSpeed, 1.0f);
        }
        else
        {
            time -= Time.deltaTime;
            menu[menuNum].transform.localScale -= new Vector3(changeSpeed, changeSpeed, 1.0f);
        }
    }

    /////////////////////////////////////////
    /// 配列ではないメニューを拡大縮小するスクリプト.
    /////////////////////////////////////////
    // メニュー、拡大縮小を切り替える時間.
    protected void ScalChange(GameObject menu, float changeTime)
    {
        changeSpeed = Time.deltaTime * 0.3f;

        if (time < 0)
        {
            returnFlag = true;
        }
        if (time > changeTime)
        {
            returnFlag = false;
        }

        if (returnFlag == true)
        {
            time += Time.deltaTime;
            menu.transform.localScale += new Vector3(changeSpeed, changeSpeed, 1.0f);
        }
        else
        {
            time -= Time.deltaTime;
            menu.transform.localScale -= new Vector3(changeSpeed, changeSpeed, 1.0f);
        }
    }

    /////////////////////////////////////////
    /// 決定する関数.
    /////////////////////////////////////////
    // AudioSource、効果音、音量、実行する関数名.
    protected void Decision(AudioSource audio, AudioClip sound, float volume, string functionName, float functionTime)
    {
        /// リセットさせる.
        time      = 0;  // 経過時間.
        afterTime = 0;  // 決定後の経過時間.

        // 効果音を鳴らす.
        audio.PlayOneShot(sound, volume);
        // フラグを立てる.
        decisionFlag = true;
        // functionTime秒後に実行.
        Invoke(functionName, functionTime);
    }

    /////////////////////////////////////////
    /// 決定後、徐々に透明にする関数.
    /////////////////////////////////////////
    // メニュー、決定直後の大きさ、消えるまでの時間、決定後の拡大速度.
    protected void Stealth(GameObject[] menu, Vector3 decisionSize, float stealthTime, float enlargementSpeed)
    {
        // 一回目のみ.
        if (firstFlag == false)
        {
            // 大きさを変更.
            menu[menuNum].transform.localScale = decisionSize;
            // Imageを取得.
            image = menu[menuNum].GetComponent<Image>();
            firstFlag = true;
        }

        // 経過時間を加算.
        afterTime += Time.deltaTime;
        if (afterTime < stealthTime)
        {
            // 透明にしていく.
            float alpha = 1.0f - afterTime / stealthTime;
            Color color = image.color;
            color.a = alpha;
            image.color = color;

            // 拡大していく.
            Vector3 tmp = menu[menuNum].transform.localScale;
            tmp += new Vector3(afterTime / enlargementSpeed, afterTime / enlargementSpeed, 0);
            menu[menuNum].transform.localScale = tmp;
        }
    }

    /////////////////////////////////////////
    /// 配列ではないメニュー決定後、徐々に透明にする関数.
    /////////////////////////////////////////
    // メニュー、消えるまでの時間、決定後の拡大速度.
    protected void Stealth(GameObject menu, Vector3 decisionSize, float stealthTime, float enlargementSpeed)
    {
        // 一回目のみ.
        if (firstFlag == false)
        {
            // 大きさを変更.
            menu.transform.localScale = decisionSize;
            // Imageを取得.
            image = menu.GetComponent<Image>();
            firstFlag = true;
        }

        // 経過時間を加算.
        afterTime += Time.deltaTime;
        if (afterTime < stealthTime)
        {
            // 透明にしていく.
            float alpha = 1.0f - afterTime / stealthTime;
            Color color = image.color;
            color.a = alpha;
            image.color = color;

            // 拡大していく.
            Vector3 tmp = menu.transform.localScale;
            tmp += new Vector3(afterTime / enlargementSpeed, afterTime / enlargementSpeed, 0);
            menu.transform.localScale = tmp;
        }
    }

    /////////////////////////////////////////
    /// メニュー選択.
    /////////////////////////////////////////
    // メニュー、メニューの最大値、選択肢変更キー（上）、選択肢変更キー（下）、選択中以外の選択肢の大きさ、AudioSource、効果音、音量.
    protected void Select(GameObject[] menu, int menuNumMax, KeyCode upKey, KeyCode downKey, Vector3 otherOptionSize, AudioSource audio, AudioClip sound, float volume)
    {
        // キー入力でメニュー選択.
        if (Input.GetKeyDown(upKey))
        {
            // 選択中のメニューが最初以外なら.
            if (menuNum != 0)
            {
                // 選択中のメニューを1つ前にする.
                menuNum--;
                // フラグを立てる.
                changeFlag = true;
            }
        }
        else if (Input.GetKeyDown(downKey))
        {
            // 選択中のメニューが最後以外なら.
            if (menuNum != (menuNumMax - 1))
            {
                // 選択中のメニューを1つ後にする.
                menuNum++;
                // フラグを立てる.
                changeFlag = true;
            }
        }

        // メニューが切り替わったら
        if (changeFlag == true)
        {
            // 選択中以外のメニューの大きさをリセット.
            for (int i = 0; i < menuNumMax; i++)
            {
                if (i == menuNum)
                {
                    continue;
                }

                menu[i].transform.localScale = otherOptionSize;
            }
            // 経過時間のリセット.
            time = 0;
            // 効果音を鳴らす.
            audio.PlayOneShot(sound, volume);

            // 切り替わったフラグを下ろす.
            changeFlag = false;
        }

    }
}
