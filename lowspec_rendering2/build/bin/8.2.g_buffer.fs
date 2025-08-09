#version 330 core
layout (location = 0) out vec4 gPosition;
layout (location = 1) out vec4 gNormal;
layout (location = 2) out vec4 gAlbedoSpec;

in VS_OUT {
    vec3 FragPos;
    vec3 Normal;
    vec2 TexCoords;
} fs_in;

uniform sampler2D texture_diffuse1;
uniform sampler2D texture_specular1;

void main()
{
    gPosition = vec4(fs_in.FragPos, 1.0);
    gNormal   = vec4(normalize(fs_in.Normal), 1.0);
    vec3  albedo = texture(texture_diffuse1, fs_in.TexCoords).rgb;
    float spec   = texture(texture_specular1, fs_in.TexCoords).r;
    gAlbedoSpec = vec4(albedo, spec);
}

