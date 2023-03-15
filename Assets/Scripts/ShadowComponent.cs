using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public enum ShadowResolution
{
    Low = 512,
    Medium = 1024,
    High = 2048
}
public class ShadowComponent:IComponentData
{
    public ShadowResolution Resolution;
    public float Bias;
    public float ShadowStrength;
    public float ShadowFilterStride;
    public float LightWidth;
}



