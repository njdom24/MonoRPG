sampler s0;
float time;

float4 VerticalStretch(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	texCoord.y = texCoord.y + 0.4*sin(texCoord.y + time * 1);
	float4 color = tex2D(s0, texCoord);
	
	return color;
}

float4 HorizontalStretch(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	texCoord.x = texCoord.x + 0.4*sin(texCoord.x + time * 2);
	float4 color = tex2D(s0, texCoord);

	return color;
}

float4 Scroll(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	texCoord.x = texCoord.x + time/4;
	float4 color = tex2D(s0, texCoord);

	return color;
}

float4 SineWave(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	texCoord.y = texCoord.y + 0.2*sin(10*texCoord.x + time*2)+0.1;
	float4 color = tex2D(s0, texCoord);

	return color;
}

float4 HorizontalSineWave(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	texCoord.x = texCoord.x + 0.2*sin(10 * texCoord.y + time * 2) + 0.1;
	float4 color = tex2D(s0, texCoord);

	return color;
}

float4 InvertColors(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	float4 color = tex2D(s0, texCoord);
	color.rgb = 1-color.rgb;

	return color;
}

float4 HighContrast(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	float4 color = tex2D(s0, texCoord);
	float high = .5;
	float low = .4;

	if (color.r > high) color.r = 1;
	else if (color.r < low) color.r = 0;

	if (color.g > high) color.g = 1;
	else if (color.g < low) color.g = 0;

	if (color.b > high) color.b = 1;
	else if (color.b < low) color.b = 0;

	return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_5_0 VerticalStretch();
    }
	pass Pass2
	{
		PixelShader = compile ps_5_0 HorizontalSineWave();
	}
	pass Pass3
	{
		PixelShader = compile ps_5_0 Scroll();
	}
}