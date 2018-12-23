Shader "Spine/Skeleton ETC" {
	Properties {
		_Cutoff ("Shadow alpha cutoff", Range(0,1)) = 0.1
		_MainTex ("Texture to blend", 2D) = "white" {}
	    _MainTex_A("Alpha ( Alpha )", 2D) = "white" {}
	}
	// 2 texture stage GPUs
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 100

		Pass {
			
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			Offset -1, -1
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
	        sampler2D _MainTex_A;
			float4 _MainTex_ST;
			float _Bright;

			struct appdata_t
			{
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				fixed4 color : COLOR;
				fixed gray : TEXCOORD1;
			};

			v2f vert(appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.color = v.color;
				o.gray = dot(v.color, fixed4(1, 1, 1, 0));
				return o;
			}

#define TEX2D_ALPHA( sampler, tex ) float4( tex2D( sampler, tex ).rgb,  tex2D( sampler##_A, tex ).r )

			fixed4 frag(v2f i) : COLOR
			{
				fixed4 col;
			    if (i.gray == 0)
			    {
				     col = TEX2D_ALPHA(_MainTex, i.texcoord) * i.color;
				     col.rgb = dot(col.rgb, fixed3(.222,.707,.071));
			     }
			     else
			     {
				     col = TEX2D_ALPHA(_MainTex, i.texcoord) * i.color;
			     }

			     return col;
			}
			ENDCG
		}

	}
	// 1 texture stage GPUs
	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
		LOD 100

		Cull Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		Lighting Off

		Pass {
			ColorMaterial AmbientAndDiffuse
			SetTexture [_MainTex] {
				Combine texture * primary DOUBLE, texture * primary
			}
		}
	}
}
