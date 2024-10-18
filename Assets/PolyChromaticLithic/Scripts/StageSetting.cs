using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StageData
{
    public int Id;
    public int TargetCuttingCount;
    public int MaxCuttingCount;
    public Mesh CakeShape;
    public Vector2 CakeOffset;
}

[CreateAssetMenu(menuName = "ScriptableObject/Stage Setting", fileName = "StageSetting")]
public class StageSetting : ScriptableObject
{
    public List<StageData> DataList;
}