using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム終了時の処理.
/// </summary>

public class GameFinish : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
