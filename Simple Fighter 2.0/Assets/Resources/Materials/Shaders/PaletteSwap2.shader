// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PaletteSwapper"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_PrimaryMix("Primary Mix", Vector) = (0.1,2,0,0)
		_SecondaryMix("Secondary Mix", Vector) = (0.1,2,0,0)
		_PrimarySwap("Primary Swap", Color) = (1,0.5990566,0.8967335,1)
		_SecondarySwap("Secondary Swap", Color) = (1,0.5990566,0.8967335,1)
		_MainTex("MainTex", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" }
		Cull Off
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float2 _PrimaryMix;
		uniform float4 _PrimarySwap;
		uniform float2 _SecondaryMix;
		uniform float4 _SecondarySwap;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode11 = tex2D( _MainTex, uv_MainTex );
			float4 color12 = IsGammaSpace() ? float4(0.7176471,0.02745098,0.03529412,1) : float4(0.4735315,0.002124689,0.002731743,1);
			float4 color15 = IsGammaSpace() ? float4(1,1,1,1) : float4(1,1,1,1);
			float4 color16 = IsGammaSpace() ? float4(0,0,0,1) : float4(0,0,0,1);
			float4 temp_output_14_0 = (( tex2DNode11 >= ( color12 * _PrimaryMix.x ) && tex2DNode11 <= ( color12 * _PrimaryMix.y ) ) ? color15 :  color16 );
			float4 color36 = IsGammaSpace() ? float4(0.09019608,0.04313726,0.5529412,1) : float4(0.008568125,0.003346536,0.2663557,1);
			float4 color39 = IsGammaSpace() ? float4(1,1,1,1) : float4(1,1,1,1);
			float4 color48 = IsGammaSpace() ? float4(0,0,0,1) : float4(0,0,0,1);
			o.Albedo = ( ( tex2DNode11 * ( 1.0 - temp_output_14_0 ) ) + ( _PrimarySwap * temp_output_14_0 ) + ( (( tex2DNode11 >= ( color36 * _SecondaryMix.x ) && tex2DNode11 <= ( color36 * _SecondaryMix.y ) ) ? color39 :  color48 ) * _SecondarySwap ) ).rgb;
			o.Alpha = tex2DNode11.a;
			clip( tex2DNode11.a - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16700
0;23;1242;792;2073.436;1092.041;3.085755;True;False
Node;AmplifyShaderEditor.Vector2Node;17;-496.0782,-50.63457;Float;False;Property;_PrimaryMix;Primary Mix;1;0;Create;True;0;0;False;0;0.1,2;0.8,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ColorNode;12;-860.6354,-62.81589;Float;False;Constant;_Primary;Primary;0;0;Create;True;0;0;False;0;0.7176471,0.02745098,0.03529412,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;37;-207.3075,714.1182;Float;False;Property;_SecondaryMix;Secondary Mix;2;0;Create;True;0;0;False;0;0.1,2;1,2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.ColorNode;36;-480.5753,731.1068;Float;False;Constant;_Secondary;Secondary;0;0;Create;True;0;0;False;0;0.09019608,0.04313726,0.5529412,1;0.7176471,0.02745098,0.03529412,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;11;-585.3766,-552.7074;Float;True;Property;_MainTex;MainTex;5;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;15;-573.601,191.9585;Float;False;Constant;_White;White;0;0;Create;True;0;0;False;0;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-520.5143,-162.6551;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-493.2777,94.84264;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;16;-560.3301,392.6801;Float;False;Constant;_Black;Black;0;0;Create;True;0;0;False;0;0,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;-204.507,859.5953;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;43;-231.7435,602.0979;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;39;-284.8303,956.7112;Float;False;Constant;_Color2;Color 2;0;0;Create;True;0;0;False;0;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;48;-264.8619,1144.798;Float;False;Constant;_Color0;Color 0;0;0;Create;True;0;0;False;0;0,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCCompareWithRange;14;-250.7692,-68.71638;Float;True;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;21;-72.62651,-201.1905;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;20;-303.2076,324.667;Float;False;Property;_PrimarySwap;Primary Swap;3;0;Create;True;0;0;False;0;1,0.5990566,0.8967335,1;0.5754717,0,0.4986044,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCCompareWithRange;41;1.665476,623.6221;Float;True;5;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;42;182.5033,960.5579;Float;False;Property;_SecondarySwap;Secondary Swap;4;0;Create;True;0;0;False;0;1,0.5990566,0.8967335,1;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;23.56882,135.5146;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;22;70.31296,-569.3373;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;371.4598,567.1329;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;24;374.1923,-68.21066;Float;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;35;857.6407,-357.0928;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;PaletteSwapper;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;TransparentCutout;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;18;0;12;0
WireConnection;18;1;17;1
WireConnection;19;0;12;0
WireConnection;19;1;17;2
WireConnection;40;0;36;0
WireConnection;40;1;37;2
WireConnection;43;0;36;0
WireConnection;43;1;37;1
WireConnection;14;0;11;0
WireConnection;14;1;18;0
WireConnection;14;2;19;0
WireConnection;14;3;15;0
WireConnection;14;4;16;0
WireConnection;21;0;14;0
WireConnection;41;0;11;0
WireConnection;41;1;43;0
WireConnection;41;2;40;0
WireConnection;41;3;39;0
WireConnection;41;4;48;0
WireConnection;51;0;20;0
WireConnection;51;1;14;0
WireConnection;22;0;11;0
WireConnection;22;1;21;0
WireConnection;45;0;41;0
WireConnection;45;1;42;0
WireConnection;24;0;22;0
WireConnection;24;1;51;0
WireConnection;24;2;45;0
WireConnection;35;0;24;0
WireConnection;35;9;11;4
WireConnection;35;10;11;4
ASEEND*/
//CHKSM=E52E8976D835A5AE4ED9CEF71D23252D21FAC9D8