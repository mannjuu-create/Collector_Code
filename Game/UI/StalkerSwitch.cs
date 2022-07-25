using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 宝石の位置を表示するUIをアクティブにするスクリプト.
/// </summary>

public class StalkerSwitch : MonoBehaviour
{
    // PauseScriptスクリプト.
    private PauseScript pauseScript;
    // 宝石を表示するUIのリスト.
    [HideInInspector] public List<GameObject> stalkerList;

    void Start()
    {
        // CameraScaleChangeスクリプト.
        pauseScript = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<PauseScript>();

        // 宝石を表示するUIのリスト.
        stalkerList = new List<GameObject>();
    }

    void Update()
    {
        // 切り替えたフラグが下りているなら
        if(!pauseScript.isSwitch)
        {
            // 子オブジェクトの数だけ繰り返す..
            for (int i = 0; i < this.transform.childCount; i++)
            {
                if(this.transform.GetChild(i).gameObject.tag == "Stalker")
                {
                    // 子オブジェクトを取得し、リストに追加.
                    stalkerList.Add(this.transform.GetChild(i).gameObject);
                }
            }

            foreach (GameObject stalker in stalkerList)
            {
                stalker.SetActive(true);
            }
            // 最後に要素を全消去する.
            stalkerList.Clear();
            // 切り替えたフラグを立てる.
            pauseScript.isSwitch = true;
        }
    }
}
