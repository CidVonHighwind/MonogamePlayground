#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_3
#endif

sampler2D s0
{
    MipFilter = Point;
    MinFilter = Point;
    MagFilter = Point;
    AddressU = Wrap;
    AddressV = Wrap;
};

int posX;
int posY;
int imageWidth;
int imageHeight;
int size;

bool enabled = true;

float4 MainPS(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 coords: TEXCOORD0) : COLOR
{
	int abstand = sqrt(pow(abs(coords.x * imageWidth - posX), 2) + pow(abs(coords.y * imageHeight - posY), 2));

	if (enabled && abstand > size)
	{
		int mode = (int)((abstand - size) / size) + 2;
		return tex2D(s0, 
			float2(	((int)(coords.x * imageWidth) - ((coords.x * imageWidth) % mode)) / (float)imageWidth,
					((int)(coords.y * imageHeight) - ((coords.y * imageHeight) % mode)) / (float)imageHeight)
			);

		float pixelX, pixelY;
		float4 sideColors = float4(0, 0, 0, 0);

		pixelX = 1.0f / imageWidth * 1;
		pixelY = 1.0f / imageHeight * 1;

		const int size = 3;

		for (int y = 0; y < size; y++)
			for (int x = 0; x < size; x++)
				sideColors += tex2D(s0, float2(coords.x - pixelX * (int)(size / 2)
					+ pixelX * x, coords.y - pixelY * (int)(size / 2) + pixelY * y));

		return (sideColors / (size * size));
	}

	return tex2D(s0, coords);
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};