shader_type canvas_item;

uniform sampler2D screen_texture;

void fragment()
{
	int steps = 16;
	vec3 screen = texture(screen_texture, SCREEN_UV).rgb;
	vec3 quantized = floor(screen * float(steps)) / float(steps);
	COLOR = vec4(quantized, 1.0);
}
