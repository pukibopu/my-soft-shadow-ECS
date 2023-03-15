using System;
using System.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.VisualScripting;

public class ShadowSystem : SystemBase
{
    private Shader _shadowMakerShader;
    private GameObject _lightCameraObj;
    private RenderTexture _shadowMapRt;
    private Camera _lightCamera;

    protected override void OnUpdate()
    {
        var shadowResolution = ShadowResolution.Medium;
        var shadowStrength = 1.0f;
        var shadowBias = 0.0001f;
        var shadowFilterStride = 1.0f;
        var lightWidth = 0.5f;

        Entities.WithName("UpdateShadowMap")
            .WithoutBurst()
            .ForEach((Entity entity, in ShadowComponent settings) =>
            {
                shadowResolution = settings.Resolution;
                shadowBias = settings.Bias;
                shadowStrength = settings.ShadowStrength;
                shadowFilterStride = settings.ShadowFilterStride;
                lightWidth = settings.LightWidth;
            }).Run();
        if (_lightCamera == null)
        {
            CreateCamera();
        }
        
        Matrix4x4 projectionMatrix = GL.GetGPUProjectionMatrix(_lightCamera.projectionMatrix, false);
        Shader.SetGlobalMatrix("_gWorldToShadow", projectionMatrix * _lightCamera.worldToCameraMatrix);

        Shader.SetGlobalFloat("_gShadowStrength", shadowStrength);
        Shader.SetGlobalFloat("_gShadow_bias", shadowBias);
        Shader.SetGlobalFloat("_gShadowFilterStride", shadowFilterStride);
        Shader.SetGlobalFloat("_gLightWidth",lightWidth);

        Shader.SetGlobalTexture("_gShadowMapTexture", _shadowMapRt);

        // string shadowTypeName = Enum.GetName(typeof(ShadowType), shadowType);
        // foreach (var type in Enum.GetNames(typeof(ShadowType)))
        // {
        //     if (type == shadowTypeName)
        //         Shader.EnableKeyword(type);
        //     else
        //         Shader.DisableKeyword(type);
        // }
        
    }

    protected override void OnDestroy()
    {
        Clean();
    }
    
    void Clean()
    {
        if (_lightCameraObj != null)
        {
            GameObject.Destroy(_lightCameraObj);
            _lightCameraObj = null;
            _lightCamera = null;
        }

        if (_shadowMapRt != null)
        {
            RenderTexture.ReleaseTemporary(_shadowMapRt);
            _shadowMapRt = null;
        }
    }
    
    private void CreateCamera()
    {
        _lightCameraObj = new GameObject("Directional Light Cam");

        _lightCamera = _lightCameraObj.AddComponent<Camera>();
        _lightCamera.backgroundColor = Color.white;
        _lightCamera.clearFlags = CameraClearFlags.SolidColor;
        _lightCamera.orthographic = true;
        _lightCamera.orthographicSize = 100f;
        _lightCamera.nearClipPlane = 0.3f;
        _lightCamera.farClipPlane = 100;
        _lightCamera.cullingMask = 1;
        _lightCamera.targetTexture = CreateRenderTexture(_lightCamera);


        if (_shadowMakerShader == null)
        {
            _shadowMakerShader = Shader.Find("Custom/ShadowMaker");
        }

        _lightCamera.SetReplacementShader(_shadowMakerShader, "");
    }

    private RenderTexture CreateRenderTexture(Camera cam)
    {
        if (_shadowMapRt)
        {
            RenderTexture.ReleaseTemporary(_shadowMapRt);
            _shadowMapRt = null;
        }

        RenderTextureFormat textureFormatRt = RenderTextureFormat.ARGB32;
        if (!SystemInfo.SupportsRenderTextureFormat(textureFormatRt))
        {
            textureFormatRt = RenderTextureFormat.Default;
        }

        var resolution =(int) ShadowResolution.Medium;

        _shadowMapRt = RenderTexture.GetTemporary(resolution, resolution, 64, textureFormatRt);
        _shadowMapRt.hideFlags = HideFlags.DontSave;

        Shader.SetGlobalTexture("_gShadowMapTexture", _shadowMapRt);
        return _shadowMapRt;
    }
}
