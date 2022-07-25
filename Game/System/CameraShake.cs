using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラを揺らすスクリプト.
/// </summary>

public class CameraShake : MonoBehaviour
{
    // プレイヤーを大きく映しているカメラ,
    private GameObject mainCamera;

    // 揺らし始める前のMainCameraの位置.
    private Vector3 mainOriginalPosition;
    // 揺れている時間.
    private float shakeTime;

    void Start()
    {
        // プレイヤーを大きく映しているカメラ.
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        // 現在のMainCameraの位置を保存.
        mainOriginalPosition = mainCamera.transform.position;
        // 揺れている時間を初期化.
        shakeTime = 0;

        // 揺れている時間が指定時間を超えるまで続ける.
        while(shakeTime < duration)
        {
            // MainCameraの位置をランダムに変更.
            mainCamera.transform.position = mainOriginalPosition + Random.insideUnitSphere * magnitude;
            // 揺れている時間を加算.
            shakeTime += Time.deltaTime;
            // 指定時間に達していないのでここで処理を中断.
            yield return null;
        }
        // 指定時間を超えたら元の位置に戻す.
        mainCamera.transform.position = mainOriginalPosition;
    }
}
