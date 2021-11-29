Shader "CloudsModified"
{
    Properties
    {
        Vector4_BCD8D570("Rotation Projection", Vector) = (1, 0, 0, 0)
        Vector1_1C365E49("Noise Scale", Float) = 10
        Vector1_412672A0("Scroll Speed", Float) = 0.1
        Vector1_C4AC984B("Noise Height", Float) = 0.1
        Vector4_B5D7B794("Noise Remap", Vector) = (0, 1, -1, 1)
        [HDR]Color_C335EDAA("Colour Peak", Color) = (1, 1, 1, 0)
        [HDR]Color_4CF0EDD("Colour Valley", Color) = (0, 0, 0, 0)
        Vector1_703B551F("Noise Edge Min", Float) = 0
        Vector1_A99D943C("Noise Edge Max", Float) = 1
        Vector1_CEA3FF9C("Noise Power", Float) = 0
        Vector1_218FD75("Base Scale", Float) = 5
        Vector1_48832F08("Base Speed", Float) = 0.2
        Vector1_CEFA9380("Base Strength", Float) = 2
        Vector1_198CECC0("Curvature Radius", Float) = 0
        Vector1_2CA6C852("Fresnel Power", Float) = 0
        Vector1_19BDB10F("Fresnel Opacity", Float) = 0
        Vector1_5208AC81("Intersection Fade Depth", Float) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "Queue"="Transparent+0"
        }
        
        Pass
        {
            Name "Pass"
            Tags 
            { 
                // LightMode: <None>
            }
           
            // Render State
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            Cull Off
            ZTest LEqual
            //CHANGED FROM AUTO-GENERATED CODE TO FIX SPONTANEOUS VERTEX CLIPPING (Originally ZWrite Off)
            ZWrite On
            // ColorMask: <None>
            
        
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            // Pragmas
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma multi_compile_fog
            #pragma multi_compile_instancing
        
            // Keywords
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma shader_feature _ _SAMPLE_GI
            // GraphKeywords: <None>
            
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_POSITION_WS 
            #define VARYINGS_NEED_NORMAL_WS
            #define VARYINGS_NEED_VIEWDIRECTION_WS
            #define FEATURES_GRAPH_VERTEX
            #pragma multi_compile_instancing
            #define SHADERPASS_UNLIT
            #define REQUIRE_DEPTH_TEXTURE
            
        
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float4 Vector4_BCD8D570;
            float Vector1_1C365E49;
            float Vector1_412672A0;
            float Vector1_C4AC984B;
            float4 Vector4_B5D7B794;
            float4 Color_C335EDAA;
            float4 Color_4CF0EDD;
            float Vector1_703B551F;
            float Vector1_A99D943C;
            float Vector1_CEA3FF9C;
            float Vector1_218FD75;
            float Vector1_48832F08;
            float Vector1_CEFA9380;
            float Vector1_198CECC0;
            float Vector1_2CA6C852;
            float Vector1_19BDB10F;
            float Vector1_5208AC81;
            CBUFFER_END
        
            // Graph Functions
            
            void Unity_Distance_float3(float3 A, float3 B, out float Out)
            {
                Out = distance(A, B);
            }
            
            void Unity_Divide_float(float A, float B, out float Out)
            {
                Out = A / B;
            }
            
            void Unity_Power_float(float A, float B, out float Out)
            {
                Out = pow(A, B);
            }
            
            void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
            {
                Out = A * B;
            }
            
            void Unity_Rotate_About_Axis_Degrees_float(float3 In, float3 Axis, float Rotation, out float3 Out)
            {
                Rotation = radians(Rotation);
            
                float s = sin(Rotation);
                float c = cos(Rotation);
                float one_minus_c = 1.0 - c;
                
                Axis = normalize(Axis);
            
                float3x3 rot_mat = { one_minus_c * Axis.x * Axis.x + c,            one_minus_c * Axis.x * Axis.y - Axis.z * s,     one_minus_c * Axis.z * Axis.x + Axis.y * s,
                                          one_minus_c * Axis.x * Axis.y + Axis.z * s,   one_minus_c * Axis.y * Axis.y + c,              one_minus_c * Axis.y * Axis.z - Axis.x * s,
                                          one_minus_c * Axis.z * Axis.x - Axis.y * s,   one_minus_c * Axis.y * Axis.z + Axis.x * s,     one_minus_c * Axis.z * Axis.z + c
                                        };
            
                Out = mul(rot_mat,  In);
            }
            
            void Unity_Preview_float3(float3 In, out float3 Out)
            {
                Out = In;
            }
            
            void Unity_Multiply_float(float A, float B, out float Out)
            {
                Out = A * B;
            }
            
            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }
            
            
            float2 Unity_GradientNoise_Dir_float(float2 p)
            {
                // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
                p = p % 289;
                float x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }
            
            void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
            { 
                float2 p = UV * Scale;
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
                float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
            }
            
            void Unity_Add_float(float A, float B, out float Out)
            {
                Out = A + B;
            }
            
            void Unity_Saturate_float(float In, out float Out)
            {
                Out = saturate(In);
            }
            
            void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
            {
                RGBA = float4(R, G, B, A);
                RGB = float3(R, G, B);
                RG = float2(R, G);
            }
            
            void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
            {
                Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
            }
            
            void Unity_Absolute_float(float In, out float Out)
            {
                Out = abs(In);
            }
            
            void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
            {
                Out = smoothstep(Edge1, Edge2, In);
            }
            
            void Unity_Add_float3(float3 A, float3 B, out float3 Out)
            {
                Out = A + B;
            }
            
            void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
            {
                Out = lerp(A, B, T);
            }
            
            void Unity_FresnelEffect_float(float3 Normal, float3 ViewDir, float Power, out float Out)
            {
                Out = pow((1.0 - saturate(dot(normalize(Normal), normalize(ViewDir)))), Power);
            }
            
            void Unity_Preview_float(float In, out float Out)
            {
                Out = In;
            }
            
            void Unity_Add_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A + B;
            }
            
            void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
            {
                Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
            }
            
            void Unity_Subtract_float(float A, float B, out float Out)
            {
                Out = A - B;
            }
        
            // Graph Vertex
            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 WorldSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
                float3 WorldSpacePosition;
                float3 TimeParameters;
            };
            
            struct VertexDescription
            {
                float3 VertexPosition;
                float3 VertexNormal;
                float3 VertexTangent;
            };
            
            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                float _Distance_8E5A0BF8_Out_2;
                Unity_Distance_float3(SHADERGRAPH_OBJECT_POSITION, IN.WorldSpacePosition, _Distance_8E5A0BF8_Out_2);
                float _Property_9FFB8E45_Out_0 = Vector1_198CECC0;
                float _Divide_3CA900CE_Out_2;
                Unity_Divide_float(_Distance_8E5A0BF8_Out_2, _Property_9FFB8E45_Out_0, _Divide_3CA900CE_Out_2);
                float _Power_20D4C9FC_Out_2;
                Unity_Power_float(_Divide_3CA900CE_Out_2, 3, _Power_20D4C9FC_Out_2);
                float3 _Multiply_EC7D9BE5_Out_2;
                Unity_Multiply_float(IN.WorldSpaceNormal, (_Power_20D4C9FC_Out_2.xxx), _Multiply_EC7D9BE5_Out_2);
                float _Property_159635A8_Out_0 = Vector1_703B551F;
                float _Property_F7E5C678_Out_0 = Vector1_A99D943C;
                float4 _Property_AA65CC00_Out_0 = Vector4_BCD8D570;
                float _Split_F7FC9C8E_R_1 = _Property_AA65CC00_Out_0[0];
                float _Split_F7FC9C8E_G_2 = _Property_AA65CC00_Out_0[1];
                float _Split_F7FC9C8E_B_3 = _Property_AA65CC00_Out_0[2];
                float _Split_F7FC9C8E_A_4 = _Property_AA65CC00_Out_0[3];
                float3 _RotateAboutAxis_6A2ED9B0_Out_3;
                Unity_Rotate_About_Axis_Degrees_float(IN.WorldSpacePosition, (_Property_AA65CC00_Out_0.xyz), _Split_F7FC9C8E_A_4, _RotateAboutAxis_6A2ED9B0_Out_3);
                float3 _Preview_5A3F1FC8_Out_1;
                Unity_Preview_float3(_RotateAboutAxis_6A2ED9B0_Out_3, _Preview_5A3F1FC8_Out_1);
                float _Property_2C5852D1_Out_0 = Vector1_412672A0;
                float _Multiply_5E3E3E8F_Out_2;
                Unity_Multiply_float(IN.TimeParameters.x, _Property_2C5852D1_Out_0, _Multiply_5E3E3E8F_Out_2);
                float2 _TilingAndOffset_DC089697_Out_3;
                Unity_TilingAndOffset_float((_Preview_5A3F1FC8_Out_1.xy), float2 (1, 1), (_Multiply_5E3E3E8F_Out_2.xx), _TilingAndOffset_DC089697_Out_3);
                float _Property_3B2E35FA_Out_0 = Vector1_1C365E49;
                float _GradientNoise_111A2408_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_DC089697_Out_3, _Property_3B2E35FA_Out_0, _GradientNoise_111A2408_Out_2);
                float3 _Preview_88ECF05E_Out_1;
                Unity_Preview_float3(_RotateAboutAxis_6A2ED9B0_Out_3, _Preview_88ECF05E_Out_1);
                float2 _TilingAndOffset_495B1598_Out_3;
                Unity_TilingAndOffset_float((_Preview_88ECF05E_Out_1.xy), float2 (1, 1), float2 (0, 0), _TilingAndOffset_495B1598_Out_3);
                float _GradientNoise_DC945C06_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_495B1598_Out_3, _Property_3B2E35FA_Out_0, _GradientNoise_DC945C06_Out_2);
                float _Add_E5060FA0_Out_2;
                Unity_Add_float(_GradientNoise_111A2408_Out_2, _GradientNoise_DC945C06_Out_2, _Add_E5060FA0_Out_2);
                float _Divide_6F6F9F0C_Out_2;
                Unity_Divide_float(_Add_E5060FA0_Out_2, 2, _Divide_6F6F9F0C_Out_2);
                float _Saturate_606F110C_Out_1;
                Unity_Saturate_float(_Divide_6F6F9F0C_Out_2, _Saturate_606F110C_Out_1);
                float _Property_63E9F3C3_Out_0 = Vector1_CEA3FF9C;
                float _Power_83B4F216_Out_2;
                Unity_Power_float(_Saturate_606F110C_Out_1, _Property_63E9F3C3_Out_0, _Power_83B4F216_Out_2);
                float4 _Property_4C7FF516_Out_0 = Vector4_B5D7B794;
                float _Split_5274083C_R_1 = _Property_4C7FF516_Out_0[0];
                float _Split_5274083C_G_2 = _Property_4C7FF516_Out_0[1];
                float _Split_5274083C_B_3 = _Property_4C7FF516_Out_0[2];
                float _Split_5274083C_A_4 = _Property_4C7FF516_Out_0[3];
                float4 _Combine_1EEC015E_RGBA_4;
                float3 _Combine_1EEC015E_RGB_5;
                float2 _Combine_1EEC015E_RG_6;
                Unity_Combine_float(_Split_5274083C_R_1, _Split_5274083C_G_2, 0, 0, _Combine_1EEC015E_RGBA_4, _Combine_1EEC015E_RGB_5, _Combine_1EEC015E_RG_6);
                float _Split_5355AC72_R_1 = _Property_4C7FF516_Out_0[0];
                float _Split_5355AC72_G_2 = _Property_4C7FF516_Out_0[1];
                float _Split_5355AC72_B_3 = _Property_4C7FF516_Out_0[2];
                float _Split_5355AC72_A_4 = _Property_4C7FF516_Out_0[3];
                float4 _Combine_C84FFC89_RGBA_4;
                float3 _Combine_C84FFC89_RGB_5;
                float2 _Combine_C84FFC89_RG_6;
                Unity_Combine_float(_Split_5355AC72_B_3, _Split_5355AC72_A_4, 0, 0, _Combine_C84FFC89_RGBA_4, _Combine_C84FFC89_RGB_5, _Combine_C84FFC89_RG_6);
                float _Remap_348B5DED_Out_3;
                Unity_Remap_float(_Power_83B4F216_Out_2, _Combine_1EEC015E_RG_6, _Combine_C84FFC89_RG_6, _Remap_348B5DED_Out_3);
                float _Absolute_3E435394_Out_1;
                Unity_Absolute_float(_Remap_348B5DED_Out_3, _Absolute_3E435394_Out_1);
                float _Smoothstep_F2D936B8_Out_3;
                Unity_Smoothstep_float(_Property_159635A8_Out_0, _Property_F7E5C678_Out_0, _Absolute_3E435394_Out_1, _Smoothstep_F2D936B8_Out_3);
                float3 _Preview_D7C48141_Out_1;
                Unity_Preview_float3(_RotateAboutAxis_6A2ED9B0_Out_3, _Preview_D7C48141_Out_1);
                float _Property_E8D50C61_Out_0 = Vector1_48832F08;
                float _Multiply_3E37F4A4_Out_2;
                Unity_Multiply_float(IN.TimeParameters.x, _Property_E8D50C61_Out_0, _Multiply_3E37F4A4_Out_2);
                float2 _TilingAndOffset_87133117_Out_3;
                Unity_TilingAndOffset_float((_Preview_D7C48141_Out_1.xy), float2 (1, 1), (_Multiply_3E37F4A4_Out_2.xx), _TilingAndOffset_87133117_Out_3);
                float _Property_20C66F3A_Out_0 = Vector1_218FD75;
                float _GradientNoise_DD3A9097_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_87133117_Out_3, _Property_20C66F3A_Out_0, _GradientNoise_DD3A9097_Out_2);
                float _Property_3911A428_Out_0 = Vector1_CEFA9380;
                float _Multiply_58919E8E_Out_2;
                Unity_Multiply_float(_GradientNoise_DD3A9097_Out_2, _Property_3911A428_Out_0, _Multiply_58919E8E_Out_2);
                float _Add_351D4C08_Out_2;
                Unity_Add_float(_Smoothstep_F2D936B8_Out_3, _Multiply_58919E8E_Out_2, _Add_351D4C08_Out_2);
                float _Add_42DEA188_Out_2;
                Unity_Add_float(_Property_3911A428_Out_0, 1, _Add_42DEA188_Out_2);
                float _Divide_652DC47F_Out_2;
                Unity_Divide_float(_Add_351D4C08_Out_2, _Add_42DEA188_Out_2, _Divide_652DC47F_Out_2);
                float3 _Multiply_F8E682E_Out_2;
                Unity_Multiply_float(IN.ObjectSpaceNormal, (_Divide_652DC47F_Out_2.xxx), _Multiply_F8E682E_Out_2);
                float _Property_38F4B400_Out_0 = Vector1_C4AC984B;
                float3 _Multiply_DA1124D6_Out_2;
                Unity_Multiply_float(_Multiply_F8E682E_Out_2, (_Property_38F4B400_Out_0.xxx), _Multiply_DA1124D6_Out_2);
                float3 _Add_C5A1A8CF_Out_2;
                Unity_Add_float3(IN.ObjectSpacePosition, _Multiply_DA1124D6_Out_2, _Add_C5A1A8CF_Out_2);
                float3 _Add_692861E2_Out_2;
                Unity_Add_float3(_Multiply_EC7D9BE5_Out_2, _Add_C5A1A8CF_Out_2, _Add_692861E2_Out_2);
                float3 _Preview_34DB2B41_Out_1;
                Unity_Preview_float3(_Add_692861E2_Out_2, _Preview_34DB2B41_Out_1);
                description.VertexPosition = _Preview_34DB2B41_Out_1;
                description.VertexNormal = IN.ObjectSpaceNormal;
                description.VertexTangent = IN.ObjectSpaceTangent;
                return description;
            }
            
            // Graph Pixel
            struct SurfaceDescriptionInputs
            {
                float3 WorldSpaceNormal;
                float3 WorldSpaceViewDirection;
                float3 WorldSpacePosition;
                float4 ScreenPosition;
                float3 TimeParameters;
            };
            
            struct SurfaceDescription
            {
                float3 Color;
                float Alpha;
                float AlphaClipThreshold;
            };
            
            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float4 _Property_AD777D88_Out_0 = Color_4CF0EDD;
                float4 _Property_337A1152_Out_0 = Color_C335EDAA;
                float _Property_159635A8_Out_0 = Vector1_703B551F;
                float _Property_F7E5C678_Out_0 = Vector1_A99D943C;
                float4 _Property_AA65CC00_Out_0 = Vector4_BCD8D570;
                float _Split_F7FC9C8E_R_1 = _Property_AA65CC00_Out_0[0];
                float _Split_F7FC9C8E_G_2 = _Property_AA65CC00_Out_0[1];
                float _Split_F7FC9C8E_B_3 = _Property_AA65CC00_Out_0[2];
                float _Split_F7FC9C8E_A_4 = _Property_AA65CC00_Out_0[3];
                float3 _RotateAboutAxis_6A2ED9B0_Out_3;
                Unity_Rotate_About_Axis_Degrees_float(IN.WorldSpacePosition, (_Property_AA65CC00_Out_0.xyz), _Split_F7FC9C8E_A_4, _RotateAboutAxis_6A2ED9B0_Out_3);
                float3 _Preview_5A3F1FC8_Out_1;
                Unity_Preview_float3(_RotateAboutAxis_6A2ED9B0_Out_3, _Preview_5A3F1FC8_Out_1);
                float _Property_2C5852D1_Out_0 = Vector1_412672A0;
                float _Multiply_5E3E3E8F_Out_2;
                Unity_Multiply_float(IN.TimeParameters.x, _Property_2C5852D1_Out_0, _Multiply_5E3E3E8F_Out_2);
                float2 _TilingAndOffset_DC089697_Out_3;
                Unity_TilingAndOffset_float((_Preview_5A3F1FC8_Out_1.xy), float2 (1, 1), (_Multiply_5E3E3E8F_Out_2.xx), _TilingAndOffset_DC089697_Out_3);
                float _Property_3B2E35FA_Out_0 = Vector1_1C365E49;
                float _GradientNoise_111A2408_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_DC089697_Out_3, _Property_3B2E35FA_Out_0, _GradientNoise_111A2408_Out_2);
                float3 _Preview_88ECF05E_Out_1;
                Unity_Preview_float3(_RotateAboutAxis_6A2ED9B0_Out_3, _Preview_88ECF05E_Out_1);
                float2 _TilingAndOffset_495B1598_Out_3;
                Unity_TilingAndOffset_float((_Preview_88ECF05E_Out_1.xy), float2 (1, 1), float2 (0, 0), _TilingAndOffset_495B1598_Out_3);
                float _GradientNoise_DC945C06_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_495B1598_Out_3, _Property_3B2E35FA_Out_0, _GradientNoise_DC945C06_Out_2);
                float _Add_E5060FA0_Out_2;
                Unity_Add_float(_GradientNoise_111A2408_Out_2, _GradientNoise_DC945C06_Out_2, _Add_E5060FA0_Out_2);
                float _Divide_6F6F9F0C_Out_2;
                Unity_Divide_float(_Add_E5060FA0_Out_2, 2, _Divide_6F6F9F0C_Out_2);
                float _Saturate_606F110C_Out_1;
                Unity_Saturate_float(_Divide_6F6F9F0C_Out_2, _Saturate_606F110C_Out_1);
                float _Property_63E9F3C3_Out_0 = Vector1_CEA3FF9C;
                float _Power_83B4F216_Out_2;
                Unity_Power_float(_Saturate_606F110C_Out_1, _Property_63E9F3C3_Out_0, _Power_83B4F216_Out_2);
                float4 _Property_4C7FF516_Out_0 = Vector4_B5D7B794;
                float _Split_5274083C_R_1 = _Property_4C7FF516_Out_0[0];
                float _Split_5274083C_G_2 = _Property_4C7FF516_Out_0[1];
                float _Split_5274083C_B_3 = _Property_4C7FF516_Out_0[2];
                float _Split_5274083C_A_4 = _Property_4C7FF516_Out_0[3];
                float4 _Combine_1EEC015E_RGBA_4;
                float3 _Combine_1EEC015E_RGB_5;
                float2 _Combine_1EEC015E_RG_6;
                Unity_Combine_float(_Split_5274083C_R_1, _Split_5274083C_G_2, 0, 0, _Combine_1EEC015E_RGBA_4, _Combine_1EEC015E_RGB_5, _Combine_1EEC015E_RG_6);
                float _Split_5355AC72_R_1 = _Property_4C7FF516_Out_0[0];
                float _Split_5355AC72_G_2 = _Property_4C7FF516_Out_0[1];
                float _Split_5355AC72_B_3 = _Property_4C7FF516_Out_0[2];
                float _Split_5355AC72_A_4 = _Property_4C7FF516_Out_0[3];
                float4 _Combine_C84FFC89_RGBA_4;
                float3 _Combine_C84FFC89_RGB_5;
                float2 _Combine_C84FFC89_RG_6;
                Unity_Combine_float(_Split_5355AC72_B_3, _Split_5355AC72_A_4, 0, 0, _Combine_C84FFC89_RGBA_4, _Combine_C84FFC89_RGB_5, _Combine_C84FFC89_RG_6);
                float _Remap_348B5DED_Out_3;
                Unity_Remap_float(_Power_83B4F216_Out_2, _Combine_1EEC015E_RG_6, _Combine_C84FFC89_RG_6, _Remap_348B5DED_Out_3);
                float _Absolute_3E435394_Out_1;
                Unity_Absolute_float(_Remap_348B5DED_Out_3, _Absolute_3E435394_Out_1);
                float _Smoothstep_F2D936B8_Out_3;
                Unity_Smoothstep_float(_Property_159635A8_Out_0, _Property_F7E5C678_Out_0, _Absolute_3E435394_Out_1, _Smoothstep_F2D936B8_Out_3);
                float3 _Preview_D7C48141_Out_1;
                Unity_Preview_float3(_RotateAboutAxis_6A2ED9B0_Out_3, _Preview_D7C48141_Out_1);
                float _Property_E8D50C61_Out_0 = Vector1_48832F08;
                float _Multiply_3E37F4A4_Out_2;
                Unity_Multiply_float(IN.TimeParameters.x, _Property_E8D50C61_Out_0, _Multiply_3E37F4A4_Out_2);
                float2 _TilingAndOffset_87133117_Out_3;
                Unity_TilingAndOffset_float((_Preview_D7C48141_Out_1.xy), float2 (1, 1), (_Multiply_3E37F4A4_Out_2.xx), _TilingAndOffset_87133117_Out_3);
                float _Property_20C66F3A_Out_0 = Vector1_218FD75;
                float _GradientNoise_DD3A9097_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_87133117_Out_3, _Property_20C66F3A_Out_0, _GradientNoise_DD3A9097_Out_2);
                float _Property_3911A428_Out_0 = Vector1_CEFA9380;
                float _Multiply_58919E8E_Out_2;
                Unity_Multiply_float(_GradientNoise_DD3A9097_Out_2, _Property_3911A428_Out_0, _Multiply_58919E8E_Out_2);
                float _Add_351D4C08_Out_2;
                Unity_Add_float(_Smoothstep_F2D936B8_Out_3, _Multiply_58919E8E_Out_2, _Add_351D4C08_Out_2);
                float _Add_42DEA188_Out_2;
                Unity_Add_float(_Property_3911A428_Out_0, 1, _Add_42DEA188_Out_2);
                float _Divide_652DC47F_Out_2;
                Unity_Divide_float(_Add_351D4C08_Out_2, _Add_42DEA188_Out_2, _Divide_652DC47F_Out_2);
                float4 _Lerp_D63CA845_Out_3;
                Unity_Lerp_float4(_Property_AD777D88_Out_0, _Property_337A1152_Out_0, (_Divide_652DC47F_Out_2.xxxx), _Lerp_D63CA845_Out_3);
                float _Property_F0B7CE35_Out_0 = Vector1_2CA6C852;
                float _FresnelEffect_E0BDE566_Out_3;
                Unity_FresnelEffect_float(IN.WorldSpaceNormal, IN.WorldSpaceViewDirection, _Property_F0B7CE35_Out_0, _FresnelEffect_E0BDE566_Out_3);
                float _Multiply_577CA5E4_Out_2;
                Unity_Multiply_float(_Divide_652DC47F_Out_2, _FresnelEffect_E0BDE566_Out_3, _Multiply_577CA5E4_Out_2);
                float _Property_42E18D94_Out_0 = Vector1_19BDB10F;
                float _Multiply_2F72B65E_Out_2;
                Unity_Multiply_float(_Multiply_577CA5E4_Out_2, _Property_42E18D94_Out_0, _Multiply_2F72B65E_Out_2);
                float _Preview_EE223B3_Out_1;
                Unity_Preview_float(_Multiply_2F72B65E_Out_2, _Preview_EE223B3_Out_1);
                float4 _Add_F30AB85E_Out_2;
                Unity_Add_float4(_Lerp_D63CA845_Out_3, (_Preview_EE223B3_Out_1.xxxx), _Add_F30AB85E_Out_2);
                float _SceneDepth_DD2F9709_Out_1;
                Unity_SceneDepth_Eye_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_DD2F9709_Out_1);
                float4 _ScreenPosition_9A47EDBC_Out_0 = IN.ScreenPosition;
                float _Split_272075D3_R_1 = _ScreenPosition_9A47EDBC_Out_0[0];
                float _Split_272075D3_G_2 = _ScreenPosition_9A47EDBC_Out_0[1];
                float _Split_272075D3_B_3 = _ScreenPosition_9A47EDBC_Out_0[2];
                float _Split_272075D3_A_4 = _ScreenPosition_9A47EDBC_Out_0[3];
                float _Subtract_C82D319_Out_2;
                Unity_Subtract_float(_Split_272075D3_A_4, 1, _Subtract_C82D319_Out_2);
                float _Subtract_BCAC126F_Out_2;
                Unity_Subtract_float(_SceneDepth_DD2F9709_Out_1, _Subtract_C82D319_Out_2, _Subtract_BCAC126F_Out_2);
                float _Property_2C14C873_Out_0 = Vector1_5208AC81;
                float _Divide_68737937_Out_2;
                Unity_Divide_float(_Subtract_BCAC126F_Out_2, _Property_2C14C873_Out_0, _Divide_68737937_Out_2);
                float _Saturate_4C1B9947_Out_1;
                Unity_Saturate_float(_Divide_68737937_Out_2, _Saturate_4C1B9947_Out_1);
                float _Smoothstep_532CE8F6_Out_3;
                Unity_Smoothstep_float(0, 1, _Saturate_4C1B9947_Out_1, _Smoothstep_532CE8F6_Out_3);
                surface.Color = (_Add_F30AB85E_Out_2.xyz);
                surface.Alpha = _Smoothstep_532CE8F6_Out_3;
                surface.AlphaClipThreshold = 0;
                return surface;
            }
        
            // --------------------------------------------------
            // Structs and Packing
        
            // Generated Type: Attributes
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };
        
            // Generated Type: Varyings
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS;
                float3 normalWS;
                float3 viewDirectionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Generated Type: PackedVaryings
            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                float3 interp00 : TEXCOORD0;
                float3 interp01 : TEXCOORD1;
                float3 interp02 : TEXCOORD2;
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Packed Type: Varyings
            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output = (PackedVaryings)0;
                output.positionCS = input.positionCS;
                output.interp00.xyz = input.positionWS;
                output.interp01.xyz = input.normalWS;
                output.interp02.xyz = input.viewDirectionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
            
            // Unpacked Type: Varyings
            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output = (Varyings)0;
                output.positionCS = input.positionCS;
                output.positionWS = input.interp00.xyz;
                output.normalWS = input.interp01.xyz;
                output.viewDirectionWS = input.interp02.xyz;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);
            
                output.ObjectSpaceNormal =           input.normalOS;
                output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
                output.ObjectSpaceTangent =          input.tangentOS;
                output.ObjectSpacePosition =         input.positionOS;
                output.WorldSpacePosition =          TransformObjectToWorld(input.positionOS);
                output.TimeParameters =              _TimeParameters.xyz;
            
                return output;
            }
            
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
            
            	// must use interpolated tangent, bitangent and normal before they are normalized in the pixel shader.
            	float3 unnormalizedNormalWS = input.normalWS;
                const float renormFactor = 1.0 / length(unnormalizedNormalWS);
            
            
                output.WorldSpaceNormal =            renormFactor*input.normalWS.xyz;		// we want a unit length Normal Vector node in shader graph
            
            
                output.WorldSpaceViewDirection =     input.viewDirectionWS; //TODO: by default normalized in HD, but not in universal
                output.WorldSpacePosition =          input.positionWS;
                output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
                output.TimeParameters =              _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
            #else
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            #endif
            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            
                return output;
            }
            
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
        
            ENDHLSL
        }
        
        Pass
        {
            Name "ShadowCaster"
            Tags 
            { 
                "LightMode" = "ShadowCaster"
            }
           
            // Render State
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            Cull Off
            ZTest LEqual
            ZWrite On
            // ColorMask: <None>
            
        
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            // Pragmas
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma multi_compile_instancing
        
            // Keywords
            #pragma shader_feature _ _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
            // GraphKeywords: <None>
            
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_POSITION_WS 
            #define FEATURES_GRAPH_VERTEX
            #pragma multi_compile_instancing
            #define SHADERPASS_SHADOWCASTER
            #define REQUIRE_DEPTH_TEXTURE
            
        
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float4 Vector4_BCD8D570;
            float Vector1_1C365E49;
            float Vector1_412672A0;
            float Vector1_C4AC984B;
            float4 Vector4_B5D7B794;
            float4 Color_C335EDAA;
            float4 Color_4CF0EDD;
            float Vector1_703B551F;
            float Vector1_A99D943C;
            float Vector1_CEA3FF9C;
            float Vector1_218FD75;
            float Vector1_48832F08;
            float Vector1_CEFA9380;
            float Vector1_198CECC0;
            float Vector1_2CA6C852;
            float Vector1_19BDB10F;
            float Vector1_5208AC81;
            CBUFFER_END
        
            // Graph Functions
            
            void Unity_Distance_float3(float3 A, float3 B, out float Out)
            {
                Out = distance(A, B);
            }
            
            void Unity_Divide_float(float A, float B, out float Out)
            {
                Out = A / B;
            }
            
            void Unity_Power_float(float A, float B, out float Out)
            {
                Out = pow(A, B);
            }
            
            void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
            {
                Out = A * B;
            }
            
            void Unity_Rotate_About_Axis_Degrees_float(float3 In, float3 Axis, float Rotation, out float3 Out)
            {
                Rotation = radians(Rotation);
            
                float s = sin(Rotation);
                float c = cos(Rotation);
                float one_minus_c = 1.0 - c;
                
                Axis = normalize(Axis);
            
                float3x3 rot_mat = { one_minus_c * Axis.x * Axis.x + c,            one_minus_c * Axis.x * Axis.y - Axis.z * s,     one_minus_c * Axis.z * Axis.x + Axis.y * s,
                                          one_minus_c * Axis.x * Axis.y + Axis.z * s,   one_minus_c * Axis.y * Axis.y + c,              one_minus_c * Axis.y * Axis.z - Axis.x * s,
                                          one_minus_c * Axis.z * Axis.x - Axis.y * s,   one_minus_c * Axis.y * Axis.z + Axis.x * s,     one_minus_c * Axis.z * Axis.z + c
                                        };
            
                Out = mul(rot_mat,  In);
            }
            
            void Unity_Preview_float3(float3 In, out float3 Out)
            {
                Out = In;
            }
            
            void Unity_Multiply_float(float A, float B, out float Out)
            {
                Out = A * B;
            }
            
            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }
            
            
            float2 Unity_GradientNoise_Dir_float(float2 p)
            {
                // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
                p = p % 289;
                float x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }
            
            void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
            { 
                float2 p = UV * Scale;
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
                float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
            }
            
            void Unity_Add_float(float A, float B, out float Out)
            {
                Out = A + B;
            }
            
            void Unity_Saturate_float(float In, out float Out)
            {
                Out = saturate(In);
            }
            
            void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
            {
                RGBA = float4(R, G, B, A);
                RGB = float3(R, G, B);
                RG = float2(R, G);
            }
            
            void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
            {
                Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
            }
            
            void Unity_Absolute_float(float In, out float Out)
            {
                Out = abs(In);
            }
            
            void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
            {
                Out = smoothstep(Edge1, Edge2, In);
            }
            
            void Unity_Add_float3(float3 A, float3 B, out float3 Out)
            {
                Out = A + B;
            }
            
            void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
            {
                Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
            }
            
            void Unity_Subtract_float(float A, float B, out float Out)
            {
                Out = A - B;
            }
        
            // Graph Vertex
            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 WorldSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
                float3 WorldSpacePosition;
                float3 TimeParameters;
            };
            
            struct VertexDescription
            {
                float3 VertexPosition;
                float3 VertexNormal;
                float3 VertexTangent;
            };
            
            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                float _Distance_8E5A0BF8_Out_2;
                Unity_Distance_float3(SHADERGRAPH_OBJECT_POSITION, IN.WorldSpacePosition, _Distance_8E5A0BF8_Out_2);
                float _Property_9FFB8E45_Out_0 = Vector1_198CECC0;
                float _Divide_3CA900CE_Out_2;
                Unity_Divide_float(_Distance_8E5A0BF8_Out_2, _Property_9FFB8E45_Out_0, _Divide_3CA900CE_Out_2);
                float _Power_20D4C9FC_Out_2;
                Unity_Power_float(_Divide_3CA900CE_Out_2, 3, _Power_20D4C9FC_Out_2);
                float3 _Multiply_EC7D9BE5_Out_2;
                Unity_Multiply_float(IN.WorldSpaceNormal, (_Power_20D4C9FC_Out_2.xxx), _Multiply_EC7D9BE5_Out_2);
                float _Property_159635A8_Out_0 = Vector1_703B551F;
                float _Property_F7E5C678_Out_0 = Vector1_A99D943C;
                float4 _Property_AA65CC00_Out_0 = Vector4_BCD8D570;
                float _Split_F7FC9C8E_R_1 = _Property_AA65CC00_Out_0[0];
                float _Split_F7FC9C8E_G_2 = _Property_AA65CC00_Out_0[1];
                float _Split_F7FC9C8E_B_3 = _Property_AA65CC00_Out_0[2];
                float _Split_F7FC9C8E_A_4 = _Property_AA65CC00_Out_0[3];
                float3 _RotateAboutAxis_6A2ED9B0_Out_3;
                Unity_Rotate_About_Axis_Degrees_float(IN.WorldSpacePosition, (_Property_AA65CC00_Out_0.xyz), _Split_F7FC9C8E_A_4, _RotateAboutAxis_6A2ED9B0_Out_3);
                float3 _Preview_5A3F1FC8_Out_1;
                Unity_Preview_float3(_RotateAboutAxis_6A2ED9B0_Out_3, _Preview_5A3F1FC8_Out_1);
                float _Property_2C5852D1_Out_0 = Vector1_412672A0;
                float _Multiply_5E3E3E8F_Out_2;
                Unity_Multiply_float(IN.TimeParameters.x, _Property_2C5852D1_Out_0, _Multiply_5E3E3E8F_Out_2);
                float2 _TilingAndOffset_DC089697_Out_3;
                Unity_TilingAndOffset_float((_Preview_5A3F1FC8_Out_1.xy), float2 (1, 1), (_Multiply_5E3E3E8F_Out_2.xx), _TilingAndOffset_DC089697_Out_3);
                float _Property_3B2E35FA_Out_0 = Vector1_1C365E49;
                float _GradientNoise_111A2408_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_DC089697_Out_3, _Property_3B2E35FA_Out_0, _GradientNoise_111A2408_Out_2);
                float3 _Preview_88ECF05E_Out_1;
                Unity_Preview_float3(_RotateAboutAxis_6A2ED9B0_Out_3, _Preview_88ECF05E_Out_1);
                float2 _TilingAndOffset_495B1598_Out_3;
                Unity_TilingAndOffset_float((_Preview_88ECF05E_Out_1.xy), float2 (1, 1), float2 (0, 0), _TilingAndOffset_495B1598_Out_3);
                float _GradientNoise_DC945C06_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_495B1598_Out_3, _Property_3B2E35FA_Out_0, _GradientNoise_DC945C06_Out_2);
                float _Add_E5060FA0_Out_2;
                Unity_Add_float(_GradientNoise_111A2408_Out_2, _GradientNoise_DC945C06_Out_2, _Add_E5060FA0_Out_2);
                float _Divide_6F6F9F0C_Out_2;
                Unity_Divide_float(_Add_E5060FA0_Out_2, 2, _Divide_6F6F9F0C_Out_2);
                float _Saturate_606F110C_Out_1;
                Unity_Saturate_float(_Divide_6F6F9F0C_Out_2, _Saturate_606F110C_Out_1);
                float _Property_63E9F3C3_Out_0 = Vector1_CEA3FF9C;
                float _Power_83B4F216_Out_2;
                Unity_Power_float(_Saturate_606F110C_Out_1, _Property_63E9F3C3_Out_0, _Power_83B4F216_Out_2);
                float4 _Property_4C7FF516_Out_0 = Vector4_B5D7B794;
                float _Split_5274083C_R_1 = _Property_4C7FF516_Out_0[0];
                float _Split_5274083C_G_2 = _Property_4C7FF516_Out_0[1];
                float _Split_5274083C_B_3 = _Property_4C7FF516_Out_0[2];
                float _Split_5274083C_A_4 = _Property_4C7FF516_Out_0[3];
                float4 _Combine_1EEC015E_RGBA_4;
                float3 _Combine_1EEC015E_RGB_5;
                float2 _Combine_1EEC015E_RG_6;
                Unity_Combine_float(_Split_5274083C_R_1, _Split_5274083C_G_2, 0, 0, _Combine_1EEC015E_RGBA_4, _Combine_1EEC015E_RGB_5, _Combine_1EEC015E_RG_6);
                float _Split_5355AC72_R_1 = _Property_4C7FF516_Out_0[0];
                float _Split_5355AC72_G_2 = _Property_4C7FF516_Out_0[1];
                float _Split_5355AC72_B_3 = _Property_4C7FF516_Out_0[2];
                float _Split_5355AC72_A_4 = _Property_4C7FF516_Out_0[3];
                float4 _Combine_C84FFC89_RGBA_4;
                float3 _Combine_C84FFC89_RGB_5;
                float2 _Combine_C84FFC89_RG_6;
                Unity_Combine_float(_Split_5355AC72_B_3, _Split_5355AC72_A_4, 0, 0, _Combine_C84FFC89_RGBA_4, _Combine_C84FFC89_RGB_5, _Combine_C84FFC89_RG_6);
                float _Remap_348B5DED_Out_3;
                Unity_Remap_float(_Power_83B4F216_Out_2, _Combine_1EEC015E_RG_6, _Combine_C84FFC89_RG_6, _Remap_348B5DED_Out_3);
                float _Absolute_3E435394_Out_1;
                Unity_Absolute_float(_Remap_348B5DED_Out_3, _Absolute_3E435394_Out_1);
                float _Smoothstep_F2D936B8_Out_3;
                Unity_Smoothstep_float(_Property_159635A8_Out_0, _Property_F7E5C678_Out_0, _Absolute_3E435394_Out_1, _Smoothstep_F2D936B8_Out_3);
                float3 _Preview_D7C48141_Out_1;
                Unity_Preview_float3(_RotateAboutAxis_6A2ED9B0_Out_3, _Preview_D7C48141_Out_1);
                float _Property_E8D50C61_Out_0 = Vector1_48832F08;
                float _Multiply_3E37F4A4_Out_2;
                Unity_Multiply_float(IN.TimeParameters.x, _Property_E8D50C61_Out_0, _Multiply_3E37F4A4_Out_2);
                float2 _TilingAndOffset_87133117_Out_3;
                Unity_TilingAndOffset_float((_Preview_D7C48141_Out_1.xy), float2 (1, 1), (_Multiply_3E37F4A4_Out_2.xx), _TilingAndOffset_87133117_Out_3);
                float _Property_20C66F3A_Out_0 = Vector1_218FD75;
                float _GradientNoise_DD3A9097_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_87133117_Out_3, _Property_20C66F3A_Out_0, _GradientNoise_DD3A9097_Out_2);
                float _Property_3911A428_Out_0 = Vector1_CEFA9380;
                float _Multiply_58919E8E_Out_2;
                Unity_Multiply_float(_GradientNoise_DD3A9097_Out_2, _Property_3911A428_Out_0, _Multiply_58919E8E_Out_2);
                float _Add_351D4C08_Out_2;
                Unity_Add_float(_Smoothstep_F2D936B8_Out_3, _Multiply_58919E8E_Out_2, _Add_351D4C08_Out_2);
                float _Add_42DEA188_Out_2;
                Unity_Add_float(_Property_3911A428_Out_0, 1, _Add_42DEA188_Out_2);
                float _Divide_652DC47F_Out_2;
                Unity_Divide_float(_Add_351D4C08_Out_2, _Add_42DEA188_Out_2, _Divide_652DC47F_Out_2);
                float3 _Multiply_F8E682E_Out_2;
                Unity_Multiply_float(IN.ObjectSpaceNormal, (_Divide_652DC47F_Out_2.xxx), _Multiply_F8E682E_Out_2);
                float _Property_38F4B400_Out_0 = Vector1_C4AC984B;
                float3 _Multiply_DA1124D6_Out_2;
                Unity_Multiply_float(_Multiply_F8E682E_Out_2, (_Property_38F4B400_Out_0.xxx), _Multiply_DA1124D6_Out_2);
                float3 _Add_C5A1A8CF_Out_2;
                Unity_Add_float3(IN.ObjectSpacePosition, _Multiply_DA1124D6_Out_2, _Add_C5A1A8CF_Out_2);
                float3 _Add_692861E2_Out_2;
                Unity_Add_float3(_Multiply_EC7D9BE5_Out_2, _Add_C5A1A8CF_Out_2, _Add_692861E2_Out_2);
                float3 _Preview_34DB2B41_Out_1;
                Unity_Preview_float3(_Add_692861E2_Out_2, _Preview_34DB2B41_Out_1);
                description.VertexPosition = _Preview_34DB2B41_Out_1;
                description.VertexNormal = IN.ObjectSpaceNormal;
                description.VertexTangent = IN.ObjectSpaceTangent;
                return description;
            }
            
            // Graph Pixel
            struct SurfaceDescriptionInputs
            {
                float3 WorldSpacePosition;
                float4 ScreenPosition;
            };
            
            struct SurfaceDescription
            {
                float Alpha;
                float AlphaClipThreshold;
            };
            
            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float _SceneDepth_DD2F9709_Out_1;
                Unity_SceneDepth_Eye_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_DD2F9709_Out_1);
                float4 _ScreenPosition_9A47EDBC_Out_0 = IN.ScreenPosition;
                float _Split_272075D3_R_1 = _ScreenPosition_9A47EDBC_Out_0[0];
                float _Split_272075D3_G_2 = _ScreenPosition_9A47EDBC_Out_0[1];
                float _Split_272075D3_B_3 = _ScreenPosition_9A47EDBC_Out_0[2];
                float _Split_272075D3_A_4 = _ScreenPosition_9A47EDBC_Out_0[3];
                float _Subtract_C82D319_Out_2;
                Unity_Subtract_float(_Split_272075D3_A_4, 1, _Subtract_C82D319_Out_2);
                float _Subtract_BCAC126F_Out_2;
                Unity_Subtract_float(_SceneDepth_DD2F9709_Out_1, _Subtract_C82D319_Out_2, _Subtract_BCAC126F_Out_2);
                float _Property_2C14C873_Out_0 = Vector1_5208AC81;
                float _Divide_68737937_Out_2;
                Unity_Divide_float(_Subtract_BCAC126F_Out_2, _Property_2C14C873_Out_0, _Divide_68737937_Out_2);
                float _Saturate_4C1B9947_Out_1;
                Unity_Saturate_float(_Divide_68737937_Out_2, _Saturate_4C1B9947_Out_1);
                float _Smoothstep_532CE8F6_Out_3;
                Unity_Smoothstep_float(0, 1, _Saturate_4C1B9947_Out_1, _Smoothstep_532CE8F6_Out_3);
                surface.Alpha = _Smoothstep_532CE8F6_Out_3;
                surface.AlphaClipThreshold = 0;
                return surface;
            }
        
            // --------------------------------------------------
            // Structs and Packing
        
            // Generated Type: Attributes
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };
        
            // Generated Type: Varyings
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Generated Type: PackedVaryings
            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                float3 interp00 : TEXCOORD0;
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Packed Type: Varyings
            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output = (PackedVaryings)0;
                output.positionCS = input.positionCS;
                output.interp00.xyz = input.positionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
            
            // Unpacked Type: Varyings
            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output = (Varyings)0;
                output.positionCS = input.positionCS;
                output.positionWS = input.interp00.xyz;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);
            
                output.ObjectSpaceNormal =           input.normalOS;
                output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
                output.ObjectSpaceTangent =          input.tangentOS;
                output.ObjectSpacePosition =         input.positionOS;
                output.WorldSpacePosition =          TransformObjectToWorld(input.positionOS);
                output.TimeParameters =              _TimeParameters.xyz;
            
                return output;
            }
            
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
            
            
            
            
            
                output.WorldSpacePosition =          input.positionWS;
                output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
            #else
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            #endif
            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            
                return output;
            }
            
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShadowCasterPass.hlsl"
        
            ENDHLSL
        }
        
        Pass
        {
            Name "DepthOnly"
            Tags 
            { 
                "LightMode" = "DepthOnly"
            }
           
            // Render State
            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            Cull Off
            ZTest LEqual
            ZWrite On
            ColorMask 0
            
        
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
        
            // Debug
            // <None>
        
            // --------------------------------------------------
            // Pass
        
            // Pragmas
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma multi_compile_instancing
        
            // Keywords
            // PassKeywords: <None>
            // GraphKeywords: <None>
            
            // Defines
            #define _SURFACE_TYPE_TRANSPARENT 1
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_POSITION_WS 
            #define FEATURES_GRAPH_VERTEX
            #pragma multi_compile_instancing
            #define SHADERPASS_DEPTHONLY
            #define REQUIRE_DEPTH_TEXTURE
            
        
            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
        
            // --------------------------------------------------
            // Graph
        
            // Graph Properties
            CBUFFER_START(UnityPerMaterial)
            float4 Vector4_BCD8D570;
            float Vector1_1C365E49;
            float Vector1_412672A0;
            float Vector1_C4AC984B;
            float4 Vector4_B5D7B794;
            float4 Color_C335EDAA;
            float4 Color_4CF0EDD;
            float Vector1_703B551F;
            float Vector1_A99D943C;
            float Vector1_CEA3FF9C;
            float Vector1_218FD75;
            float Vector1_48832F08;
            float Vector1_CEFA9380;
            float Vector1_198CECC0;
            float Vector1_2CA6C852;
            float Vector1_19BDB10F;
            float Vector1_5208AC81;
            CBUFFER_END
        
            // Graph Functions
            
            void Unity_Distance_float3(float3 A, float3 B, out float Out)
            {
                Out = distance(A, B);
            }
            
            void Unity_Divide_float(float A, float B, out float Out)
            {
                Out = A / B;
            }
            
            void Unity_Power_float(float A, float B, out float Out)
            {
                Out = pow(A, B);
            }
            
            void Unity_Multiply_float(float3 A, float3 B, out float3 Out)
            {
                Out = A * B;
            }
            
            void Unity_Rotate_About_Axis_Degrees_float(float3 In, float3 Axis, float Rotation, out float3 Out)
            {
                Rotation = radians(Rotation);
            
                float s = sin(Rotation);
                float c = cos(Rotation);
                float one_minus_c = 1.0 - c;
                
                Axis = normalize(Axis);
            
                float3x3 rot_mat = { one_minus_c * Axis.x * Axis.x + c,            one_minus_c * Axis.x * Axis.y - Axis.z * s,     one_minus_c * Axis.z * Axis.x + Axis.y * s,
                                          one_minus_c * Axis.x * Axis.y + Axis.z * s,   one_minus_c * Axis.y * Axis.y + c,              one_minus_c * Axis.y * Axis.z - Axis.x * s,
                                          one_minus_c * Axis.z * Axis.x - Axis.y * s,   one_minus_c * Axis.y * Axis.z + Axis.x * s,     one_minus_c * Axis.z * Axis.z + c
                                        };
            
                Out = mul(rot_mat,  In);
            }
            
            void Unity_Preview_float3(float3 In, out float3 Out)
            {
                Out = In;
            }
            
            void Unity_Multiply_float(float A, float B, out float Out)
            {
                Out = A * B;
            }
            
            void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
            {
                Out = UV * Tiling + Offset;
            }
            
            
            float2 Unity_GradientNoise_Dir_float(float2 p)
            {
                // Permutation and hashing used in webgl-nosie goo.gl/pX7HtC
                p = p % 289;
                float x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }
            
            void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
            { 
                float2 p = UV * Scale;
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(Unity_GradientNoise_Dir_float(ip), fp);
                float d01 = dot(Unity_GradientNoise_Dir_float(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(Unity_GradientNoise_Dir_float(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                Out = lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x) + 0.5;
            }
            
            void Unity_Add_float(float A, float B, out float Out)
            {
                Out = A + B;
            }
            
            void Unity_Saturate_float(float In, out float Out)
            {
                Out = saturate(In);
            }
            
            void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
            {
                RGBA = float4(R, G, B, A);
                RGB = float3(R, G, B);
                RG = float2(R, G);
            }
            
            void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
            {
                Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
            }
            
            void Unity_Absolute_float(float In, out float Out)
            {
                Out = abs(In);
            }
            
            void Unity_Smoothstep_float(float Edge1, float Edge2, float In, out float Out)
            {
                Out = smoothstep(Edge1, Edge2, In);
            }
            
            void Unity_Add_float3(float3 A, float3 B, out float3 Out)
            {
                Out = A + B;
            }
            
            void Unity_SceneDepth_Eye_float(float4 UV, out float Out)
            {
                Out = LinearEyeDepth(SHADERGRAPH_SAMPLE_SCENE_DEPTH(UV.xy), _ZBufferParams);
            }
            
            void Unity_Subtract_float(float A, float B, out float Out)
            {
                Out = A - B;
            }
        
            // Graph Vertex
            struct VertexDescriptionInputs
            {
                float3 ObjectSpaceNormal;
                float3 WorldSpaceNormal;
                float3 ObjectSpaceTangent;
                float3 ObjectSpacePosition;
                float3 WorldSpacePosition;
                float3 TimeParameters;
            };
            
            struct VertexDescription
            {
                float3 VertexPosition;
                float3 VertexNormal;
                float3 VertexTangent;
            };
            
            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                float _Distance_8E5A0BF8_Out_2;
                Unity_Distance_float3(SHADERGRAPH_OBJECT_POSITION, IN.WorldSpacePosition, _Distance_8E5A0BF8_Out_2);
                float _Property_9FFB8E45_Out_0 = Vector1_198CECC0;
                float _Divide_3CA900CE_Out_2;
                Unity_Divide_float(_Distance_8E5A0BF8_Out_2, _Property_9FFB8E45_Out_0, _Divide_3CA900CE_Out_2);
                float _Power_20D4C9FC_Out_2;
                Unity_Power_float(_Divide_3CA900CE_Out_2, 3, _Power_20D4C9FC_Out_2);
                float3 _Multiply_EC7D9BE5_Out_2;
                Unity_Multiply_float(IN.WorldSpaceNormal, (_Power_20D4C9FC_Out_2.xxx), _Multiply_EC7D9BE5_Out_2);
                float _Property_159635A8_Out_0 = Vector1_703B551F;
                float _Property_F7E5C678_Out_0 = Vector1_A99D943C;
                float4 _Property_AA65CC00_Out_0 = Vector4_BCD8D570;
                float _Split_F7FC9C8E_R_1 = _Property_AA65CC00_Out_0[0];
                float _Split_F7FC9C8E_G_2 = _Property_AA65CC00_Out_0[1];
                float _Split_F7FC9C8E_B_3 = _Property_AA65CC00_Out_0[2];
                float _Split_F7FC9C8E_A_4 = _Property_AA65CC00_Out_0[3];
                float3 _RotateAboutAxis_6A2ED9B0_Out_3;
                Unity_Rotate_About_Axis_Degrees_float(IN.WorldSpacePosition, (_Property_AA65CC00_Out_0.xyz), _Split_F7FC9C8E_A_4, _RotateAboutAxis_6A2ED9B0_Out_3);
                float3 _Preview_5A3F1FC8_Out_1;
                Unity_Preview_float3(_RotateAboutAxis_6A2ED9B0_Out_3, _Preview_5A3F1FC8_Out_1);
                float _Property_2C5852D1_Out_0 = Vector1_412672A0;
                float _Multiply_5E3E3E8F_Out_2;
                Unity_Multiply_float(IN.TimeParameters.x, _Property_2C5852D1_Out_0, _Multiply_5E3E3E8F_Out_2);
                float2 _TilingAndOffset_DC089697_Out_3;
                Unity_TilingAndOffset_float((_Preview_5A3F1FC8_Out_1.xy), float2 (1, 1), (_Multiply_5E3E3E8F_Out_2.xx), _TilingAndOffset_DC089697_Out_3);
                float _Property_3B2E35FA_Out_0 = Vector1_1C365E49;
                float _GradientNoise_111A2408_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_DC089697_Out_3, _Property_3B2E35FA_Out_0, _GradientNoise_111A2408_Out_2);
                float3 _Preview_88ECF05E_Out_1;
                Unity_Preview_float3(_RotateAboutAxis_6A2ED9B0_Out_3, _Preview_88ECF05E_Out_1);
                float2 _TilingAndOffset_495B1598_Out_3;
                Unity_TilingAndOffset_float((_Preview_88ECF05E_Out_1.xy), float2 (1, 1), float2 (0, 0), _TilingAndOffset_495B1598_Out_3);
                float _GradientNoise_DC945C06_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_495B1598_Out_3, _Property_3B2E35FA_Out_0, _GradientNoise_DC945C06_Out_2);
                float _Add_E5060FA0_Out_2;
                Unity_Add_float(_GradientNoise_111A2408_Out_2, _GradientNoise_DC945C06_Out_2, _Add_E5060FA0_Out_2);
                float _Divide_6F6F9F0C_Out_2;
                Unity_Divide_float(_Add_E5060FA0_Out_2, 2, _Divide_6F6F9F0C_Out_2);
                float _Saturate_606F110C_Out_1;
                Unity_Saturate_float(_Divide_6F6F9F0C_Out_2, _Saturate_606F110C_Out_1);
                float _Property_63E9F3C3_Out_0 = Vector1_CEA3FF9C;
                float _Power_83B4F216_Out_2;
                Unity_Power_float(_Saturate_606F110C_Out_1, _Property_63E9F3C3_Out_0, _Power_83B4F216_Out_2);
                float4 _Property_4C7FF516_Out_0 = Vector4_B5D7B794;
                float _Split_5274083C_R_1 = _Property_4C7FF516_Out_0[0];
                float _Split_5274083C_G_2 = _Property_4C7FF516_Out_0[1];
                float _Split_5274083C_B_3 = _Property_4C7FF516_Out_0[2];
                float _Split_5274083C_A_4 = _Property_4C7FF516_Out_0[3];
                float4 _Combine_1EEC015E_RGBA_4;
                float3 _Combine_1EEC015E_RGB_5;
                float2 _Combine_1EEC015E_RG_6;
                Unity_Combine_float(_Split_5274083C_R_1, _Split_5274083C_G_2, 0, 0, _Combine_1EEC015E_RGBA_4, _Combine_1EEC015E_RGB_5, _Combine_1EEC015E_RG_6);
                float _Split_5355AC72_R_1 = _Property_4C7FF516_Out_0[0];
                float _Split_5355AC72_G_2 = _Property_4C7FF516_Out_0[1];
                float _Split_5355AC72_B_3 = _Property_4C7FF516_Out_0[2];
                float _Split_5355AC72_A_4 = _Property_4C7FF516_Out_0[3];
                float4 _Combine_C84FFC89_RGBA_4;
                float3 _Combine_C84FFC89_RGB_5;
                float2 _Combine_C84FFC89_RG_6;
                Unity_Combine_float(_Split_5355AC72_B_3, _Split_5355AC72_A_4, 0, 0, _Combine_C84FFC89_RGBA_4, _Combine_C84FFC89_RGB_5, _Combine_C84FFC89_RG_6);
                float _Remap_348B5DED_Out_3;
                Unity_Remap_float(_Power_83B4F216_Out_2, _Combine_1EEC015E_RG_6, _Combine_C84FFC89_RG_6, _Remap_348B5DED_Out_3);
                float _Absolute_3E435394_Out_1;
                Unity_Absolute_float(_Remap_348B5DED_Out_3, _Absolute_3E435394_Out_1);
                float _Smoothstep_F2D936B8_Out_3;
                Unity_Smoothstep_float(_Property_159635A8_Out_0, _Property_F7E5C678_Out_0, _Absolute_3E435394_Out_1, _Smoothstep_F2D936B8_Out_3);
                float3 _Preview_D7C48141_Out_1;
                Unity_Preview_float3(_RotateAboutAxis_6A2ED9B0_Out_3, _Preview_D7C48141_Out_1);
                float _Property_E8D50C61_Out_0 = Vector1_48832F08;
                float _Multiply_3E37F4A4_Out_2;
                Unity_Multiply_float(IN.TimeParameters.x, _Property_E8D50C61_Out_0, _Multiply_3E37F4A4_Out_2);
                float2 _TilingAndOffset_87133117_Out_3;
                Unity_TilingAndOffset_float((_Preview_D7C48141_Out_1.xy), float2 (1, 1), (_Multiply_3E37F4A4_Out_2.xx), _TilingAndOffset_87133117_Out_3);
                float _Property_20C66F3A_Out_0 = Vector1_218FD75;
                float _GradientNoise_DD3A9097_Out_2;
                Unity_GradientNoise_float(_TilingAndOffset_87133117_Out_3, _Property_20C66F3A_Out_0, _GradientNoise_DD3A9097_Out_2);
                float _Property_3911A428_Out_0 = Vector1_CEFA9380;
                float _Multiply_58919E8E_Out_2;
                Unity_Multiply_float(_GradientNoise_DD3A9097_Out_2, _Property_3911A428_Out_0, _Multiply_58919E8E_Out_2);
                float _Add_351D4C08_Out_2;
                Unity_Add_float(_Smoothstep_F2D936B8_Out_3, _Multiply_58919E8E_Out_2, _Add_351D4C08_Out_2);
                float _Add_42DEA188_Out_2;
                Unity_Add_float(_Property_3911A428_Out_0, 1, _Add_42DEA188_Out_2);
                float _Divide_652DC47F_Out_2;
                Unity_Divide_float(_Add_351D4C08_Out_2, _Add_42DEA188_Out_2, _Divide_652DC47F_Out_2);
                float3 _Multiply_F8E682E_Out_2;
                Unity_Multiply_float(IN.ObjectSpaceNormal, (_Divide_652DC47F_Out_2.xxx), _Multiply_F8E682E_Out_2);
                float _Property_38F4B400_Out_0 = Vector1_C4AC984B;
                float3 _Multiply_DA1124D6_Out_2;
                Unity_Multiply_float(_Multiply_F8E682E_Out_2, (_Property_38F4B400_Out_0.xxx), _Multiply_DA1124D6_Out_2);
                float3 _Add_C5A1A8CF_Out_2;
                Unity_Add_float3(IN.ObjectSpacePosition, _Multiply_DA1124D6_Out_2, _Add_C5A1A8CF_Out_2);
                float3 _Add_692861E2_Out_2;
                Unity_Add_float3(_Multiply_EC7D9BE5_Out_2, _Add_C5A1A8CF_Out_2, _Add_692861E2_Out_2);
                float3 _Preview_34DB2B41_Out_1;
                Unity_Preview_float3(_Add_692861E2_Out_2, _Preview_34DB2B41_Out_1);
                description.VertexPosition = _Preview_34DB2B41_Out_1;
                description.VertexNormal = IN.ObjectSpaceNormal;
                description.VertexTangent = IN.ObjectSpaceTangent;
                return description;
            }
            
            // Graph Pixel
            struct SurfaceDescriptionInputs
            {
                float3 WorldSpacePosition;
                float4 ScreenPosition;
            };
            
            struct SurfaceDescription
            {
                float Alpha;
                float AlphaClipThreshold;
            };
            
            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                float _SceneDepth_DD2F9709_Out_1;
                Unity_SceneDepth_Eye_float(float4(IN.ScreenPosition.xy / IN.ScreenPosition.w, 0, 0), _SceneDepth_DD2F9709_Out_1);
                float4 _ScreenPosition_9A47EDBC_Out_0 = IN.ScreenPosition;
                float _Split_272075D3_R_1 = _ScreenPosition_9A47EDBC_Out_0[0];
                float _Split_272075D3_G_2 = _ScreenPosition_9A47EDBC_Out_0[1];
                float _Split_272075D3_B_3 = _ScreenPosition_9A47EDBC_Out_0[2];
                float _Split_272075D3_A_4 = _ScreenPosition_9A47EDBC_Out_0[3];
                float _Subtract_C82D319_Out_2;
                Unity_Subtract_float(_Split_272075D3_A_4, 1, _Subtract_C82D319_Out_2);
                float _Subtract_BCAC126F_Out_2;
                Unity_Subtract_float(_SceneDepth_DD2F9709_Out_1, _Subtract_C82D319_Out_2, _Subtract_BCAC126F_Out_2);
                float _Property_2C14C873_Out_0 = Vector1_5208AC81;
                float _Divide_68737937_Out_2;
                Unity_Divide_float(_Subtract_BCAC126F_Out_2, _Property_2C14C873_Out_0, _Divide_68737937_Out_2);
                float _Saturate_4C1B9947_Out_1;
                Unity_Saturate_float(_Divide_68737937_Out_2, _Saturate_4C1B9947_Out_1);
                float _Smoothstep_532CE8F6_Out_3;
                Unity_Smoothstep_float(0, 1, _Saturate_4C1B9947_Out_1, _Smoothstep_532CE8F6_Out_3);
                surface.Alpha = _Smoothstep_532CE8F6_Out_3;
                surface.AlphaClipThreshold = 0;
                return surface;
            }
        
            // --------------------------------------------------
            // Structs and Packing
        
            // Generated Type: Attributes
            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : INSTANCEID_SEMANTIC;
                #endif
            };
        
            // Generated Type: Varyings
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Generated Type: PackedVaryings
            struct PackedVaryings
            {
                float4 positionCS : SV_POSITION;
                #if UNITY_ANY_INSTANCING_ENABLED
                uint instanceID : CUSTOM_INSTANCE_ID;
                #endif
                float3 interp00 : TEXCOORD0;
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
                #endif
            };
            
            // Packed Type: Varyings
            PackedVaryings PackVaryings(Varyings input)
            {
                PackedVaryings output = (PackedVaryings)0;
                output.positionCS = input.positionCS;
                output.interp00.xyz = input.positionWS;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
            
            // Unpacked Type: Varyings
            Varyings UnpackVaryings(PackedVaryings input)
            {
                Varyings output = (Varyings)0;
                output.positionCS = input.positionCS;
                output.positionWS = input.interp00.xyz;
                #if UNITY_ANY_INSTANCING_ENABLED
                output.instanceID = input.instanceID;
                #endif
                #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
                output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
                #endif
                #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
                output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
                #endif
                #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
                output.cullFace = input.cullFace;
                #endif
                return output;
            }
        
            // --------------------------------------------------
            // Build Graph Inputs
        
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);
            
                output.ObjectSpaceNormal =           input.normalOS;
                output.WorldSpaceNormal =            TransformObjectToWorldNormal(input.normalOS);
                output.ObjectSpaceTangent =          input.tangentOS;
                output.ObjectSpacePosition =         input.positionOS;
                output.WorldSpacePosition =          TransformObjectToWorld(input.positionOS);
                output.TimeParameters =              _TimeParameters.xyz;
            
                return output;
            }
            
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
            
            
            
            
            
                output.WorldSpacePosition =          input.positionWS;
                output.ScreenPosition =              ComputeScreenPos(TransformWorldToHClip(input.positionWS), _ProjectionParams.x);
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
            #else
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            #endif
            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            
                return output;
            }
            
        
            // --------------------------------------------------
            // Main
        
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"
        
            ENDHLSL
        }
        
    }
    FallBack "Hidden/Shader Graph/FallbackError"
}
