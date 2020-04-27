// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
 
// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)
// Modified to add background blurring
Shader "UI/Blurred"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _Size("Blur Radius", Range(0,10)) = 3.5
 
        _StencilComp("Stencil Comparison", Float) = 8
        _Stencil("Stencil ID", Float) = 0
        _StencilOp("Stencil Operation", Float) = 0
        _StencilWriteMask("Stencil Write Mask", Float) = 255
        _StencilReadMask("Stencil Read Mask", Float) = 255
 
        _ColorMask("Color Mask", Float) = 15
 
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Use Alpha Clip", Float) = 0
    }
 
        SubShader
        {
            Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }
 
            Stencil
        {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }
 
            Cull Off
            Lighting Off
            ZWrite Off
            ZTest[unity_GUIZTestMode]
            Blend SrcAlpha OneMinusSrcAlpha
            ColorMask[_ColorMask]
 
            // Horizontal blur
            GrabPass{
            Tags{ "LightMode" = "Always" }
        }
            Pass{
            Tags{ "LightMode" = "Always" }
 
            CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"
 
            struct appdata_t {
            float4 vertex : POSITION;
            float2 texcoord: TEXCOORD0;
        };
 
        struct v2f {
            float4 vertex : POSITION;
            float4 uvgrab : TEXCOORD0;
        };
 
        v2f vert(appdata_t v) {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
#if UNITY_UV_STARTS_AT_TOP
            float scale = -1.0;
#else
            float scale = 1.0;
#endif
            o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
            o.uvgrab.zw = o.vertex.zw;
            return o;
        }
 
        sampler2D _GrabTexture;
        float4 _GrabTexture_TexelSize;
        float _Size;
 
        half4 frag(v2f i) : COLOR{
            //                  half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
            //                  return col;
 
            half4 sum = half4(0,0,0,0);
#define GRABPIXEL(weight,kernelx) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x + _GrabTexture_TexelSize.x * kernelx*_Size, i.uvgrab.y, i.uvgrab.z, i.uvgrab.w))) * weight
            sum += GRABPIXEL(0.05, -4.0);
            sum += GRABPIXEL(0.09, -3.0);
            sum += GRABPIXEL(0.12, -2.0);
            sum += GRABPIXEL(0.15, -1.0);
            sum += GRABPIXEL(0.18,  0.0);
            sum += GRABPIXEL(0.15, +1.0);
            sum += GRABPIXEL(0.12, +2.0);
            sum += GRABPIXEL(0.09, +3.0);
            sum += GRABPIXEL(0.05, +4.0);
 
            return sum;
        }
            ENDCG
        }
            // Vertical blur
            GrabPass{
            Tags{ "LightMode" = "Always" }
        }
            Pass{
            Tags{ "LightMode" = "Always" }
 
            CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"
 
            struct appdata_t {
            float4 vertex : POSITION;
            float2 texcoord: TEXCOORD0;
        };
 
        struct v2f {
            float4 vertex : POSITION;
            float4 uvgrab : TEXCOORD0;
        };
 
        v2f vert(appdata_t v) {
            v2f o;
            o.vertex = UnityObjectToClipPos(v.vertex);
#if UNITY_UV_STARTS_AT_TOP
            float scale = -1.0;
#else
            float scale = 1.0;
#endif
            o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
            o.uvgrab.zw = o.vertex.zw;
            return o;
        }
 
        sampler2D _GrabTexture;
        float4 _GrabTexture_TexelSize;
        float _Size;
 
        half4 frag(v2f i) : COLOR{
            //                  half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
            //                  return col;
 
            half4 sum = half4(0,0,0,0);
#define GRABPIXEL(weight,kernely) tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(float4(i.uvgrab.x, i.uvgrab.y + _GrabTexture_TexelSize.y * kernely*_Size, i.uvgrab.z, i.uvgrab.w))) * weight
            //G(X) = (1/(sqrt(2*PI*deviation*deviation))) * exp(-(x*x / (2*deviation*deviation)))
 
            sum += GRABPIXEL(0.05, -4.0);
            sum += GRABPIXEL(0.09, -3.0);
            sum += GRABPIXEL(0.12, -2.0);
            sum += GRABPIXEL(0.15, -1.0);
            sum += GRABPIXEL(0.18,  0.0);
            sum += GRABPIXEL(0.15, +1.0);
            sum += GRABPIXEL(0.12, +2.0);
            sum += GRABPIXEL(0.09, +3.0);
            sum += GRABPIXEL(0.05, +4.0);
 
            return sum;
        }
            ENDCG
        }
 
        Pass
        {
            Name "Default"
            CGPROGRAM
    #pragma vertex vert
    #pragma fragment frag
    #pragma target 2.0
 
    #include "UnityCG.cginc"
    #include "UnityUI.cginc"
 
    #pragma multi_compile __ UNITY_UI_CLIP_RECT
    #pragma multi_compile __ UNITY_UI_ALPHACLIP
 
            struct appdata_t
        {
            float4 vertex   : POSITION;
            float4 color    : COLOR;
            float2 texcoord : TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };
 
        struct v2f
        {
            float4 vertex   : SV_POSITION;
            fixed4 color : COLOR;
            float2 texcoord  : TEXCOORD0;
            float4 worldPosition : TEXCOORD1;
            float4 uvgrab : TEXCOORD2;
            UNITY_VERTEX_OUTPUT_STEREO
        };
 
        sampler2D _MainTex;
        sampler2D _GrabTexture;
        float4 _GrabTexture_TexelSize;
        fixed4 _Color;
        fixed4 _TextureSampleAdd;
        float4 _ClipRect;
        float4 _MainTex_ST;
 
        v2f vert(appdata_t v)
        {
            v2f OUT;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
            OUT.worldPosition = v.vertex;
            OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);
 
            OUT.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
#if UNITY_UV_STARTS_AT_TOP
            float scale = -1.0;
#else
            float scale = 1.0;
#endif
            OUT.uvgrab.xy = (float2(OUT.vertex.x, OUT.vertex.y * scale) + OUT.vertex.w) * 0.5;
            OUT.uvgrab.zw = OUT.vertex.zw;
 
            OUT.color = v.color * _Color;
            return OUT;
        }
 
        fixed4 frag(v2f IN) : SV_Target
        {
            half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;
            half4 blur = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(IN.uvgrab));
 
    #ifdef UNITY_UI_CLIP_RECT
            color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
    #endif
 
    #ifdef UNITY_UI_ALPHACLIP
            clip(color.a - 0.001);
    #endif
 
            //color.rgb = lerp(blur.rgb, color.rgb, color.a);
            return color;
        }
            ENDCG
        }
        }
}