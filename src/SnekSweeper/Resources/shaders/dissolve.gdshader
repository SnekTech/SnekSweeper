shader_type canvas_item;

uniform float progress: hint_range(0.0, 1.0);
uniform float coverAlpha: hint_range(0.0, 1.0) = 1.0f;
uniform sampler2D mask: source_color;

void fragment() {
	float threshold = texture(mask, UV).r;
    float alpha = step(progress, threshold) * coverAlpha;

    COLOR = vec4(COLOR.rgb, alpha);
}
