#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;


struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 coords : TEXCOORD0;
};

float width, height, posX, posY;

float4 MainPS(VertexShaderOutput input) : COLOR
{
	double  coordX = input.coords.x * width + posX;
	double  coordY = input.coords.y * height + posY;

	//float value = input.Position.x;

	double valueX = 0;
	double  valueY = 0;

	double  tempX, tempY;

	for	(int i = 0; i < 100; i++){
		tempX = valueX * valueX - valueY * valueY + coordX;
		tempY = valueX * valueY + valueX * valueY + coordY;

		valueX = tempX;
		valueY = tempY;
	}

	float value = valueX * valueY;

	if(value < 0)
		value = -value;

	return float4(value * 100, value * 100, 1, 1);
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile ps_5_0 MainPS();
	}
};