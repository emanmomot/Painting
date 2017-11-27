Shader "Unlit/Drip"
{
	Properties {
		_Seed ("Seed", Range(0,50)) = 0.0
		_Amt ("Amt", Range(0,1)) = 0.0
		_MainTex ("Main tex", 2D) = "white" {}
	}

	CGINCLUDE

    #pragma multi_compile CNOISE

    #include "UnityCG.cginc"
    #include "ClassicNoise3D.hlsl"

    float _Seed;
    float _Amt;

	v2f_img vert(appdata_base v)
    {
        v2f_img o;
        o.pos = UnityObjectToClipPos(v.vertex);
        o.uv = v.texcoord.xy;
        return o;
    }

    float4 frag(v2f_img i) : SV_Target
    {
        const float epsilon = 0.0001;

        float2 uv = i.uv;

        float o = 0.3;

        float s = 5.0;

        float w = 0.5;
       	
        float3 coord = float3((uv * s).x + _Seed, 0, 0);
        float3 period = float3(s, s, 1.0) * 2.0;

        o += cnoise(coord) * w;
        o *= sin(i.uv.x * 3.14159);

        //float edgeFactor = 1;

        //float edgeWidth = .1;
        //if(i.uv.x < edgeWidth) {
        //	edgeFactor = i.uv.x / edgeWidth;
        //} else if(i.uv.x > 1 - edgeWidth) {
        //	edgeFactor = (1 - i.uv.x) / edgeWidth;
        //}

        //o *= edgeFactor * edgeFactor;

        float val = 1;
        if(((1-i.uv.y) * (1/_Amt)) > o) {
        	val = 0;
        }
            

        s *= 2.0;
        w *= 0.5;

        return float4(val,val,val, val);
    }

    ENDCG
    SubShader
    {
   		Tags {"Queue"="Transparent" "RenderType"="Transparent" }
		LOD 200

		ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
}
