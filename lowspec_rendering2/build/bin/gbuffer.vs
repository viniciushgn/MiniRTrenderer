#version 330 core
layout (location=0) in vec3 aPos; layout(location=1) in vec3 aNormal; layout(location=2) in vec2 aUV;

out VS_OUT { vec3 FragPos; vec3 Normal; vec2 UV; vec4 CurrClip; vec4 PrevClip; } vs;

uniform mat4 model, view, projection; uniform mat4 prevModel; uniform mat4 prevViewProj;

void main(){
    vec4 world = model * vec4(aPos,1.0);
    vec4 prevWorld = prevModel * vec4(aPos,1.0);
    vs.FragPos = world.xyz;
    vs.Normal  = mat3(transpose(inverse(model))) * aNormal;
    vs.UV = aUV;
    vs.CurrClip = projection * view * world;
    vs.PrevClip = prevViewProj * prevWorld; // includes previous jitter
    gl_Position = vs.CurrClip;
}