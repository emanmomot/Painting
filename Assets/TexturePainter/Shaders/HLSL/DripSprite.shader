// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Drip Sprite"
 {  
     Properties
     {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Color", Color) = (0,0,0,1)
        _Seed("Seed", Range(0,50)) = 0
        _Amt("Amt", Range(0,1)) = 0
     }
     SubShader
     {
         Tags 
         { 
             "RenderType" = "Opaque" 
             "Queue" = "Transparent+1" 
         }
 
         Pass
         {
             ZWrite Off
             Blend SrcAlpha OneMinusSrcAlpha 
  
             CGPROGRAM
             #pragma multi_compile CNOISE

    		#include "UnityCG.cginc"
    		#include "ClassicNoise3D.hlsl"
             #pragma vertex vert
             #pragma fragment frag
             #pragma multi_compile DUMMY PIXELSNAP_ON
  
             sampler2D _MainTex;
             float4 _Color;
             float _Seed;
             float _Amt;
 
             struct Vertex
             {
                 float4 vertex : POSITION;
                 float4 color : COLOR;
                 float2 uv_MainTex : TEXCOORD0;
                 float2 uv2 : TEXCOORD1;
             };
     
             struct Fragment
             {
                 float4 vertex : POSITION;
                 float4 color : COLOR;
                 float2 uv_MainTex : TEXCOORD0;
                 float2 uv2 : TEXCOORD1;
             };
  
             Fragment vert(Vertex v)
             {
                 Fragment o;
     
                 o.vertex = UnityObjectToClipPos(v.vertex);
                 o.uv_MainTex = v.uv_MainTex;
                 o.uv2 = v.uv2;
                 o.color = v.color;
     
                 return o;
             }
                                                     
             float4 frag(Fragment IN) : COLOR
             {
             const float epsilon = 0.0001;

        float2 uv = IN.uv_MainTex;

        float o = 0.4;

        float s = 5.0;

        float w = 0.5;
       	
        float3 coord = float3((uv * s).x + _Seed, 0, 0);
        float3 period = float3(s, s, 1.0) * 2.0;

        o += cnoise(coord) * w;
        o *= sin(uv.x * 3.14159);

        //float edgeFactor = 1;

        //float edgeWidth = .1;
        //if(i.uv.x < edgeWidth) {
        //	edgeFactor = i.uv.x / edgeWidth;
        //} else if(i.uv.x > 1 - edgeWidth) {
        //	edgeFactor = (1 - i.uv.x) / edgeWidth;
        //}

        //o *= edgeFactor * edgeFactor;

        float val = 1;
        if(((1-uv.y) * (1/_Amt)) > o) {
        	val = 0;
        }
            

        s *= 2.0;
        w *= 0.5;

        return float4(IN.color.rgb, val);


             }
 
             ENDCG
         }
     }
 }