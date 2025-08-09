#version 330 core
layout(location=0) in vec3 aPos; out vec3 localPos; uniform mat4 captureVP; void main(){ localPos=aPos; gl_Position=captureVP*vec4(aPos,1.0);} 