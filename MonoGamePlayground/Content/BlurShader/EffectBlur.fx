float4x4 World;
float4x4 View;
float4x4 Projection;

int mode = 1, size = 3;
float pixelX, pixelY;
int width, height;

sampler s0;

float4 PixelShaderFunction(float4 pos : SV_Position, float4 color1 : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
	float4 sideColors = tex2D(s0, float2(coords.x + pixelX * 0.5, coords.y + pixelY * 0.5)) * 0.402619947;

	//v blur
	if (mode == 0) {
		sideColors += tex2D(s0, float2(coords.x - pixelX * 1.5, coords.y + pixelY * 0.5)) * 0.244201342;
		sideColors += tex2D(s0, float2(coords.x + pixelX * 2.5, coords.y + pixelY * 0.5)) * 0.244201342;
		sideColors += tex2D(s0, float2(coords.x - pixelX * 3.5, coords.y + pixelY * 0.5)) * 0.054488685;
		sideColors += tex2D(s0, float2(coords.x + pixelX * 4.5, coords.y + pixelY * 0.5)) * 0.054488685;
	}
	//h blur
	else if (mode == 1) {
		sideColors += tex2D(s0, float2(coords.x + pixelY * 0.5, coords.y - pixelY * 1.5)) * 0.244201342;
		sideColors += tex2D(s0, float2(coords.x + pixelY * 0.5, coords.y + pixelY * 2.5)) * 0.244201342;
		sideColors += tex2D(s0, float2(coords.x + pixelY * 0.5, coords.y - pixelY * 3.5)) * 0.054488685;
		sideColors += tex2D(s0, float2(coords.x + pixelY * 0.5, coords.y + pixelY * 4.5)) * 0.054488685;
	}
		
	return sideColors;
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile  ps_4_1  PixelShaderFunction();
	}
}
