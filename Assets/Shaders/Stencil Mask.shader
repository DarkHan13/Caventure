Shader "Custom/Stencil Mask"
{
    Properties
    {
    	_FieldOfViewTex ("Field of View Texture", 2D) = "white" {}
        _Color ("Light Color", Color) = (1, 1, 1, 1)
        _Intensity ("Light Intensity", Range(0, 1)) = 1
        _Range ("Light Range", Range(0, 10)) = 5
    }
    SubShader
    {
        Tags { "LightMode" = "Universal2D" }
        ZWrite off
    	ColorMask 0
        LOD 100

        Stencil {
            Ref 1
            Comp always
            Pass replace     
        }
        
        Pass{

        	Blend SrcAlpha OneMinusSrcAlpha
        	Cull off
			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float4 _Color;
            float _Intensity;
            float _Range;

			

			struct appdata{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f{
				float4 position : SV_POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			v2f vert(appdata v){
				v2f o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f i) : SV_TARGET{
				// Calculate distance from the center of the mesh
                float d = length(i.uv);
 
                // Calculate the light intensity based on distance and range
                float intensity = _Intensity * (1 - saturate(d / _Range));
 
                // Multiply the light color by the intensity
                fixed4 col = _Color * intensity;
 
                return col;
			}

			ENDCG
		}
    }
    FallBack "Sprite/Default"
}
