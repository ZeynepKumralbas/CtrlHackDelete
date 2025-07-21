/*
This is the name of the Shader in the inspector.
You can set directory hierarchy using forward slashes,
like shown here.
*/
Shader "My Custom Shaders/Toon Shader"
{
       /*
       The properties block enables you to edit and save the defined
       Material properties.
       If you don't define anything here, you can still set properties 
       by code, but you can't edit properties from the inspector, and
       changes don't persist between sessions.
       */ 
       Properties
       {
           [MainTexture] _ColorMap ("Color Map", 2D) = "white" {}
           [MainColor] _Color ("Color", Color) = (0.91, 0.91, 0.38)
       }
       
       
       /*
       SubShaders let you define settings and programs that can vary
       depending on hardware, render pipeline, and runtime settings.
       
       This is a more complex topic, but it enables you to write
       shaders that can easily work between different render pipelines.
       */
       
       SubShader
       {
       
           /* 
           We need to make sure the Tags includes our RenderPipeline 
           so that this shader works properly.
           */
           
           Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
           
           
           /*
           These are Shader Commands that control 
           GPU-side rendering properties.
           */
           
           Cull Back
           ZWrite On
           ZTest LEqual
           ZClip Off
           
           Pass
           {
               /* 
               The lightmode tag is extremely important.
               Unity sets up and runs render passes.
               These render passes render objects depending on the
               included LightMode tags. 
               
               e.g., "Render only UniversalForwardOnly in this pass".
               
               Our other LightModes (ShadowCaster, DepthOnly, DepthNormalsOnly)
               are automatically queued up and rendered by Unity during
               the appropriate render pass.
               */
               
               Name "ForwardLit"
               Tags {"LightMode" = "UniversalForwardOnly"}
               
               
               HLSLPROGRAM
               
               // These #pragma directives make fog and Decal rendering work.
               #pragma multi_compile_fog
               #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
               
               // These #pragma directives set up Main Light Shadows.
               #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
               #pragma multi_compile _ _SHADOWS_SOFT
               
               // These #pragma directives define the function names
               // for our Vertex and Fragment stage functions
               #pragma vertex Vertex
               #pragma fragment Fragment
               
               #include "ToonShaderPass.hlsl"
               
               ENDHLSL
           }
           
           Pass
           {
               Name "ShadowCaster"
               Tags {"LightMode" = "ShadowCaster"}
               
               
               HLSLPROGRAM
               
               // This define lets us take an alternate path 
               // when we get the Clipspace Position during the Vertex stage.
               #define SHADOW_CASTER_PASS
               
               #pragma vertex Vertex
               
               // In this case, we want to use the FragmentDepthOnly
               // function instead of the Fragment function we 
               // used in the ForwardLit pass.
               #pragma fragment FragmentDepthOnly
               
               #include "ToonShaderPass.hlsl"
               
               ENDHLSL
           }
           
           Pass
           {
               Name "DepthOnly"
               Tags {"LightMode" = "DepthOnly"}
               
               
               HLSLPROGRAM
               
               #pragma vertex Vertex
               
               // Our DepthOnly Pass and ShadowCaster pass
               // both use the FragmentDepthOnly function
               #pragma fragment FragmentDepthOnly
               
               #include "ToonShaderPass.hlsl"
               
               ENDHLSL
           }
           
           Pass
           {
               Name "DepthNormalsOnly"
               Tags {"LightMode" = "DepthNormalsOnly"}
               
               
               HLSLPROGRAM
               
               #pragma vertex Vertex
               
               // And our DepthNormalsOnly pass uses our
               // Fragment function named FragmentDepthNormalsOnly.
               #pragma fragment FragmentDepthNormalsOnly
               
               #include "ToonShaderPass.hlsl"
               
               ENDHLSL
           }
       }
}
#ifndef MY_TOON_SHADER_INCLUDE
#define MY_TOON_SHADER_INCLUDE
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/SpaceTransforms.hlsl"
// See ShaderVariablesFunctions.hlsl in com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl
///////////////////////////////////////////////////////////////////////////////
//                      CBUFFER                                              //
///////////////////////////////////////////////////////////////////////////////
/*
Unity URP requires us to set up a CBUFFER
(or "Constant Buffer") of Constant Variables.
These should be the same variables we set up 
in the Properties.
This CBUFFER is REQUIRED for Unity
to correctly handle per-material changes
as well as batching / instancing.
Don't skip it :)
*/
CBUFFER_START(UnityPerMaterial)
       TEXTURE2D(_ColorMap);
       SAMPLER(sampler_ColorMap);
       float4 _ColorMap_ST;
       float3 _Color;
CBUFFER_END
///////////////////////////////////////////////////////////////////////////////
//                      STRUCTS                                              //
///////////////////////////////////////////////////////////////////////////////
/*
Our attributes struct is simple.
It contains the Object-Space Position
and Normal Direction as well as the 
UV0 coordinates for the mesh.
The Attributes struct is passed 
from the GPU to the Vertex function.
*/
struct Attributes
{
       float4 positionOS : POSITION;
       float3 normalOS   : NORMAL;
       float2 uv         : TEXCOORD0;
       
