#version 330 core
layout (location=0) out vec4 outAlbedo;    // rgb=albedo, a=unused
layout (location=1) out vec4 outNormal;    // xyz=normal (world), w=unused
layout (location=2) out vec2 outRM;        // r=roughness, g=metalness
layout (location=3) out vec3 outEmissive;  // emissive rgb
layout (location=4) out vec2 outVelocity;  // screen-space motion (uv delta current->prev)

in VS_OUT { vec3 FragPos; vec3 Normal; vec2 UV; vec4 CurrClip; vec4 PrevClip; } fs;

uniform sampler2D texture_diffuse1;
uniform sampler2D texture_metallic1;
uniform sampler2D texture_roughness1;
uniform sampler2D texture_emissive1;

vec2 ndc(vec4 clip){ return (clip.xy/clip.w); }

void main(){
    vec3 albedo = pow(texture(texture_diffuse1, fs.UV).rgb, vec3(2.2));
    float rough = texture(texture_roughness1, fs.UV).r; if(rough==0.0) rough=0.5;
    float metal = texture(texture_metallic1, fs.UV).r;
    vec3 emiss = texture(texture_emissive1, fs.UV).rgb;

    vec3 N = normalize(fs.Normal);

    vec2 curr = ndc(fs.CurrClip); vec2 prev = ndc(fs.PrevClip);
    vec2 vel = (prev - curr); // reprojection samples history at uv + vel

    outAlbedo   = vec4(albedo, 1.0);
    outNormal   = vec4(N, 0.0);
    outRM       = vec2(rough, metal);
    outEmissive = emiss;
    outVelocity = vel;
}