#version 330 core
out vec4 FragColor; in vec3 WorldDir; uniform samplerCube envMap; uniform float roughness; void main(){ vec3 N=normalize(WorldDir); vec3 c = textureLod(envMap,N,roughness*5.0).rgb; FragColor=vec4(c,1.0);} 