       // This line is required for VR SPI to work.
       UNITY_VERTEX_INPUT_INSTANCE_ID
};
/*
The Varyings struct is also straightforward.
It contains the Clip Space Position, the UV, and 
the World-Space Normals.
The Varyings struct is passed from the Vertex
function to the Fragment function.
*/
struct Varyings
{
       float4 positionHCS     : SV_POSITION;
       float2 uv              : TEXCOORD0;
       float3 positionWS      : TEXCOORD1;
       float3 normalWS        : TEXCOORD2;
       
       // This line is required for VR SPI to work.
    UNITY_VERTEX_INPUT_INSTANCE_ID
       UNITY_VERTEX_OUTPUT_STEREO
};
///////////////////////////////////////////////////////////////////////////////
//                      Common Lighting Transforms                           //
///////////////////////////////////////////////////////////////////////////////
// This is a global variable, Unity sets it for us.
float3 _LightDirection;
/*
This is a simple lighting transformation.
Normally, we just return the WorldToHClip position.
During the Shadow Pass, we want to make sure that Shadow Bias is baked 
in to the shadow map. To accomplish this, we use the ApplyShadowBias
method to push the world-space positions in their normal direction by the bias amount.
We define SHADOW_CASTER_PASS during the setup for the Shadow Caster pass.
*/
float4 GetClipSpacePosition(float3 positionWS, float3 normalWS)
{
       #if defined(SHADOW_CASTER_PASS)
           float4 positionHCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, _LightDirection));
           
           #if UNITY_REVERSED_Z
               positionHCS.z = min(positionHCS.z, positionHCS.w * UNITY_NEAR_CLIP_VALUE);
           #else
               positionHCS.z = max(positionHCS.z, positionHCS.w * UNITY_NEAR_CLIP_VALUE);
           #endif
           
           return positionHCS;
       #endif
       
       return TransformWorldToHClip(positionWS);
}
/*
These two functions give us the shadow coordinates 
depending on whether screen shadows are enabled or not.
We have two methods here, one with two args (positionWS
and positionHCS), and one with just positionWS.
The two-arg method is faster when you have 
already calculated the positionHCS variable.
*/
float4 GetMainLightShadowCoord(float3 positionWS, float4 positionHCS)
{
       #if defined(_MAIN_LIGHT_SHADOWS_SCREEN)
           return ComputeScreenPos(positionHCS);
       #else
           return TransformWorldToShadowCoord(positionWS);
       #endif
}
float4 GetMainLightShadowCoord(float3 PositionWS)
{
       #if defined(_MAIN_LIGHT_SHADOWS_SCREEN)
           float4 clipPos = TransformWorldToHClip(PositionWS);
           return ComputeScreenPos(clipPos);
       #else
    return TransformWorldToShadowCoord(PositionWS);
       #endif
}
/*
This method gives us the main light as an out parameter.
The Light struct is defined in 
"Packages/com.unity.render-pipelines.universal/ShaderLibrary/RealtimeLights.hlsl",
so you can reference it there for more details on its fields.
This version of the GetMainLight method doesn't account for Light Cookies.
To account for Light cookies, you need to add the following line to your shader pass:
#pragma multi_compile _ _LIGHT_COOKIES
and also call a different GetMainLight method:
GetMainLight(float4 shadowCoord, float3 positionWS, half4 shadowMask)
*/
void GetMainLightData(float3 PositionWS, out Light light)
{
       float4 shadowCoord = GetMainLightShadowCoord(PositionWS);
       light = GetMainLight(shadowCoord);
}
///////////////////////////////////////////////////////////////////////////////
//                      Functions                                            //
///////////////////////////////////////////////////////////////////////////////
/*
The Vertex function is responsible 
for generating and manipulating the 
data for each vertex of the mesh.
*/
Varyings Vertex(Attributes IN)
{
       Varyings OUT = (Varyings)0;
       
       // These macros are required for VR SPI compatibility
       UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
       
       
       // Set up each field of the Varyings struct, then return it.
       OUT.positionWS = mul(unity_ObjectToWorld, IN.positionOS).xyz;
       OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
       OUT.positionHCS = GetClipSpacePosition(OUT.positionWS, OUT.normalWS);
       OUT.uv = TRANSFORM_TEX(IN.uv, _ColorMap);
       
       return OUT;
}
/*
The FragmentDepthOnly function is responsible 
for handling per-pixel shading during the 
DepthOnly and ShadowCaster passes.
*/
float FragmentDepthOnly(Varyings IN) : SV_Target
{
       // These macros are required for VR SPI compatibility
       UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
       
       return 0;
}
/*
The FragmentDepthNormalsOnly function is responsible 
for handling per-pixel shading during the 
DepthNormalsOnly pass. This pass is less common, but
can be required by some post-process effects such as SSAO.
*/
float4 FragmentDepthNormalsOnly(Varyings IN) : SV_Target
{
       // These macros are required for VR SPI compatibility
       UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
       
       return float4(normalize(IN.normalWS), 0);
}
/*
The Fragment function is responsible 
for handling per-pixel shading during the Forward 
rendering pass. We use the ForwardOnly pass, so this works
by default in both Forward and Deferred paths.
*/
float3 Fragment(Varyings IN) : SV_Target
{
       // These macros are required for VR SPI compatibility
    UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
       
       return 0;
}
#endif
float3 Fragment(Varyings IN) : SV_Target
{
     // These macros are required for VR SPI compatibility
    UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);
     
     return _Color * SAMPLE_TEXTURE2D(_ColorMap, sampler_ColorMap, IN.uv);
}