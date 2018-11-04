Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};


bool mirror;
float direction = 0.5;
float size = 0.5;

float posX, posY, width, height;

struct VertexShaderOutput
{
	float4 Position : SV_Position;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float x = (input.TextureCoordinates.x - direction + input.TextureCoordinates.y * direction);
	float y = (-1 + size + input.TextureCoordinates.y) / size;
	
	if(x < 0 || x > 1 || y < 0 || y > 1)
		return float4(0, 0, 0, 0);
	
	float4 tex;// = tex2D(SpriteTextureSampler, float2(posX + x * width, posY + y * height));
	
	if(mirror)
		tex = tex2D(SpriteTextureSampler, float2(posX + width - x * width, posY + y * height));
	else
		tex = tex2D(SpriteTextureSampler, float2(posX + x * width, posY + y * height));

	return float4(0, 0, 0, tex.a * 0.5) * input.Color;
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile ps_4_0 MainPS();
	}
};