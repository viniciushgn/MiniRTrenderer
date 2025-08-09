#version 330 core
out vec4 FragColor; in vec3 WorldDir; uniform samplerCube envMap; const float PI=3.14159265; void main(){ // convolve diffuse (omitted full kernel for brevity)
    vec3 N = normalize(WorldDir); vec3 irradiance = texture(envMap, N).rgb; FragColor = vec4(irradiance,1.0);} 