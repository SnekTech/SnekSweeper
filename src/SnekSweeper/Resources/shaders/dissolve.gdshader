shader_type canvas_item;

uniform float progress: hint_range(0.0, 1.0);
uniform float coverAlpha: hint_range(0.0, 1.0) = 1.0f;
uniform float noiseSeed;
uniform sampler2D mask: source_color;

float rand(float x) {
    return fract(sin(x) * 43758.5453123);
}

float rand(vec2 st) {
    return fract(sin(dot(st, vec2(12.9898, 78.233))) * 43758.5453123);
}

vec2 randomUV(vec2 uv, float t) {
    const float steps = 2.0;
    const float stepLength = 1.0 / steps;
    
    vec2 result = vec2(0.0);
    float x = floor(steps * rand(t));
    result.x = x * stepLength + uv.x * stepLength;
    float y = floor(steps * rand(t));
    result.y = y * stepLength + uv.y * stepLength;
    
    return result;
}

void fragment() {
    vec2 mappedUV = randomUV(UV, noiseSeed);
	float threshold = texture(mask, mappedUV).r;
    float alpha = step(progress, threshold) * coverAlpha;

    COLOR = vec4(COLOR.rgb, alpha);
}