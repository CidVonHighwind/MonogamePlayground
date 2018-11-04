#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0
#endif

Texture2D SpriteTexture;

float softRad = 20;
float size;
int centerX, centerY;


sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float2 pos = float2(input.Position.x - centerX, input.Position.y - centerY);
	float black = length(pos);

	black = clamp((size - black) / softRad, 0, 1);

	return tex2D(SpriteTextureSampler,input.TextureCoordinates) * input.Color * float4(black, black, black, 1);
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};