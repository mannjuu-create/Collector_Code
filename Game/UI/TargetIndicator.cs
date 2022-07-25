using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 画面外の宝石の位置を画面端に表示するスクリプト.
/// </summary>

[RequireComponent(typeof(RectTransform))]
public class TargetIndicator : MonoBehaviour
{
    // PauseScriptスクリプト.
    private PauseScript pauseScript;

    [HideInInspector] public Transform target = default;    // ターゲットの位置(JewelMaker.csで初期化).
    [SerializeField] private Image     arrow  = default;    // 表示する画像.

    private Camera        mainCamera;                       // メインカメラ.
    private RectTransform rectTransform;                    // 自身のRectTransform.

    private void Start()
    {
        // PauseScriptスクリプト.
        pauseScript = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<PauseScript>();

        mainCamera    = Camera.main;                        // メインカメラ.
        rectTransform = GetComponent<RectTransform>();      // 自身のRectTransform.
    }

    void Update()
    {
        // ターゲットがいないなら消去.
        if (target == null)
        {
            Destroy(this.gameObject);
        }
        // カメラがステージ全体を映しているなら非アクティブにする
        if(pauseScript.pauseFrag)
        {
            this.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        if(target == null)
        {
            return;
        }

        // キャンパスのスケール.
        float canvasScale = transform.root.localScale.z;
        // スクリーンの中心位置.
        Vector3 center = 0.5f * new Vector3(Screen.width, Screen.height);

        // ターゲットの位置から画面の中心位置を引いたベクトルを求める.
        Vector3 pos = mainCamera.WorldToScreenPoint(target.position) - center;
        // カメラ後方にあるターゲットのスクリーン座標は、画面中心に対する点対称の座標にする
        if (pos.z < 0f)
        {
            pos.x = -pos.x;
            pos.y = -pos.y;

            // カメラと水平なターゲットのスクリーン座標を補正する
            if (Mathf.Approximately(pos.y, 0f))
            {
                pos.y = -center.y;
            }
        }

        // 画面橋のスクリーン座標を求める.
        var halfSize = 0.5f * canvasScale * rectTransform.sizeDelta;
        // 画面端の表示位置をUIのサイズの半分だけ画面中心側に寄せて、画面端のUIが見切れないようにする.
        float d = Mathf.Max(
            Mathf.Abs(pos.x / (center.x - halfSize.x)),
            Mathf.Abs(pos.y / (center.y - halfSize.y))
        );
        // ターゲットのスクリーン座標が画面外なら、画面端になるよう調整する
        bool isOffscreen = (pos.z < 0f || d > 1f);
        if (isOffscreen)
        {
            pos.x /= d;
            pos.y /= d;
        }
        rectTransform.anchoredPosition = pos / canvasScale;

        // ターゲットのスクリーン座標が画面外なら、ターゲットの方向を指す矢印を表示する
        arrow.enabled = isOffscreen;
        if (isOffscreen)
        {
            arrow.rectTransform.eulerAngles = new Vector3(
                0f, 0f,
                Mathf.Atan2(pos.y, pos.x) * Mathf.Rad2Deg
            );
        }
    }
}