// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TVPPShader"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_DistortionRT("DistortionRT", 2D) = "white" {}
		_DistortionStrength("DistortionStrength", Float) = 0
		_NoiseSize("Noise Size", Float) = 0.01
		_NoiseOpacity("Noise Opacity", Range( 0 , 1)) = 0
		_TimeScale("TimeScale", Float) = 100
		_VignetteSize("Vignette Size", Range( 0 , 1)) = 1
		_VignetteOpacity("Vignette Opacity", Range( -1 , 0)) = 0
		_MaskTexture("Mask Texture", 2D) = "white" {}
		_MaskBlend("Mask Blend", Float) = 0.5
		_MaskSize("Mask Size", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Opaque" }
		LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_texcoord : TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
			};

			uniform float _NoiseOpacity;
			uniform float _NoiseSize;
			uniform float _TimeScale;
			uniform sampler2D _MainTex;
			uniform sampler2D _DistortionRT;
			uniform float _DistortionStrength;
			uniform sampler2D _MaskTexture;
			uniform float4 _MaskTexture_ST;
			uniform float _MaskSize;
			uniform float _MaskBlend;
			uniform float _VignetteSize;
			uniform float _VignetteOpacity;
			float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }
			float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }
			float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }
			float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }
			float snoise( float3 v )
			{
				const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
				float3 i = floor( v + dot( v, C.yyy ) );
				float3 x0 = v - i + dot( i, C.xxx );
				float3 g = step( x0.yzx, x0.xyz );
				float3 l = 1.0 - g;
				float3 i1 = min( g.xyz, l.zxy );
				float3 i2 = max( g.xyz, l.zxy );
				float3 x1 = x0 - i1 + C.xxx;
				float3 x2 = x0 - i2 + C.yyy;
				float3 x3 = x0 - 0.5;
				i = mod3D289( i);
				float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
				float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
				float4 x_ = floor( j / 7.0 );
				float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
				float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
				float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
				float4 h = 1.0 - abs( x ) - abs( y );
				float4 b0 = float4( x.xy, y.xy );
				float4 b1 = float4( x.zw, y.zw );
				float4 s0 = floor( b0 ) * 2.0 + 1.0;
				float4 s1 = floor( b1 ) * 2.0 + 1.0;
				float4 sh = -step( h, 0.0 );
				float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
				float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
				float3 g0 = float3( a0.xy, h.x );
				float3 g1 = float3( a0.zw, h.y );
				float3 g2 = float3( a1.xy, h.z );
				float3 g3 = float3( a1.zw, h.w );
				float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
				g0 *= norm.x;
				g1 *= norm.y;
				g2 *= norm.z;
				g3 *= norm.w;
				float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
				m = m* m;
				m = m* m;
				float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
				return 42.0 * dot( m, px);
			}
			
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float4 ase_clipPos = UnityObjectToClipPos(v.vertex);
				float4 screenPos = ComputeScreenPos(ase_clipPos);
				o.ase_texcoord = screenPos;
				
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				float3 vertexValue =  float3(0,0,0) ;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				fixed4 finalColor;
				float4 screenPos = i.ase_texcoord;
				float4 ase_screenPosNorm = screenPos / screenPos.w;
				ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
				float2 appendResult26 = (float2(ase_screenPosNorm.x , ase_screenPosNorm.y));
				float mulTime28 = _Time.y * _TimeScale;
				float3 appendResult27 = (float3(( appendResult26 * _NoiseSize ) , mulTime28));
				float simplePerlin3D16 = snoise( appendResult27 );
				float4 appendResult24 = (float4(simplePerlin3D16 , simplePerlin3D16 , simplePerlin3D16 , 1.0));
				float2 appendResult6 = (float2(ase_screenPosNorm.x , ase_screenPosNorm.y));
				float2 uv_MaskTexture = i.ase_texcoord1.xy * _MaskTexture_ST.xy + _MaskTexture_ST.zw;
				float4 lerpResult62 = lerp( tex2D( _MainTex, ( float4( appendResult6, 0.0 , 0.0 ) + ( tex2D( _DistortionRT, appendResult6 ) * _DistortionStrength ) ).rg ) , ( tex2D( _MaskTexture, uv_MaskTexture ) * _MaskSize ) , _MaskBlend);
				float temp_output_32_0 = sqrt( ( pow( ( ase_screenPosNorm.x - 0.5 ) , 2.0 ) + pow( ( ase_screenPosNorm.y - 0.5 ) , 2.0 ) ) );
				float4 appendResult35 = (float4(temp_output_32_0 , temp_output_32_0 , temp_output_32_0 , 1.0));
				float4 appendResult55 = (float4(_VignetteOpacity , _VignetteOpacity , _VignetteOpacity , 0.0));
				
				
				finalColor = ( ( _NoiseOpacity * appendResult24 ) + lerpResult62 + ( ( appendResult35 * _VignetteSize ) * appendResult55 ) );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=16600
