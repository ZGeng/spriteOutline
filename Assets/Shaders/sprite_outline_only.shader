Shader "Sprites/OnlyOutline"
{
	Properties
	{  
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1) 

		_OutlineColor("Outline Color", Color) = (1,1,1,1)
		_OutlineWidth("Outline Width", Float) = 2
		_Threshold("Threshold", Range(0, 1)) = 0.5
	}

	SubShader
	{
		Tags
		{   
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="TransparentCutout" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
            #pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
			};
			
			fixed4 _Color;
			fixed4 _TextureSampleAdd;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				return OUT;
			}

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;

			float4 _OutlineColor;
			float _OutlineWidth;
			float _Threshold;

			fixed4 frag(v2f IN) :SV_Target
			{   
				fixed4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

				float width = _MainTex_TexelSize.z, height = _MainTex_TexelSize.w;
				//directly clip the sprite content
				clip(_Threshold - color.a );
				if (color.a <= _Threshold)
				{
					half2 dir[8] = { { 0,1 },{ 1,1 },{ 1,0 },{ 1,-1 },{ 0,-1 },{ -1,-1 },{ -1,0 },{ -1,1 } };
					for (int i = 0; i < 8; i++)
					{
						float2 offset = float2(dir[i].x / width, dir[i].y / height);
						offset *= _OutlineWidth;

						half4 nearby = (tex2D(_MainTex, IN.texcoord + offset) + _TextureSampleAdd) * IN.color;
						if (nearby.a > _Threshold)
						{
							color = _OutlineColor;
							color.a = 1.0;
							break;
						}
					}
				}
				

				return color;
			}
			ENDCG
		}
	}

}

