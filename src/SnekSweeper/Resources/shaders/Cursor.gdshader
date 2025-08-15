shader_type canvas_item;

uniform vec4 tint: source_color = vec4(1.0);
uniform float borderWidth: hint_range(0.0, 1.0, 0.01) = 0.2;
uniform float freq: hint_range(1.0, 10.0, 0.1);

void fragment() {
    float x = UV.x;
    float y = UV.y;
    bool isTopBorder = y > 1.0 - borderWidth;
    bool isBottomBorder = y < borderWidth;
    bool isLeftBorder = x < borderWidth;
    bool isRightBorder = x > 1.0 - borderWidth;

    if (isTopBorder || isBottomBorder || isLeftBorder || isRightBorder) {
        COLOR = mix(COLOR, tint, sin(TIME * freq));
    }
    else {
        COLOR = vec4(0.0);
    }
}