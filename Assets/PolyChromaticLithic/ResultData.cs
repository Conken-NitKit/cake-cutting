using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultData
{
    public ResultData(int targetCutCount, float time, float[] sizes, float remainingSize, int cutCount)
    {
        this.targetCutCount = targetCutCount;
        this.time = time;
        this.sizes = sizes;
        this.remainingSize = remainingSize;
        this.cutCount = cutCount;
    }
    
    /// <summary>
    /// �ڕW�Ƃ���P�[�L�̕�����(�M�̐�)
    /// </summary>
    readonly public int targetCutCount;

    /// <summary>
    /// ��o�܂ł̎���(�b)
    /// </summary>
    readonly public float time;

    /// <summary>
    /// ���ꂼ��̎M�̃P�[�L�̑傫��
    /// </summary>
    readonly public float[] sizes;

    /// <summary>
    /// �M�ɐ���t����ꂸ�c�����P�[�L�̑傫��
    /// </summary>
    readonly public float remainingSize;

    /// <summary>
    /// ���v�̃P�[�L�̑傫��
    /// </summary>
    public float TotalSize
    {
        get
        {
            float totalSize = 0;
            foreach (var size in sizes)
            {
                totalSize += size;
            }
            return totalSize + remainingSize;
        }
    }

    /// <summary>
    /// �v���C���[���؂�����
    /// </summary>
    readonly public int cutCount;

    public override string ToString()
    {
        string str = "�ڕW�̕�����: " + targetCutCount + "\n";
        str += "��o�܂ł̎���: " + time + "�b\n";
        str += "�M�̃P�[�L�̑傫��: ";
        foreach (var size in sizes)
        {
            str += size + " ";
        }
        str += "\n";
        str += "�c�����P�[�L�̑傫��: " + remainingSize + "\n";
        str += "���v�̃P�[�L�̑傫��: " + TotalSize + "\n";
        str += "�v���C���[���؂�����: " + cutCount + "\n";
        return str;
    }
}
