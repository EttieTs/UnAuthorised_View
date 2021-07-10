Shader "Unlit/MenuShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Angle("Angle", Range(0, 360)) = 33
        _Alpha("Alpha", Range(0, 1)) = 0
        
    }
        SubShader
        {
            Tags
            {
                "RenderType" = "Transparent"
                "Queue" = "Transparent"
            }

            LOD 100

            Pass
            {
                Cull Off
                Zwrite Off
                Blend SrcAlpha OneMinusSrcAlpha

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 pos : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 pos : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                float _Angle = 10;
                float _Alpha = 0;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.pos);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    float2 uv = 2.0 * i.uv - 1.0; //takes it from 0 to 1 to -0.5 to 0.5
                    uv.x = -uv.x;
                    uv.y = -uv.y;
                    float angle1 = 0 * 3.14592 / 180.0;
                    float angle2 = _Angle * 3.14592 / 180.0;
                    float3 delta = float3(uv.xy,  0);
                    float2 dir1 = float2(sin(angle1), cos(angle1));
                    float3 delta1 = float3(dir1.xy, 0);
                    float c1 = cross(delta1, delta).z;
                    float2 dir2 = float2(sin(angle2), cos(angle2));
                    float3 delta2 = float3(dir2.xy, 0);
                    float c2 = cross(delta2, delta).z;
                    float4 col = tex2D(_MainTex, i.uv);
                    
                    col.a = 1.0;
                                    
                    if (_Angle <= 180.0)
                    {
                        if (c1 < 0)
                        {
                            col.a = _Alpha;
                        }
                        if (c2 > 0)
                        {
                            col.a = _Alpha;
                        }
                    }
                    else if (_Angle < 360)
                    {
                        if ((c1 < 0) && (c2 > 0))
                        {
                            col.a = _Alpha;
                        }
                    }
                    
                    float radius = length(uv);
                    if (radius > 1)
                    {
                        col.a = _Alpha;
                    }
                    
                    
                    return col;
                                      

                }
                ENDCG
            }
        }
}
