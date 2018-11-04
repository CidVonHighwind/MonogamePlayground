float4x4 World;
float4x4 View;
float4x4 Projection;

sampler s0;
float value = 0.65;

Texture2D sprImage;

SamplerState sprImageSampler
{
    //Filter = MIN_MAG_MIP_LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};


float4 PixelShaderFunction(float4 pos : SV_Position, float4 color1 : COLOR0, float2 coords : TEXCOORD0) : COLOR0
{
	float4 sideColors = tex2D(s0, coords);
	
	float4 lightColor = sprImage.Sample(sprImageSampler, coords);
	
	return float4(lightColor.r * sideColors.r, lightColor.g * sideColors.g, lightColor.b * sideColors.b, 1);
}

technique Technique1
{
	pass Pass1
	{
		PixelShader = compile  ps_4_1  PixelShaderFunction();
	}
}