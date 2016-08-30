Shader "Hidden/BarrelDistortion"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Diff ("Diff", Vector) = (0.0, 0.0,0,0)
	    _XOffset ("XOffset", float) = 0
		_K1 ("k1", float) = 0.0
		_K2 ("k2", float) = 0.0
		_K2 ("k3", float) = 0.0
		_K2 ("k4", float) = 0.0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float2 _Diff;
			
			float _XOffset;
			float _K1;
			float _K2;
			float _K3;
			float _K4;

			float2 calculateDistortion(float2 offset) {
				float2 offsetSquared = offset * offset;
				float radiusSquared = offsetSquared.x + offsetSquared.y;
				float distortionScale = _K1 + _K2 * radiusSquared + _K3 * radiusSquared * radiusSquared + _K4 * radiusSquared * radiusSquared * radiusSquared;
				return distortionScale * offset;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				//move this from viewport space to centered coordinates;
				float2 position = (((i.uv + float2(_XOffset,0)) + _Diff) * 2 - float2(1,1));

				float2 distorted = calculateDistortion(position) * (1.0 - _K2);

				//convert back to texcoords
				distorted = (distorted + float2(1, 1) / 2);

				if (distorted.x < 0 || distorted.y < 0)
					return float4(0, 0, 0, 1);

				if (distorted.x > 1 || distorted.y > 1)
					return float4(0, 0, 0, 1);

				fixed4 col = tex2D(_MainTex, distorted);

				return col;
			}
			ENDCG
		}
	}
}
