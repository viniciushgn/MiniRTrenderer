#version 330 core
out vec4 FragColor;

in vec2 TexCoords;

uniform sampler2D gPosition;
uniform sampler2D gNormal;
uniform sampler2D gAlbedoSpec;

struct Light {
    vec3 Position;
    vec3 Color;
    
    float Linear;
    float Quadratic;
    float Radius;
};

const int MAX_LIGHTS = 512;
uniform Light lights[MAX_LIGHTS];
uniform int numLights;
uniform vec3 viewPos;

void main()
{
    // retrieve data from G-buffer
    vec3 FragPos = texture(gPosition, TexCoords).rgb;
    vec3 Normal = texture(gNormal, TexCoords).rgb;
    vec3 Diffuse = texture(gAlbedoSpec, TexCoords).rgb;
    float SpecularStrength = texture(gAlbedoSpec, TexCoords).a;
    
    // then calculate lighting as usual
    vec3 lighting = Diffuse * 0.1; // hard-coded ambient component
    vec3 viewDir = normalize(viewPos - FragPos);

    for (int i = 0; i < numLights; ++i)
    {
        vec3  L     = lights[i].Position - FragPos;
        float dist2 = dot(L, L);
        float rad2  = lights[i].Radius * lights[i].Radius;
        if (dist2 > rad2) continue;         // quick reject, no sqrt

        float invDist = inversesqrt(max(dist2, 1e-8)); // 1/sqrt(r^2)
        vec3  lightDir = L * invDist;

        float ndotl = max(dot(Normal, lightDir), 0.0);
        if (ndotl <= 0.0) continue;         // skip spec/atten if backfacing

        // attenuation: 1 / (1 + linear * d + quadratic * d^2)
        float distance = 1.0 / invDist;     // sqrt(dist2)
        float attenuation = 1.0 / (1.0 + lights[i].Linear * distance 
                                       + lights[i].Quadratic * dist2);

        vec3 diffuse  = ndotl * Diffuse * lights[i].Color * attenuation;

        // Blinn-Phong spec (skip if ndotl == 0)
        vec3  halfwayDir = normalize(lightDir + viewDir);
        float spec = pow(max(dot(Normal, halfwayDir), 0.0), 16.0);
        vec3  specular = lights[i].Color * spec * SpecularStrength * attenuation;

        lighting += diffuse + specular;
    }
    
    FragColor = vec4(lighting, 1.0);
}