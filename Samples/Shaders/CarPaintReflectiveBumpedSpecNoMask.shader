Shader "Car Paint Reflective Bumped Specular (Paint only, no mask)" 
	{
	Properties 
		{
		_Color ("Main Color", Color) = (1,1,1,1)	
		_Tint ("Tint Color", Color) = (1,1,1,1)	
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
		_Reflection ("Reflection", Range (0.03, 1)) = 0.25
		_ReflectColor ("Reflection Color (RGB)", Color) = (1,1,1,0.5)
		
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_Skybox ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
		//_BumpMap ("Normalmap (RGB)", 2D) = "bump" {}
		}
		
	SubShader 
		{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf BlinnPhong
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _TintMask;
		//sampler2D _BumpMap;
		samplerCUBE _Skybox;

		float4 _Color;
		float4 _Tint;
		float4 _ReflectColor;
		float _Shininess;
		float _Reflection;
		
		struct Input 
			{
			float2 uv_MainTex;
			float3 worldRefl;
			float3 viewDir;
			
			INTERNAL_DATA
			};

		void surf (Input IN, inout SurfaceOutput o) 
			{
			half4 tex = tex2D(_MainTex, IN.uv_MainTex);
			//half4 bump = tex2D(_BumpMap, IN.uv_MainTex);
			
			half4 c = tex * _Color * _Tint;			
			o.Albedo = c.rgb;
			
			o.Gloss = tex.a;
			o.Specular = _Shininess;
			
			//o.Normal = UnpackNormal(bump);			
			float3 worldRefl = WorldReflectionVector(IN, o.Normal);
			
			//float _ReflectColor = 0.5;
			half4 reflcol = texCUBE(_Skybox, worldRefl);
			half fresnel = 1.0-dot(normalize(IN.viewDir), o.Normal);			
			reflcol *= _Reflection * fresnel;
			
			o.Emission = reflcol.rgb * _ReflectColor.rgb;
			o.Alpha = 1;
			}
			
		ENDCG
		} 
	FallBack "Diffuse"
	}
/*
Shader "Car Paint Reflective" 
	{
	Properties 
		{
		_Color ("Main Color (RGB) Gloss (A)", Color) = (1,1,1,1)	
		_ReflectColor ("Reflection Color (RGB) RefStrength (A)", Color) = (1,1,1,0.5)
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		
		_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
		
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		//_BumpMap ("Normalmap", 2D) = "bump" {}
		_ReflMask ("Reflection Mask (A)", 2D) = "white" {}
		_Cube ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
		}
		
	SubShader 
		{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf BlinnPhong

		float4 _Color;
		float4 _ReflectColor;
		float _Shininess;
		
		sampler2D _MainTex;
		//sampler2D _BumpMap;
		sampler2D _ReflMask;
		samplerCUBE _Cube;

		struct Input 
			{
			float2 uv_MainTex;
			float3 worldRefl;
			float3 viewDir;
			};

		void surf (Input IN, inout SurfaceOutput o) 
			{
			half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;			
			o.Albedo = c.rgb;
			o.Alpha = c.a;			
			o.Gloss = c.a;
			o.Specular = _Shininess;
			
			//o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			
			//half fresnel = 1.0-dot(normalize(IN.viewDir), o.Normal);
			
			half4 reflcol = texCUBE(_Cube, IN.worldRefl);
			reflcol *= tex2D(_ReflMask, IN.uv_MainTex).a;
			reflcol *= _ReflectColor.a; //* fresnel;
			
			o.Emission = reflcol.rgb * _ReflectColor.rgb;
			}
			
		ENDCG
		} 
	FallBack "Diffuse"
	}
*/