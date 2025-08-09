#version 330 core
layout(location=0) in vec3 aPos; out vec3 WorldDir; uniform mat4 captureVP; void main(){ WorldDir=aPos; gl_Position=captureVP*vec4(aPos,1.0);} 