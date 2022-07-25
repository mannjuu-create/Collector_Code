using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 宝石を回転させ続けるスクリプト.
/// </summary>

public class JewelRotate : MonoBehaviour
{
    // 宝石の傾き.
    private Quaternion slope = Quaternion.Euler(0f, 0f, -35f);
    // 回転させる角度.
    private Quaternion angle;
    // 最終的なQuaternion.
    private Quaternion result;
    // 回転させるスピード.
    private const float SPEED = 50f;

    void Update()
    {
        angle = Quaternion.Euler(0f, Time.time * SPEED, 0f);
        result = angle * slope;
        transform.rotation = result;
    }
}
