#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

matrix WorldViewProjection;

sampler s0;

int width, height, posX, posY;

float pixelX, pixelY;

bool isRunning;

float4 MainPS(float4 pos : SV_Position, float4 color1 : COLOR0, float2 coords : TEXCOORD0) : COLOR
{
	//double  coordX = input.coords.x * width + posX;
	//double  coordY = input.coords.y * height + posY;

	if(!isRunning)
		return tex2D(s0, coords);


	// float4 sideColors = tex2D(s0, coords);

	float value = 0.5;
	bool alive = tex2D(s0, coords).r < value;
	int neighbourCount = 0;



	if(tex2D(s0, coords + float2(-pixelX, -pixelY)).r < value)
		neighbourCount++;
	if(tex2D(s0, coords + float2(-pixelX, 0)).r < value)
		neighbourCount++;
	if(tex2D(s0, coords + float2(-pixelX, pixelY)).r < value)
		neighbourCount++;
	if(tex2D(s0, coords + float2(pixelX, -pixelY)).r < value)
		neighbourCount++;
	if(tex2D(s0, coords + float2(pixelX, 0)).r < value)
		neighbourCount++;
	if(tex2D(s0, coords + float2(pixelX, pixelY)).r < value)
		neighbourCount++;
	if(tex2D(s0, coords + float2(0, -pixelY)).r < value)
		neighbourCount++;
	if(tex2D(s0, coords + float2(0, pixelY)).r < value)
		neighbourCount++;

	if(alive && neighbourCount < 2)
	{
		alive = false;
	}
	else if(alive && neighbourCount > 3){
		alive = false;
	}
	else if(!alive && neighbourCount == 3){
		alive = true;
	}

	return alive ? float4(0,0,0,1) : float4(1,1,1,1);// * float4(0.9,0.9,0.9,1);
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile ps_5_0 MainPS();
	}
};