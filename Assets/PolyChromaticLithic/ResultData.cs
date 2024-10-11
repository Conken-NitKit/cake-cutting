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
    /// 目標とするケーキの分割数(皿の数)
    /// </summary>
    readonly public int targetCutCount;

    /// <summary>
    /// 提出までの時間(秒)
    /// </summary>
    readonly public float time;

    /// <summary>
    /// それぞれの皿のケーキの大きさ
    /// </summary>
    readonly public float[] sizes;

    /// <summary>
    /// 皿に盛り付けられず残ったケーキの大きさ
    /// </summary>
    readonly public float remainingSize;

    /// <summary>
    /// 合計のケーキの大きさ
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
    /// プレイヤーが切った回数
    /// </summary>
    readonly public int cutCount;

    public override string ToString()
    {
        string str = "目標の分割数: " + targetCutCount + "\n";
        str += "提出までの時間: " + time + "秒\n";
        str += "皿のケーキの大きさ: ";
        foreach (var size in sizes)
        {
            str += size + " ";
        }
        str += "\n";
        str += "残ったケーキの大きさ: " + remainingSize + "\n";
        str += "合計のケーキの大きさ: " + TotalSize + "\n";
        str += "プレイヤーが切った回数: " + cutCount + "\n";
        return str;
    }
}
