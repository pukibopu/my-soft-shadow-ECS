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

public class ShadowMapECS : MonoBehaviour, IConvertGameObjectToEntity
{
    public ShadowResolution shadowResolution = ShadowResolution.Medium;
    public float bias = 0.0001f;
    public float shadowStrength = 1.0f;
    public float shadowFilterStride = 1.0f;
    public float lightWidth = 0.5f;
     public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
     {
         var tempDate = new ShadowComponent
         {
            Resolution = shadowResolution,
            Bias = bias,
            ShadowStrength = shadowStrength,
            ShadowFilterStride = shadowFilterStride,
            LightWidth = lightWidth
         };
         dstManager.AddComponentData(entity, tempDate);
     }
}

