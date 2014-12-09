// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:1,bsrc:3,bdst:7,culm:0,dpts:2,wrdp:False,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32354,y:32708|emission-26-OUT,alpha-20-OUT;n:type:ShaderForge.SFN_Cubemap,id:2,x:34245,y:32694,ptlb:Environoment,ptin:_Environoment,cube:a596436b21c6d484bb9b3b6385e3e666,pvfc:0|DIR-3-OUT;n:type:ShaderForge.SFN_ViewReflectionVector,id:3,x:34435,y:32694;n:type:ShaderForge.SFN_Color,id:4,x:33444,y:32557,ptlb:MainColor,ptin:_MainColor,glob:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Fresnel,id:8,x:33740,y:33032;n:type:ShaderForge.SFN_Multiply,id:11,x:33083,y:32554|A-14-OUT,B-4-RGB;n:type:ShaderForge.SFN_LightVector,id:12,x:33444,y:32237;n:type:ShaderForge.SFN_NormalVector,id:13,x:33444,y:32365,pt:False;n:type:ShaderForge.SFN_Dot,id:14,x:33267,y:32352,dt:0|A-12-OUT,B-13-OUT;n:type:ShaderForge.SFN_ValueProperty,id:15,x:33595,y:33558,ptlb:Visibility,ptin:_Visibility,glob:False,v1:0.5;n:type:ShaderForge.SFN_Relay,id:16,x:33557,y:33066,cmnt:reflection|IN-8-OUT;n:type:ShaderForge.SFN_OneMinus,id:17,x:33292,y:33316|IN-16-OUT;n:type:ShaderForge.SFN_OneMinus,id:18,x:33304,y:33518|IN-15-OUT;n:type:ShaderForge.SFN_Multiply,id:19,x:33083,y:33418|A-17-OUT,B-18-OUT;n:type:ShaderForge.SFN_OneMinus,id:20,x:32893,y:33301|IN-19-OUT;n:type:ShaderForge.SFN_Multiply,id:21,x:33142,y:32715|A-35-OUT,B-16-OUT;n:type:ShaderForge.SFN_OneMinus,id:22,x:33200,y:33056|IN-16-OUT;n:type:ShaderForge.SFN_Multiply,id:23,x:33006,y:33056|A-22-OUT,B-15-OUT;n:type:ShaderForge.SFN_Multiply,id:24,x:32948,y:32885|A-11-OUT,B-23-OUT;n:type:ShaderForge.SFN_Add,id:25,x:32742,y:32670|A-21-OUT,B-24-OUT;n:type:ShaderForge.SFN_Divide,id:26,x:32613,y:32786|A-25-OUT,B-20-OUT;n:type:ShaderForge.SFN_ValueProperty,id:27,x:34030,y:32967,ptlb:EnvBoostPower,ptin:_EnvBoostPower,glob:False,v1:10;n:type:ShaderForge.SFN_ValueProperty,id:28,x:33922,y:32624,ptlb:EnvBoostCoef,ptin:_EnvBoostCoef,glob:False,v1:3;n:type:ShaderForge.SFN_Vector3,id:33,x:34245,y:32860,v1:0.299,v2:0.587,v3:0.114;n:type:ShaderForge.SFN_Dot,id:34,x:34012,y:32770,dt:0|A-2-RGB,B-33-OUT;n:type:ShaderForge.SFN_Multiply,id:35,x:33385,y:32715|A-2-RGB,B-36-OUT;n:type:ShaderForge.SFN_Add,id:36,x:33495,y:32848|A-37-OUT,B-39-OUT;n:type:ShaderForge.SFN_Vector1,id:37,x:33658,y:32756,v1:1;n:type:ShaderForge.SFN_Power,id:38,x:33841,y:32863|VAL-34-OUT,EXP-27-OUT;n:type:ShaderForge.SFN_Multiply,id:39,x:33684,y:32848|A-28-OUT,B-38-OUT;proporder:2-4-15-27-28;pass:END;sub:END;*/

Shader "Custom/EnvironmentalMap" {
    Properties {
        _Environoment ("Environoment", Cube) = "_Skybox" {}
        _MainColor ("MainColor", Color) = (1,0,0,1)
        _Visibility ("Visibility", Float ) = 0.5
        _EnvBoostPower ("EnvBoostPower", Float ) = 10
        _EnvBoostCoef ("EnvBoostCoef", Float ) = 3
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        LOD 200
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform samplerCUBE _Environoment;
            uniform float4 _MainColor;
            uniform float _Visibility;
            uniform float _EnvBoostPower;
            uniform float _EnvBoostCoef;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float3 normalDirection =  i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
////// Lighting:
////// Emissive:
                float4 node_2 = texCUBE(_Environoment,viewReflectDirection);
                float node_16 = (1.0-max(0,dot(normalDirection, viewDirection))); // reflection
                float node_20 = (1.0 - ((1.0 - node_16)*(1.0 - _Visibility)));
                float3 emissive = ((((node_2.rgb*(1.0+(_EnvBoostCoef*pow(dot(node_2.rgb,float3(0.299,0.587,0.114)),_EnvBoostPower))))*node_16)+((dot(lightDirection,i.normalDir)*_MainColor.rgb)*((1.0 - node_16)*_Visibility)))/node_20);
                float3 finalColor = emissive;
/// Final Color:
                return fixed4(finalColor,node_20);
            }
            ENDCG
        }
        Pass {
            Name "ForwardAdd"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            ZWrite Off
            
            Fog { Color (0,0,0,0) }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #pragma multi_compile_fwdadd
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform samplerCUBE _Environoment;
            uniform float4 _MainColor;
            uniform float _Visibility;
            uniform float _EnvBoostPower;
            uniform float _EnvBoostCoef;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                LIGHTING_COORDS(2,3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
/////// Normals:
                float3 normalDirection =  i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
////// Lighting:
                float3 finalColor = 0;
                float node_16 = (1.0-max(0,dot(normalDirection, viewDirection))); // reflection
                float node_20 = (1.0 - ((1.0 - node_16)*(1.0 - _Visibility)));
/// Final Color:
                return fixed4(finalColor * node_20,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
