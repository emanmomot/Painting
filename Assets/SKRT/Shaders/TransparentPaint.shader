﻿Shader "Standard transparent (with paint)" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_PaintTex ("Paint Texture", 2D) = "black" {}
		_PaintAlpha("Paint Alpha", float) = 1
		_CursorTex ("Cursor Texture", 2d) = "black" {}
	}
	SubShader {
			Tags {"Queue"="Transparent" "RenderType"="Transparent" }
			LOD 200

			ZWrite Off
	        Blend SrcAlpha OneMinusSrcAlpha
			
			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows alpha:fade

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _PaintTex;

			struct Input {
				float2 uv_MainTex;
			};

			half _Glossiness;
			half _Metallic;
			float _PaintAlpha;
			fixed4 _Color;

			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			// #pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_CBUFFER_START(Props)
				// put more per-instance properties here
			UNITY_INSTANCING_CBUFFER_END

			void surf (Input IN, inout SurfaceOutputStandard o) {
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
				fixed4 paintColor = tex2D(_PaintTex, IN.uv_MainTex);
				//paintColor.a *= _PaintAlpha;
				c = paintColor * paintColor.a + c * (1 - paintColor.a);
				c.a *= _PaintAlpha;
				o.Albedo = c.rgb;
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha =  c.a;
			}
			ENDCG

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard alpha:fade
			
			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0
			
			sampler2D _CursorTex;
			
			struct Input {
				float2 uv_MainTex;
			};
			
			// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
			// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
			// #pragma instancing_options assumeuniformscaling
			UNITY_INSTANCING_CBUFFER_START(Props)
				// put more per-instance properties here
			UNITY_INSTANCING_CBUFFER_END
			
			void surf (Input IN, inout SurfaceOutputStandard o) {
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D (_CursorTex, IN.uv_MainTex);
			
				o.Albedo = c.rgb;
				o.Alpha = c.a;
			}
			ENDCG
		
	}
	FallBack "Diffuse"
}
