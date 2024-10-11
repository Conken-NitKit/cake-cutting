using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// ������Normalized�ŕ��������_�^�̌덷������邩�̒����p
// ���Ɍ덷�͂Ȃ�����

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



    // �֐�: �����_���ȃX�P�[����K�p�����x�N�g����Ԃ�
    public Vector2[] GetRandomScaledVectors(Vector2 originalVector, int n, float minScale, float maxScale)
    {
        Vector2[] resultVectors = new Vector2[n]; // n�̃x�N�g�����i�[����z����쐬
        for (int i = 0; i < n; i++)
        {
            // minScale����maxScale�͈̔͂Ń����_���ȃX�P�[���𐶐�
            float randomScale = UnityEngine.Random.Range(minScale, maxScale);
            // �X�P�[�����|�����x�N�g�����v�Z
            resultVectors[i] = originalVector * randomScale;
        }
        return resultVectors; // �X�P�[�����ꂽ�x�N�g���̔z���Ԃ�
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
        //{        // �e�X�g�p�̃x�N�g�����쐬
        //    Vector2 originalVector = new Vector2(UnityEngine.Random.value * UnityEngine.Random.Range(1, 10), UnityEngine.Random.value * UnityEngine.Random.Range(1, 10));
        //    Vector2 normalized = originalVector.normalized;
        //    // �����_���ȃX�P�[����K�p�����x�N�g�����擾
        //    Vector2[] scaledVectors = GetRandomScaledVectors(originalVector, 500, 0.000001f, 1.0f);
        //    // ���ʂ�\��
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
