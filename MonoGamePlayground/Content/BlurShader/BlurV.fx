float4x4 World;
float4x4 View;
float4x4 Projection;

float pixelX, pixelY;
int width, height;

sampler s0;

float4 PixelShaderFunction(float4 pos : SV_Position, float4 color1 : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
	float4 sideColors = tex2D(s0, coords) * 0.402619947;

	sideColors += tex2D(s0, float2(coords.x, coords.y - pixelY * 1)) * 0.244201342;
	sideColors += tex2D(s0, float2(coords.x, coords.y + pixelY * 1)) * 0.244201342;
	sideColors += tex2D(s0, float2(coords.x, coords.y - pixelY * 2)) * 0.054488685;
	sideColors += tex2D(s0, float2(coords.x, coords.y + pixelY * 2)) * 0.054488685;
	
	return sideColors;
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile  ps_4_1  PixelShaderFunction();
	}
}
