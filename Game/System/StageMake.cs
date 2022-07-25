using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageMake : MonoBehaviour
{
    // ステージのPrefabのリスト.
    [SerializeField] private List<GameObject> stageList = new List<GameObject>();
    // ステージの番号.
    private int num;

    void Awake()
    {
        // ステージの番号を取得.
        num = PlayerPrefs.GetInt("STAGE", 0);
        

        Instantiate(stageList[num]);
    }
}
