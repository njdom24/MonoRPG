sampler s0;

float4 BackgroundShift(float4 pos : SV_POSITION, float4 color1 : COLOR0, float2 texCoord : TEXCOORD0) : SV_TARGET0
{
	float4 color = tex2D(s0, texCoord);
	
	color.r = 0;
	//color.a = mask.a;
	return color;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_5_0 BackgroundShift();
    }
}