191;23;1242;792;2188.601;1638.954;4.24542;False;False
Node;AmplifyShaderEditor.ScreenPosInputsNode;5;-470.9777,365.3813;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;50;-716.3354,-242.4085;Float;False;1435.88;494.5573;Vignette;12;32;35;45;41;33;31;36;43;44;52;54;55;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;11;-620.4034,-775.428;Float;False;1191.493;482.2995;Grain;11;14;24;19;16;18;17;26;27;28;21;29;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;43;-666.3354,-108.973;Float;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;21;-612.1754,-667.8423;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;44;-510.6476,-2.437755;Float;False;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;30;108.7983,309.1814;Float;False;1071.943;542.9514;Distortion;5;6;8;9;10;7;;1,1,1,1;0;0
Node;AmplifyShaderEditor.PowerNode;31;-515.7316,-114.5571;Float;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;36;-359.5596,-5.215275;Float;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;26;-417.1687,-663.4343;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-429.3935,-389.4802;Float;False;Property;_TimeScale;TimeScale;5;0;Create;True;0;0;False;0;100;100;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-584.2504,-449.6327;Float;False;Property;_NoiseSize;Noise Size;3;0;Create;True;0;0;False;0;0.01;111.72;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;6;201.8885,381.6921;Float;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;28;-258.0964,-403.5508;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-324.3752,-561.6895;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;33;-346.4755,-110.6186;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;596.6022,702.776;Float;False;Property;_DistortionStrength;DistortionStrength;2;0;Create;True;0;0;False;0;0;0.09;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;8;212.9597,552.949;Float;True;Property;_DistortionRT;DistortionRT;1;0;Create;True;0;0;False;0;None;bdee6674211c44e328e10331e18b408b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;27;-159.1104,-600.8759;Float;False;FLOAT3;4;0;FLOAT2;0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SqrtOpNode;32;-160.0228,-114.4099;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;63;1325.695,37.35309;Float;False;507.3774;667.5236;Scanlines;5;59;62;61;60;58;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;794.7565,534.7692;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;60;1375.695,365.8075;Float;False;Property;_MaskSize;Mask Size;10;0;Create;True;0;0;False;0;1;2.09;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;7;940.2597,343.8092;Float;True;2;2;0;FLOAT2;0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;58;1422.022,474.8766;Float;True;Property;_MaskTexture;Mask Texture;8;0;Create;True;0;0;False;0;None;84c0985c864bb462685a6e217613f4ea;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;16;-57.88485,-479.8091;Float;False;Simplex3D;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;35;67.05697,-127.3351;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-84.92959,137.649;Float;False;Property;_VignetteSize;Vignette Size;6;0;Create;True;0;0;False;0;1;0.71;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;54;201.5042,174.0327;Float;False;Property;_VignetteOpacity;Vignette Opacity;7;0;Create;True;0;0;False;0;0;-0.52;-1;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;55;511.4039,124.4488;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;24;170.6071,-496.7682;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;306.8528,-118.6118;Float;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-80.39944,-723.5533;Float;False;Property;_NoiseOpacity;Noise Opacity;4;0;Create;True;0;0;False;0;0;0.05;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;994.0164,-106.7251;Float;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;59;1443.198,188.4;Float;False;Property;_MaskBlend;Mask Blend;9;0;Create;True;0;0;False;0;0.5;0.12;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;1600.876,330.2594;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;573.3839,-128.4296;Float;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;62;1646.572,87.35307;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;14;222.9913,-708.0272;Float;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;25;1193.008,-364.4036;Float;False;3;3;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;1666.038,-235.6582;Float;False;True;2;Float;ASEMaterialInspector;0;1;TVPPShader;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;0;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;True;False;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;True;0;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;43;0;5;1
WireConnection;44;0;5;2
WireConnection;31;0;43;0
WireConnection;36;0;44;0
WireConnection;26;0;21;1
WireConnection;26;1;21;2
WireConnection;6;0;5;1
WireConnection;6;1;5;2
WireConnection;28;0;29;0
WireConnection;18;0;26;0
WireConnection;18;1;17;0
WireConnection;33;0;31;0
WireConnection;33;1;36;0
WireConnection;8;1;6;0
WireConnection;27;0;18;0
WireConnection;27;2;28;0
WireConnection;32;0;33;0
WireConnection;9;0;8;0
WireConnection;9;1;10;0
WireConnection;7;0;6;0
WireConnection;7;1;9;0
WireConnection;16;0;27;0
WireConnection;35;0;32;0
WireConnection;35;1;32;0
WireConnection;35;2;32;0
WireConnection;55;0;54;0
WireConnection;55;1;54;0
WireConnection;55;2;54;0
WireConnection;24;0;16;0
WireConnection;24;1;16;0
WireConnection;24;2;16;0
WireConnection;41;0;35;0
WireConnection;41;1;45;0
WireConnection;2;1;7;0
WireConnection;61;0;58;0
WireConnection;61;1;60;0
WireConnection;52;0;41;0
WireConnection;52;1;55;0
WireConnection;62;0;2;0
WireConnection;62;1;61;0
WireConnection;62;2;59;0
WireConnection;14;0;19;0
WireConnection;14;1;24;0
WireConnection;25;0;14;0
WireConnection;25;1;62;0
WireConnection;25;2;52;0
WireConnection;1;0;25;0
ASEEND*/
//CHKSM=67A74CD9724F13B315EAE90E24F68C4F3CBD7E39