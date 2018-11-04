float4x4 World;
float4x4 View;
float4x4 Projection;

int mode = 1, size = 3;
float pixelX, pixelY;
int width, height;

sampler s0;

float4 PixelShaderFunction(float4 pos : SV_Position, float4 color1 : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
	float4 sideColors = tex2D(s0, coords) / 3;

	//v blur
	if (mode == 0) {
		sideColors += tex2D(s0, float2(coords.x - pixelX * 1.5, coords.y)) / 3;
		sideColors += tex2D(s0, float2(coords.x + pixelX * 1.5, coords.y)) / 3;
		//sideColors += tex2D(s0, float2(coords.x - pixelX, coords.y)) / 3;
		//sideColors += tex2D(s0, float2(coords.x + pixelX, coords.y)) / 3;
	}
	//h blur
	else if (mode == 1) {
		sideColors += tex2D(s0, float2(coords.x, coords.y - pixelY * 1.5)) / 3;
		sideColors += tex2D(s0, float2(coords.x, coords.y + pixelY * 1.5)) / 3;
		//sideColors += tex2D(s0, float2(coords.x, coords.y - pixelY)) / 3;
		//sideColors += tex2D(s0, float2(coords.x, coords.y + pixelY)) / 3;
	}
	//return blured image
	else if (mode == 2)
				return tex2D(s0, float2(pos.x / (float)width, pos.y / (float)height)) * color1;
		//return tex2D(s0, float2(pos.x / (float)width, pos.y / (float)height))
		//* float4(0.3,0.3,0.3,1) + color1;
		
	return sideColors;
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile  ps_4_1  PixelShaderFunction();
	}
}
