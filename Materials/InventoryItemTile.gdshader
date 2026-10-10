shader_type canvas_item;

uniform sampler2D textureimg : source_color, repeat_enable;
uniform sampler2D texturebg : source_color, repeat_enable;
uniform vec4 color : source_color;

void fragment()
{
	if (UV.x - floor(UV.x) < 0.5)
	{
		COLOR = 
			vec4(color.rgb * 0.25, texture(textureimg, vec2(UV.x + 0.5, UV.y)).r) + 
			vec4(color.rgb * 0.25, texture(texturebg, vec2(UV.x + 0.5, UV.y)).r) + 
			vec4(color.rgb * 0.25, texture(textureimg, vec2(UV.x, UV.y)).r * (UV.x * 2.0 - floor(UV.x * 2.0)) * (UV.x * 2.0 - floor(UV.x * 2.0)) * (UV.x * 2.0 - floor(UV.x * 2.0))) + 
			vec4(color.rgb * 0.25, texture(texturebg, vec2(UV.x, UV.y)).r * (UV.x * 2.0 - floor(UV.x * 2.0)) * (UV.x * 2.0 - floor(UV.x * 2.0)) * (UV.x * 2.0 - floor(UV.x * 2.0)));
	}
	else
	{
		COLOR = 
			vec4(color.rgb * 0.25, texture(textureimg, vec2(UV.x + 0.5, UV.y)).r * (UV.x * 2.0 - floor(UV.x * 2.0)) * (UV.x * 2.0 - floor(UV.x * 2.0)) * (UV.x * 2.0 - floor(UV.x * 2.0))) + 
			vec4(color.rgb * 0.25, texture(texturebg, vec2(UV.x + 0.5, UV.y)).r * (UV.x * 2.0 - floor(UV.x * 2.0)) * (UV.x * 2.0 - floor(UV.x * 2.0)) * (UV.x * 2.0 - floor(UV.x * 2.0))) + 
			vec4(color.rgb * 0.25, texture(textureimg, vec2(UV.x, UV.y)).r) + 
			vec4(color.rgb * 0.25, texture(texturebg, vec2(UV.x, UV.y)).r);
	}
}
