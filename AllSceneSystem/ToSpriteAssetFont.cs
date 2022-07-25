using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 数字の文字列をSpriteAssetのフォントに変換するスクリプト.
/// </summary>

public class ToSpriteAssetFont : MonoBehaviour
{
    public static string FontConversion(string chars)
    {
        string result = "";
        // nullならスルー.
        if(chars == null)
        {
            return result;
        }

        // 一文字ずつ変換.
        for (int i = 0; i < chars.Length; i++)
        {
            // 文字列変換.
            string tmpChar;
            // 受け取った数字をもとにFontAssetのIDに変換する.
            switch(chars[i])
            {
                case ':':
                    tmpChar = "10";
                    break;
                // 数字はそのままIDに設定.
                default:
                    tmpChar = chars[i].ToString();
                    break;
            }
            // 「<sprite=【ID】>」の形に変換.
            result += "<sprite=" + tmpChar + ">";
        }
        return result;
    }
}
