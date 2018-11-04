float4x4 World;
float4x4 View;
float4x4 Projection;

int mode = 1, size = 3;
float pixelX, pixelY;
int width, height;

Texture2D SpirteDepth;
sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpirteDepth>;
};

Texture2D SpriteOriginal;
sampler2D SpriteOriginalSample = sampler_state
{
	Texture = <SpriteOriginal>;
};

sampler s0;

float4 PixelShaderFunction(float4 pos : SV_Position, float4 color1 : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
	float4 colorBlur = tex2D(s0, float2(coords.x, coords.y));
	float4 colorOriginal = tex2D(SpriteOriginalSample, float2(coords.x, coords.y));
	float4 colorDepth = tex2D(SpriteTextureSampler, float2(coords.x, coords.y));
		
	return lerp(colorBlur, colorOriginal, colorDepth.a) * colorOriginal.a;
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile  ps_4_1  PixelShaderFunction();
	}
}
