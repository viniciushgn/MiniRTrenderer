#version 330 core
out vec4 FragColor; in vec2 vUV; uniform sampler2D hdr; uniform vec2 srcSize;

vec3 ACESFilm(vec3 x){
    const float a=2.51,b=0.03,c=2.43,d=0.59,e=0.14; return clamp((x*(a*x+b))/(x*(c*x+d)+e), 0.0, 1.0);
}

void main(){
    vec3 hdrColor = texture(hdr, vUV).rgb; // if Rw!=W, hardware upscales here (bilinear)
    vec3 mapped = ACESFilm(hdrColor);
    mapped = pow(mapped, vec3(1.0/2.2)); // gamma to display
    FragColor = vec4(mapped,1.0);
}