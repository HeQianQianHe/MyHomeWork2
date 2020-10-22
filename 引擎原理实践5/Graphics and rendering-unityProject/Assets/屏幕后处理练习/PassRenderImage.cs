using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]//写了这个就可以在编辑器模式下运行
public class PassRenderImage : MonoBehaviour
{
    public float depthScale = 1;//这是给那个深度测试的shader用的

    public float grayScale = 1;//这是给那个屏幕变灰白shader用的

    public float brightness = 1;//这些是给控制亮度，对比度，饱和度的shader用的
    public float saturation = 1;
    public float contrast = 1;

    public float blendOpcity = 1;//这是图像混合shader的混合强度
    public Texture2D blendTex;

    
    public float effectAmount = 1f;//这是老旧电视的那个shader
    public float tvContrast = 1f;
    public float OldFilmEffectAmount = 1.0f;
    public Color sepiaColor = Color.white;
    public Texture2D vignetteTexture;
    public float vignetteAmount = 1.0f;
    public Texture2D scratchesTexture;
    public float scratchesXSpeed = 10.0f;
    public float scratchesYSpeed = 10.0f;
    public Texture2D dustTexture;
    public float dustYspeed = 10.0f;
    public float dustXspeed = 10.0f;
    float randomValue;


    public float nightContrast = 2;//这是夜视镜用的那个shader
    public float nightbrightness = 1;
    public Color nightVisionColor = Color.white;
    public Texture2D nightVignetteTexture;
    public Texture2D scanLineTexture;
    public float scanLineTileAmount = 4;
    public Texture2D nightVsionNoiseTexture;
    public float noiseXspeed = 100.0f;
    public float noiseYspeed = 100.0f;
    public float scale = 0.8f;
    public float distortion = 0.2f;


    public Shader postShader;
    public Material postMaterial;

    void Start()
    {
        //先检测目前平台支不支持屏幕后处理，以及我们的shader
        if (SystemInfo.supportsImageEffects == false || SystemInfo.supportsRenderTextures == false || postShader.isSupported == false)
        {
            Debug.LogWarning("This platform does not support image effects or render textures.");
            enabled = false;//不支持就把脚本关掉
        }

    }

    void Update()
    {
        randomValue = Random.Range(-1f,1f);
    }

    //这个回调函数会把当前渲染好的屏幕画面传输到src参数里，然后经过我们处理再设置会dest里，unity再把dest输出到屏幕上
    //（这个脚本必须挂在有camera组件的物体上才能接收到此回调）
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (postShader!=null)
        {
            //把该脚本里的值设置给shader
            postMaterial.SetFloat("_GrayScale", grayScale);
            postMaterial.SetFloat("_DepthScale", depthScale);

            postMaterial.SetFloat("_Brightness", brightness);
            postMaterial.SetFloat("_Saturation", saturation);
            postMaterial.SetFloat("_Contrast", contrast);

            postMaterial.SetTexture("_BlendTex", blendTex);
            postMaterial.SetFloat("_Opcity", blendOpcity);

            postMaterial.SetFloat("_effectAmount", effectAmount);
            postMaterial.SetFloat("_contrast", tvContrast);
            postMaterial.SetFloat("_OldFilmEffectAmount", OldFilmEffectAmount);
            postMaterial.SetFloat("_vignetteAmount", vignetteAmount);
            postMaterial.SetFloat("_scratchesXSpeed", scratchesXSpeed);
            postMaterial.SetFloat("_scratchesYSpeed", scratchesYSpeed);
            postMaterial.SetFloat("_dustYspeed", dustYspeed);
            postMaterial.SetFloat("_dustXspeed", dustXspeed);
            postMaterial.SetFloat("_randomValue", randomValue);
            postMaterial.SetColor("_sepiaColor", sepiaColor);
            postMaterial.SetTexture("_vignetteTexture", vignetteTexture);
            postMaterial.SetTexture("_scratchesTexture", scratchesTexture);
            postMaterial.SetTexture("_dustTexture", dustTexture);

            postMaterial.SetFloat("_nightContrast", nightContrast);
            postMaterial.SetFloat("_nightbrightness", nightbrightness);
            postMaterial.SetFloat("_scanLineTileAmount", scanLineTileAmount);
            postMaterial.SetFloat("_noiseXspeed", noiseXspeed);
            postMaterial.SetFloat("_noiseYspeed", noiseYspeed);
            postMaterial.SetFloat("_scale", scale);
            postMaterial.SetFloat("_distortion", distortion);
            postMaterial.SetTexture("_nightVignetteTexture", nightVignetteTexture);
            postMaterial.SetTexture("_scanLineTexture", scanLineTexture);
            postMaterial.SetTexture("_nightVsionNoiseTexture", nightVsionNoiseTexture);
            postMaterial.SetColor("_nightVisionColor", nightVisionColor);

            //把scr的图像传给材质，材质会利用自己的shader来计算后处理效果，然后返回到dest
            Graphics.Blit(src, dest, postMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }


}
