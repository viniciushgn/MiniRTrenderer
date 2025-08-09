#version 330 core
out vec4 FragColor; in vec2 vUV;

uniform sampler2D gAlbedo, gNormal, gRM, gEmissive, gVelocity, gDepth;
uniform vec3 viewPos; uniform mat4 invView, invProj;

struct Light{ vec3 Position; vec3 Color; float Linear; float Quadratic; float Radius; };
const int MAX_LIGHTS = 64;
uniform int numLights;
uniform Light lights[MAX_LIGHTS];

// helpers
vec3  toView(vec2 uv, float depth){ vec4 ndc = vec4(uv*2.0-1.0, depth*2.0-1.0, 1.0); vec4 viewPos4 = invProj * ndc; return (viewPos4.xyz/viewPos4.w); }
vec3  toWorld(vec3 viewP){ return (invView * vec4(viewP,1.0)).xyz; }
float saturate(float x){ return clamp(x,0.0,1.0); }

const float PI=3.14159265;
float D_GGX(float NoH, float a){ float a2=a*a; float d=(NoH*a2 - NoH)*NoH + 1.0; return a2/(PI*d*d); }
float V_SmithGGXCorrelated(float NoV, float NoL, float a){ float a2=a*a; float lambdaV = NoL*sqrt((NoV-NoV*a2)*NoV + a2); float lambdaL = NoV*sqrt((NoL-NoL*a2)*NoL + a2); return 0.5/(lambdaV+lambdaL+1e-5); }
vec3  F_Schlick(vec3 F0,float u){ float f=pow(1.0-u,5.0); return F0 + (1.0-F0)*f; }

void main(){
    vec3  albedo = texture(gAlbedo, vUV).rgb;
    vec3  N      = normalize(texture(gNormal, vUV).xyz);
    vec2  rm     = texture(gRM, vUV).xy; float rough = clamp(rm.x,0.04,1.0); float metal = rm.y;
    vec3  emiss  = texture(gEmissive, vUV).rgb;
    float z      = texture(gDepth, vUV).r; if(z==1.0){ discard; }

    // reconstruct world position and view vector
    vec3 Vpos    = toView(vUV, z);
    vec3 Wpos    = toWorld(Vpos);
    vec3 V       = normalize(viewPos - Wpos);

    vec3  F0 = mix(vec3(0.04), albedo, metal);
    float a  = max(1e-3, rough*rough);

    vec3 Lo = vec3(0.0);
    for(int i=0;i<numLights && i<MAX_LIGHTS;++i){
        vec3 Lpos = lights[i].Position; vec3 L = normalize(Lpos - Wpos);
        float dist = length(Lpos - Wpos);
        float att = 1.0 / (1.0 + lights[i].Linear*dist + lights[i].Quadratic*dist*dist);

        vec3 H = normalize(V+L);
        float NoV = max(dot(N,V), 1e-4);
        float NoL = max(dot(N,L), 0.0);
        float NoH = max(dot(N,H), 0.0);

        vec3  F   = F_Schlick(F0, max(dot(H,V),0.0));
        float D   = D_GGX(NoH, a);
        float Vis = V_SmithGGXCorrelated(NoV,NoL,a);

        vec3  spec = (D*Vis) * F;
        vec3  kd = (1.0 - F) * (1.0 - metal);
        vec3  diff = kd * albedo / PI;

        vec3  c = (diff + spec) * (NoL * att) * lights[i].Color;
        if(dist < lights[i].Radius) Lo += c;
    }

    vec3 color = Lo + emiss; // DIRECT lighting only (no IBL)
    FragColor = vec4(color,1.0);
}