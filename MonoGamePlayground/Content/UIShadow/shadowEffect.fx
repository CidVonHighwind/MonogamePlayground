int depth = 5;
float pixelSizeX, pixelSizeY;
sampler s0;

float4 MainPS(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 coords: TEXCOORD0) : COLOR
{
	float4 color = float4(0,0,0,0);
	
	//don´t draw shadow if there is stuff over it
	clip(tex2D(s0, coords).a >  0.05f ? -1 : 1);
		
	for (int i = 0; i < 15; i++) {
		clip(i + 1 > depth ? -1 : 1);
		
		//draw the shadow
		//move up+left
		if (tex2D(s0, coords - float2(pixelSizeX, pixelSizeY) * (i + 1)).a != 0 ||
				(tex2D(s0, coords - float2(pixelSizeX, pixelSizeY) * (i + 1) + float2(pixelSizeX, 0)).a != 0 && 
				 tex2D(s0, coords - float2(pixelSizeX, pixelSizeY) * (i + 1) + float2(0, pixelSizeY)).a != 0))
				return float4(0, 0, 0, 1.0 / depth * 0.5 * (depth - i));

	}
	
	return color;
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile ps_4_1 MainPS();
	}
};