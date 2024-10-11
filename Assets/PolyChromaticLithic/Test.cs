using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// 複数のNormalizedで浮動小数点型の誤差が現れるかの調査用
// 特に誤差はなかった

public class Test : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            ResultData resultData = new ResultData(1, 1f, Plate.GetSizes(), 1f, 1);
            Debug.Log(resultData.ToString());
        }
    }



    // 関数: ランダムなスケールを適用したベクトルを返す
    public Vector2[] GetRandomScaledVectors(Vector2 originalVector, int n, float minScale, float maxScale)
    {
        Vector2[] resultVectors = new Vector2[n]; // n個のベクトルを格納する配列を作成
        for (int i = 0; i < n; i++)
        {
            // minScaleからmaxScaleの範囲でランダムなスケールを生成
            float randomScale = UnityEngine.Random.Range(minScale, maxScale);
            // スケールを掛けたベクトルを計算
            resultVectors[i] = originalVector * randomScale;
        }
        return resultVectors; // スケールされたベクトルの配列を返す
    }

    private void Start()
    {
        

        
        //var a = new Vector2(1f,1f);
        //var b = new Vector2(BitIncrement(1f),BitIncrement(1f));
        //Debug.Log(a.x == b.x);
        //Debug.Log(a == b);

        //
        //Debug.Log(BitIncrement(1f) == 1f);

        //for (int i = 0; i < 100; i++)
        //{        // テスト用のベクトルを作成
        //    Vector2 originalVector = new Vector2(UnityEngine.Random.value * UnityEngine.Random.Range(1, 10), UnityEngine.Random.value * UnityEngine.Random.Range(1, 10));
        //    Vector2 normalized = originalVector.normalized;
        //    // ランダムなスケールを適用したベクトルを取得
        //    Vector2[] scaledVectors = GetRandomScaledVectors(originalVector, 500, 0.000001f, 1.0f);
        //    // 結果を表示
        //    Debug.Log("Original Vector: " + originalVector);
        //    foreach (Vector2 v in scaledVectors)
        //    {
        //        if (normalized != v.normalized) Debug.LogError("Error!");
        //    }
        //}
    }

    public static float BitDecrement(float x)
    {
        int bits = BitConverter.SingleToInt32Bits(x);

        if ((bits & 0x7F800000) >= 0x7F800000)
        {
            // NaN returns NaN
            // -Infinity returns -Infinity
            // +Infinity returns float.MaxValue
            return (bits == 0x7F800000) ? float.MaxValue : x;
        }

        if (bits == 0x00000000)
        {
            // +0.0 returns -float.Epsilon
            return -float.Epsilon;
        }

        // Negative values need to be incremented
        // Positive values need to be decremented

        bits += ((bits < 0) ? +1 : -1);
        return BitConverter.Int32BitsToSingle(bits);
    }

    public static float BitIncrement(float x)
    {
        int bits = BitConverter.SingleToInt32Bits(x);

        if ((bits & 0x7F800000) >= 0x7F800000)
        {
            // NaN returns NaN
            // -Infinity returns float.MinValue
            // +Infinity returns +Infinity
            return (bits == unchecked((int)(0xFF800000))) ? float.MinValue : x;
        }

        if (bits == unchecked((int)(0x80000000)))
        {
            // -0.0 returns float.Epsilon
            return float.Epsilon;
        }

        // Negative values need to be decremented
        // Positive values need to be incremented

        bits += ((bits < 0) ? -1 : +1);
        return BitConverter.Int32BitsToSingle(bits);
    }
}
