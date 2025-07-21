Shader "Custom/Tooning"{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
// Physically based Standard lighting model, 
// and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows 
  
// Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0  

        sampler2D _MainTex;
        fixed4 _Color;

        struct Input
        {
            float2 uv_MainTex;
        }; 

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            
        }
        ENDCG
    }
    FallBack "Diffuse"
}
Properties
{
    _Color ("Color", Color) = (1,1,1,1)
    _MainTex ("Albedo (RGB)", 2D) = "white" {}
    _StepCount ("Lighting Step Count", Range(0, 10)) = 4
}
SubShader
{
  CGPROGRAM
  ...........
  
  float _StepCount;
  
  ...........
  ENDCG
}
SubShader
{
    Tags { "RenderType"="Opaque" }
    LOD 200
    
    CGPROGRAM
    #pragma surface surf toon fullforwardshadows 
    
    // custom toon lighting model, and enable shadows on all light types  
    #pragma target 3.0  
    
    ......
    ENDCG
}
  half4 Lightingtoon(SurfaceOutput o, half3 lightDir, half3 viewDir, half atten)
{
//Diffuse lighting with stepping
float3 normalizedLightDir = normalize(lightDir);
float3 normalizedNormal = normalize(o.Normal);
float NdotL = max(0, dot(normalizedNormal, normalizedLightDir));
float steppedDiffuse = round(NdotL * _StepCount) / _StepCount;
float3 diffuseColor = o.Albedo * steppedDiffuse;
return half4(diffuseColor.rgb, o.Alpha);

}
 void surf (Input IN, inout SurfaceOutput o)
        {
         ...///......//......///...
        }
         Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _StepCount ("Lighting Step Count", Range(0, 10)) = 4

        _SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
        _SpecularStepCount ("Specular Step Count", Range(1, 10)) = 4
        _Shininess ("Specular Shininess", Range(1, 128)) = 16
    
    }
     SubShader
{
    CGPROGRAM
        #pragma surface surf toon fullforwardshadows 
        #pragma target 3.0  

        sampler2D _MainTex;
        fixed4 _Color;
        float _StepCount;

        fixed4 _SpecularColor;
        float _SpecularStepCount;
        float _Shininess;
        .........
        ENDCG
}
 half4 Lightingtoon(SurfaceOutput o, half3 lightDir, half3 viewDir, half atten)
{
  // Diffuse lighting with stepping
  .............
  
  // Specular highlight with falloff
  float3 normalizedVDir=normalize(viewDir);
  float3 normalizedRLightDir = normalize(-lightDir);
  float3 reflectDir = reflect(normalizedRLightDir,normalizedNormal);
  float RdotV = max(0, dot(normalizedVDir, reflectDir));
  float specular = pow(RdotV, _Shininess);
  float steppedSpecular = round(specular * _SpecularStepCount)/_SpecularStepCount;
  float3 specularColor = _SpecularColor.rgb * steppedSpecular;
  float3 finalColor = (diffuseColor + specularColor);
   
   return half4(finalColor, o.Alpha);
 }
 float3 finalColor = (diffuseColor + specularColor);
return half4(finalColor, o.Alpha);
Properties
    {
    _Color ("Color", Color) = (1,1,1,1)
    _MainTex ("Albedo (RGB)", 2D) = "white" {}
    _StepCount ("Lighting Step Count", Range(0, 10)) = 4

    _SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
    _SpecularStepCount ("Specular Step Count", Range(1, 10)) = 4
    _Shininess ("Specular Shininess", Range(1, 128)) = 16

    _FalloffStrength ("Specular Falloff Strength", Range(0, 5)) = 1.0
        
    }
     SubShader
{
    CGPROGRAM
        #pragma surface surf toon fullforwardshadows 
        #pragma target 3.0  

        sampler2D _MainTex;
        fixed4 _Color;
        float _StepCount;

        fixed4 _SpecularColor;
        float _SpecularStepCount;
        float _Shininess;
        
        float _FalloffStrength;
        .........
        ENDCG
}
half4 Lightingtoon(SurfaceOutput o, half3 lightDir, half3 viewDir, half atten)
{
// Diffuse lighting with stepping
float3 normalizedNormal = normalize(o.Normal);
.............

// Specular highlight with falloff
float3 normalizedVDir=normalize(viewDir);
.............

// Falloff towards the edges
   float viewDotNormal = max(0, dot(normalizedVDir, normalizedNormal));
   float falloff = pow(viewDotNormal, _FalloffStrength);
   float3 specularColor = _SpecularColor.rgb * steppedSpecular*falloff;
   float3 finalColor = (diffuseColor + specularColor);

 return half4(finalColor, o.Alpha);
}
float3 specularColor = _SpecularColor.rgb * steppedSpecular*falloff;
float3 finalColor = (diffuseColor + specularColor);
return half4(finalColor, o.Alpha);
 Properties
    {
    _Color ("Color", Color) = (1,1,1,1)
    _MainTex ("Albedo (RGB)", 2D) = "white" {}
    _StepCount ("Lighting Step Count", Range(0, 10)) = 4

    _SpecularColor ("Specular Color", Color) = (1, 1, 1, 1)
    _SpecularStepCount ("Specular Step Count", Range(1, 10)) = 4
    _Shininess ("Specular Shininess", Range(1, 128)) = 16
    
    _FalloffStrength ("Specular Falloff Strength", Range(0, 5)) = 1.0

     [HDR]
    _RimColor("Rim Color", Color) = (1,1,1,1)
    _RimAmount("Rim Amount", Range(0, 1.1)) = 0.716

    }
    SubShader
{
    CGPROGRAM
        #pragma surface surf toon fullforwardshadows 
        #pragma target 3.0  

        sampler2D _MainTex;
        fixed4 _Color;
        float _StepCount;

        fixed4 _SpecularColor;
        float _SpecularStepCount;
        float _Shininess;
        
        float _FalloffStrength;
        
        float4 _RimColor;
        float _RimAmount;
        .........
        ENDCG
}
half4 Lightingtoon(SurfaceOutput o, half3 lightDir, half3 viewDir, half atten)
        {
            // Diffuse lighting with stepping
            float3 normalizedNormal = normalize(o.Normal);
            .............
            
            // Specular highlight with falloff
            float3 normalizedVDir=normalize(viewDir);
            .............

            // Falloff towards the edges
            ...............
               
           //creating my own rim lighting
           float4 rimDot = 1 - dot(normalizedVDir, normalizedNormal);
           float rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimDot);
           float4 rim = rimIntensity * _RimColor;
           
           // Shadow attenuation
            float shadowFactor = atten;
             
           float3 finalColor = (diffuseColor + specularColor) *shadowFactor;
             return half4(finalColor + rim.rgb, o.Alpha);
             }
             float3 specularColor = _SpecularColor.rgb * steppedSpecular*falloff;
  float3 finalColor = (diffuseColor + specularColor)* shadowFactor;
  return half4(finalColor + rim.rgb, o.Alpha);
