#version 330 core
out vec4 FragColor; in vec3 localPos; uniform sampler2D equirect; const float PI=3.14159265; 
vec2 sampleSphericalMap(vec3 v){ vec2 uv = vec2(atan(v.z,v.x), asin(v.y)); uv *= vec2(0.1591, 0.3183); uv += 0.5; return uv; }
void main(){ vec2 uv = sampleSphericalMap(normalize(localPos)); vec3 c = texture(equirect, uv).rgb; FragColor = vec4(c,1.0);} 

