//Taken from http://www.cnblogs.com/bearworks/p/5269481.html
Shader "Custom/HiddenTwistEffect" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
	}

		SubShader
	{
		Pass
	{
		ZTest Always Cull Off ZWrite Off
		Fog{ Mode off }

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest

#include "UnityCG.cginc"

		uniform sampler2D _MainTex;

	uniform float4 _MainTex_ST;

	uniform float4 _MainTex_TexelSize;
	uniform float _Angle;
	uniform float4 _CenterRadius;

	inline float TriWave(float x)
	{
		return abs(frac(x) * 2 - 1) - 0.5;

	}
	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
		float2 uvOrig : TEXCOORD1;
	};

	v2f vert(appdata_img v)
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		float2 uv = v.texcoord.xy - _CenterRadius.xy;
		o.uv = TRANSFORM_TEX(uv, _MainTex); //MultiplyUV (UNITY_MATRIX_TEXTURE0, uv);
		o.uvOrig = uv;
		return o;
	}

	float4 frag(v2f i) : COLOR
	{
		float2 offset = i.uvOrig;
		float t = TriWave(_Time.y * 0.05);
		float angle = 1.0 - length(offset / (_CenterRadius.zw*t));
		angle = max(0, angle);
		angle = angle * _Angle * t;
		float cosLength, sinLength;
		sincos(angle, sinLength, cosLength);

		float2 uv;
		uv.x = cosLength * offset[0] - sinLength * offset[1];
		uv.y = sinLength * offset[0] + cosLength * offset[1];
		uv += _CenterRadius.xy;

		return tex2D(_MainTex, uv) * (1 - abs(angle * 0.5));
	}
		ENDCG

	}
	}

		Fallback off
}
