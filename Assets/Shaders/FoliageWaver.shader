// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Sprites/Waver"
{
	Properties
	{
		_MainTex("Texture", 2D) = "black" { }
		_Color("Tint", Color) = (1, 1, 1, 1)

		_Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
		_ZOffset("Z Offset", Range(0, 0.1)) = 0.05
	}

		SubShader
		{
			Tags
			{
				"Queue" = "AlphaTest"
				"IgnoreProjector" = "True"
				"PreviewType" = "Plane"
				"CanUseSpriteAtlas" = "True"
			}

			Cull Off
			Lighting Off
			Blend One OneMinusSrcAlpha
			CGPROGRAM

			#pragma surface surf Lambert vertex:vert nofog alphatest:_Cutoff
			#pragma target 3.0

			sampler2D _MainTex;

			fixed4 _Color;

			struct Input
			{
				float2 uv_MainTex;
				fixed4 color;
				float3 worldPos;
			};

			/****************************************************************************/
			/*************************************WAVING*********************************/
			/****************************************************************************/


			float4 _FoliageShake;
			float3 _FoliageZoneApplicationAndTime;
			float3 _FoliageRotation;
			float3 _FoliageTransformRotation;

			float _ZOffset;

			float4 _FastSin(float4 val)
			{
				val = val * 6.408849 - 3.1415927;
				float4 r5 = val * val;
				float4 r1 = r5 * val;
				float4 r2 = r1 * r5;
				float4 r3 = r2 * r5;

				float4 sin7 = { 1, -0.16161616, 0.0083333, -0.00019841 };

				return val + r1 * sin7.y + r2 * sin7.z + r3 * sin7.w;
			}

			float4x4 _RotationMatrix(float angle_)
			{
				float sinX = sin(angle_);
				float cosX = cos(angle_);
				float sinY = sin(angle_);
				float4x4 rotation_matrix = float4x4(cosX, -sinX, 0, 0,
													 sinY, cosX,  0, 0,
													 0,    0,     1, 0,
													 0,    0,     0, 1);
				return rotation_matrix;
			}

			float2 _Rotate(float2 point_, float angle_)
			{
				return mul(float4(point_.x, point_.y, 0, 0), _RotationMatrix(angle_));
			}

			float2 _RotateAbout(float2 point_, float2 about_, float angle_)
			{
				point_ -= about_;
				point_ = _Rotate(point_, angle_);
				return  point_ + about_;
			}

			float2 wave(float2 vertex_, float4 texcoord_, float2 world_pos_)
			{
				//Constants
				const float4 wave_x_size = float4(0.048, 0.06, 0.24, 0.096);
				const float4 wave_speed = float4 (1.2, 2, 1.6, 4.8);
				const float4 wave_x_move = float4(0.024, 0.04, -0.12, 0.096);

				//Getting properties from script
				int shake_freq = _FoliageShake.x;
				float shake_amount = _FoliageShake.y;
				float shake_bending = _FoliageShake.z;
				float shake_speed = _FoliageShake.w;

				float2 zone_application = float2(_FoliageZoneApplicationAndTime.x, _FoliageZoneApplicationAndTime.y);
				float time = _FoliageZoneApplicationAndTime.z;

				float2 rotation_pivot = float2(_FoliageRotation.x, _FoliageRotation.y);
				float rotation_amount = _FoliageRotation.z;

				float3 body_rotation = _FoliageTransformRotation;

				//First let's rotate to put it in the origin;
				vertex_ = _Rotate(vertex_, float2(-body_rotation.z, -body_rotation.y));

				//Wave calculations
				float4 waves = vertex_.x * wave_x_size;
				waves += time * shake_speed * wave_speed;
				waves = frac(waves);

				float coord = (texcoord_.y - zone_application.x) / (zone_application.y - zone_application.x);
				coord = clamp(coord, 0, 1);

				float wave_amount = coord * shake_bending;
				float4 s = _FastSin(waves);
				s *= wave_amount;
				s *= normalize(wave_speed);
				s *= shake_freq ? s : pow(s, 3);

				float3 wave_move = float3 (0, 0, 0);
				wave_move.x = dot(s, wave_x_move);
				vertex_.x += mul((float3x3)unity_WorldToObject, wave_move).x *  shake_amount;
				vertex_ = _RotateAbout(vertex_, float2(rotation_pivot.x - world_pos_.x, rotation_pivot.y - world_pos_.y), rotation_amount * texcoord_.y);

				//Rotate back
				vertex_ = _Rotate(vertex_, float2(body_rotation.z, body_rotation.y));

				return vertex_;
			}

			/****************************************************************************/
			/****************************************************************************/
			/****************************************************************************/

			void vert(inout appdata_full v, out Input o)
			{
				#if defined( PIXELSNAP_ON )
					v.vertex = UnityPixelSnap(v_.vertex);
				#endif

				UNITY_INITIALIZE_OUTPUT(Input, o);
				o.color = v.color * _Color;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);

				//Plant waving
				v.vertex.xy = wave( v.vertex.xy, v.texcoord, o.worldPos );
				v.vertex.z = ( uint)( v.texcoord.x * 1000 ) % 2 == 0 ? -_ZOffset : _ZOffset;
			}

			void surf( Input i, inout SurfaceOutput o )
			{
				fixed4 color = tex2D( _MainTex, i.uv_MainTex ) * i.color;

				o.Albedo = color.rgb * color.a;
				o.Alpha = color.a;
			}
			ENDCG
		}

			FallBack "Diffuse"
}