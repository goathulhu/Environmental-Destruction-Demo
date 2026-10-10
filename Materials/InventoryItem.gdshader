shader_type canvas_item;

uniform sampler2D textureimg : source_color;
uniform sampler2D texturebg : source_color;
uniform vec4 color : source_color;
uniform float offset = 0.0;

void fragment()
{
	vec4 img = vec4(color.rgb * 0.5, texture(textureimg, UV + vec2(offset, 0.0)).r);
	vec4 bg = vec4(color.rgb * 0.5, texture(texturebg, UV + vec2(offset, 0.0)).r);
	COLOR = img + bg;
}
