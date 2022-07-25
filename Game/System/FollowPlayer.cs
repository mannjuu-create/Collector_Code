using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーを追いかけるカメラのスクリプト.
/// </summary>

public class FollowPlayer : MonoBehaviour
{
    // プレイヤーオブジェクト.
    private GameObject player;
    // プレイヤーから離れる距離.
    private Vector3 offset;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        offset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        Vector3 tmp = new Vector3(0, (0 - player.transform.position.y) / 4, 0);

        transform.position = Vector3.Lerp(transform.position, player.transform.position + offset + tmp, 6.0f * Time.deltaTime);
    }
}
