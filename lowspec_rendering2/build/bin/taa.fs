#version 330 core
out vec4 FragColor; in vec2 vUV;
uniform sampler2D hdrCurr; uniform sampler2D hdrHistory; uniform sampler2D velocityTex; uniform float feedback; uniform vec2 invRenderSize;

vec3 sampleCatmullRom(sampler2D tex, vec2 uv){ return texture(tex, uv).rgb; } // simple for demo

void main(){
    vec2 vel = texture(velocityTex, vUV).xy; // uv delta (prev - curr)
    vec2 prevUV = vUV + vel; // where current pixel came from
    vec3 curr = texture(hdrCurr, vUV).rgb;
    vec3 hist = sampleCatmullRom(hdrHistory, prevUV);

    // neighborhood clamp (min/max in 3x3 of current)
    vec3 cmin = curr, cmax = curr;
    for(int y=-1;y<=1;++y) for(int x=-1;x<=1;++x){
        vec2 o=vec2(x,y)*invRenderSize; vec3 s=texture(hdrCurr, vUV+o).rgb; cmin=min(cmin,s); cmax=max(cmax,s);
    }
    hist = clamp(hist, cmin, cmax);

    vec3 outc = mix(hist, curr, feedback); // combine
    FragColor = vec4(outc,1.0);
}