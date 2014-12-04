// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32521,y:32640|emission-14-OUT;n:type:ShaderForge.SFN_LightAttenuation,id:2,x:33973,y:33056;n:type:ShaderForge.SFN_Relay,id:3,x:33667,y:32826,cmnt:add tone curve?|IN-64-OUT;n:type:ShaderForge.SFN_ValueProperty,id:5,x:33503,y:33104,ptlb:Levels,ptin:_Levels,glob:False,v1:3;n:type:ShaderForge.SFN_Multiply,id:6,x:33278,y:32814|A-3-OUT,B-5-OUT;n:type:ShaderForge.SFN_Ceil,id:7,x:33120,y:32796|IN-6-OUT;n:type:ShaderForge.SFN_Divide,id:8,x:33205,y:33009|A-7-OUT,B-5-OUT;n:type:ShaderForge.SFN_LightVector,id:10,x:34657,y:32919;n:type:ShaderForge.SFN_NormalVector,id:11,x:34587,y:32735,pt:False;n:type:ShaderForge.SFN_Dot,id:12,x:34337,y:32645,dt:0|A-59-OUT,B-11-OUT;n:type:ShaderForge.SFN_Tex2d,id:13,x:33120,y:32621,ptlb:MainTex,ptin:_MainTex,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:14,x:32789,y:32750|A-13-RGB,B-20-OUT;n:type:ShaderForge.SFN_RemapRange,id:20,x:33017,y:33102,frmn:0,frmx:1,tomn:0.4,tomx:1|IN-8-OUT;n:type:ShaderForge.SFN_Clamp01,id:36,x:34007,y:32677|IN-74-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:56,x:34862,y:32340;n:type:ShaderForge.SFN_LightPosition,id:57,x:34862,y:32180;n:type:ShaderForge.SFN_Subtract,id:58,x:34666,y:32329|A-57-XYZ,B-56-XYZ;n:type:ShaderForge.SFN_Normalize,id:59,x:34504,y:32496|IN-58-OUT;n:type:ShaderForge.SFN_Sqrt,id:64,x:33825,y:32792|IN-36-OUT;n:type:ShaderForge.SFN_ValueProperty,id:73,x:34337,y:32865,ptlb:Bias,ptin:_Bias,glob:False,v1:0;n:type:ShaderForge.SFN_Add,id:74,x:34164,y:32677|A-12-OUT,B-73-OUT;proporder:5-13-73;pass:END;sub:END;*/

Shader "Custom/UnityChanCustom" {
    Properties {
        _Levels ("Levels", Float ) = 3
        _MainTex ("MainTex", 2D) = "white" {}
        _Bias ("Bias", Float ) = 0
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        LOD 200
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float _Levels;
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float _Bias;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.uv0 = v.texcoord0;
                o.normalDir = mul(float4(v.normal,0), _World2Object).xyz;
                o.posWorld = mul(_Object2World, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Normals:
                float3 normalDirection =  i.normalDir;
////// Lighting:
////// Emissive:
                float2 node_78 = i.uv0;
                float3 emissive = (tex2D(_MainTex,TRANSFORM_TEX(node_78.rg, _MainTex)).rgb*((ceil((sqrt(saturate((dot(normalize((_WorldSpaceLightPos0.rgb-i.posWorld.rgb)),i.normalDir)+_Bias)))*_Levels))/_Levels)*0.6+0.4));
                float3 finalColor = emissive;
/// Final Color:
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
