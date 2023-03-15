using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class ShadowMapInitializer : MonoBehaviour,IConvertGameObjectToEntity
{
    [SerializeField] private ShadowResolution _resolution = ShadowResolution.Medium;

    [SerializeField, Range(0, 1)] private float shadowBias = 0.0001f;
    [Range(0, 1)] public float shadowStrength = 1.0f;
    [SerializeField, Range(0, 10)] private float shadowFilterStride = 1.0f;
    [SerializeField,Range(0,5)] private float lightWidth=0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var tempData = new ShadowComponent
        {
            Resolution = _resolution,
            Bias = shadowBias,
            ShadowStrength = shadowStrength,
            ShadowFilterStride = shadowFilterStride,
            LightWidth = lightWidth
        };
        dstManager.AddComponentData(entity, tempData);
    }
}
