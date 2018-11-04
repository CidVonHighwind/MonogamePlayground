float4x4 xViewProjection;

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

sampler s0;

float pixelWidth;
int posX, posY;
float width;
float height;

struct VertexShaderOutput
{
	//float4 Position : SV_Position;
	float4 Pos : SV_Position;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

struct PixelShaderInput
{
	//float4 Position : SV_Position;
	float4 Pos : SV_Position;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
	float2 RealCoord : TEXCOORD1;
};

PixelShaderInput SpriteVertexShader(VertexShaderOutput input)
{
    PixelShaderInput Output = (PixelShaderInput)0;

	input.Pos.x -= (input.Pos.y - posY - height) / height * width * 0.3;
	input.Pos.y -= (-(posY + height) + input.Pos.y) / height * height * 0.35;
	
	Output.RealCoord = float2((input.Pos.x - posX) / width, (input.Pos.y - posY - (height * 0.35)) / (height / 2));
    Output.Pos = mul(input.Pos, xViewProjection);
    
	Output.Color = input.Color;
	Output.TextureCoordinates = input.TextureCoordinates;
	
    return Output;
}

float4 MainPS(PixelShaderInput input) : COLOR
{
	float4 texColor = tex2D(s0, float2(input.TextureCoordinates.x, input.TextureCoordinates.y));
	float4 texColor1 = tex2D(s0, float2(input.TextureCoordinates.x + (input.RealCoord.y - 1) * pixelWidth, input.TextureCoordinates.y));
	float4 texColor2 = tex2D(s0, float2(input.TextureCoordinates.x - (input.RealCoord.y - 1) * pixelWidth, input.TextureCoordinates.y));
	
	float4 texColorOut = texColor * 0.4 + texColor1 * 0.3 + texColor2 * 0.3;

	//input.RealCoord.y 
	return texColorOut.a * float4(0, 0, 0, 0.5) * (input.RealCoord.y * 0.65 + 0.35);
}

technique SpriteDrawing
{
	pass P0
	{
        VertexShader = compile vs_4_0_level_9_1 SpriteVertexShader();
		PixelShader = compile ps_4_0_level_9_1 MainPS();
	}
};