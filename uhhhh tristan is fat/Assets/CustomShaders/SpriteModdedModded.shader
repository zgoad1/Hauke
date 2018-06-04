Shader "CustomShaders/SpritesModdedModded"
 {
     Properties
     {
         [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
         _Color ("Tint", Color) = (1,1,1,1)
         [MaterialToggle] PixelSnap ("Pixel Snap", Float) = 1
         [PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
         [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
     }
 
     SubShader
     {
         Tags{"Queue"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True"}
 
         Cull Off
         Lighting Off
         ZWrite Off
         Blend One OneMinusSrcAlpha
 
         //Pass
         //{
         CGPROGRAM
         #pragma surface surf Lambert vertex:vert nolightmap nodynlightmap keepalpha noinstancing
         #pragma multi_compile _ PIXELSNAP_ON
         #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
         //#pragma multi_compile_fog
         //#include "UnityCG.cginc"
         #include "Assets/Includes/Sprites.cginc" 
             
         struct Input 
         {
             float2 uv_MainTex;
             fixed4 color;
                 //UNITY_FOG_COORDS(1)
         };
 
         void vert (inout appdata_full v, out Input o)
         {
             
             #if defined(PIXELSNAP_ON)
             v.vertex = UnityPixelSnap(v.vertex);
             #endif
 
             // So Output doesnt have to be initialized?? - Does NOTHING?
             UNITY_INITIALIZE_OUTPUT(Input, o);
 
             o.color = v.color * _Color;
                 //UNITY_TRANSFER_FOG(o, o.uv_MainTex);
         }
 
         void surf(Input IN, inout SurfaceOutput o) 
         {
             fixed4 c = SampleSpriteTexture(IN.uv_MainTex) * IN.color;
             o.Albedo = c.rgb * c.a;
             o.Alpha = c.a;
                 //UNITY_APPLY_FOG(IN.fogCoord, c);
         }
 
         ENDCG
         //}
     }
 
 //Fallback "Transparent/VertexLit"
 }