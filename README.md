# Deferred Shading (SDL2 + OpenGL 3.3) — Dynamic Resolution Scaling

Compact deferred renderer based on LearnOpenGL’s 8.2 sample, extended with **dynamic resolution scaling (DRS)**, a **lighting L-buffer**, **per-light frustum culling**, and shader **early-outs**. Runs on **OpenGL 3.3 Core** with **SDL2** in a single, self-contained demo.

# Features

* **Deferred shading pipeline**

  * G-buffer: **RGB16F** position, **RGB16F** normal, **RGBA8** albedo+spec; depth **renderbuffer**.
  * Full-screen lighting to an internal **L-buffer** (GL\_RGBA8) at the current DRS resolution, then a single upscale to the window.
* **Dynamic Resolution Scaling (DRS)**

  * EMA frame-time controller with deadband; scales internal res in $DRS\_MIN, DRS\_MAX$ (default 0.2–1.0) toward a target frame time (default 16.6 ms).
  * **Both geometry and lighting** run at the internal resolution.
* **Lights & culling**

  * Up to **512** animated point lights.
  * CPU **frustum sphere culling**; GLSL **radius early-out** and `inversesqrt` optimization in the lighting loop.
* **Debug/UX**

  * View modes: Lit / gPosition / gNormal / gAlbedoSpec.
  * On-screen HUD: frame ms / FPS, light count, DRS scale & mode, internal resolution.
  * Runtime toggles for vsync, light count, and DRS target.

# Controls

* **W A S D + mouse** — free-fly camera
* **Mouse wheel** — zoom (FOV)
* **1 / 2 / 3 / 4** — Lit / gPosition / gNormal / gAlbedoSpec
* **\[  /  ]** — decrease / increase active light count
* **R** — toggle DRS auto/manual
* **-  /  =** — manual scale down / up (disables auto)
* **T** — cycle DRS targets (\~60/90/120 FPS)
* **V** — toggle vsync

# Requirements

* C++17 compiler, CMake ≥ 3.15
* Libraries: **SDL2**, **Assimp**, **GLM** (headers), **stb\_image** (header)
* **GLAD** loader
* LearnOpenGL helpers: `Shader.h`, `Camera.h`, `Model.h`, `FileSystem.h`
* Assets: LearnOpenGL **backpack** model and textures

# Getting Started

## Directory layout

```
repo/
├─ src/main.cpp
├─ shaders/
│  ├─ 8.2.g_buffer.vs
│  ├─ 8.2.g_buffer.fs
│  ├─ 8.2.deferred_shading.vs
│  ├─ 8.2.deferred_shading.fs
│  ├─ 8.2.deferred_light_box.vs
│  └─ 8.2.deferred_light_box.fs
├─ external/learnopengl/        # Shader.h, Camera.h, Model.h, FileSystem.h
└─ resources/objects/backpack/  # backpack.obj + textures
```

The code uses `FileSystem::getPath("resources/...")`. Place the `resources/` folder at the repo root (or adjust the helper).

## Build (Release)

### Linux (Ubuntu example)

```bash
sudo apt-get update
sudo apt-get install -y build-essential cmake libsdl2-dev libassimp-dev libglm-dev
# (stb_image and GLAD are header-only; vendor them or include via the project)
cmake -S . -B build -DCMAKE_BUILD_TYPE=Release
cmake --build build -j
./build/<your-binary-name>
```

### macOS (Homebrew)

```bash
brew install cmake sdl2 assimp glm
cmake -S . -B build -DCMAKE_BUILD_TYPE=Release
cmake --build build -j
./build/<your-binary-name>
```

### Windows

* Install CMake and a recent MSVC toolset.
* Option A: vendor libs/headers.
* Option B (recommended): use vcpkg for `sdl2`, `assimp`, `glm` and pass
  `-DCMAKE_TOOLCHAIN_FILE=<vcpkg>/scripts/buildsystems/vcpkg.cmake` to CMake.

```bat
cmake -S . -B build -DCMAKE_BUILD_TYPE=Release ^
  -DCMAKE_TOOLCHAIN_FILE=<path-to-vcpkg>\scripts\buildsystems\vcpkg.cmake
cmake --build build --config Release
build\Release\<your-binary-name>.exe
```

# How It Works (short)

* **G-buffer**: world-space position/normal (RGB16F), albedo+spec (RGBA8), depth RBO.
* **Lighting**: samples the G-buffer and writes into an **L-buffer** at DRS resolution; color is blitted with `GL_LINEAR` to the window. Depth is blitted (`GL_NEAREST`) from the G-buffer so light cubes render correctly.
* **DRS**: exponential moving average (α=0.1) of frame time, adjustments \~4×/s with a ±6% deadband; scales internal resolution in small steps (down faster than up).
* **Culling/Shader**: per-light frustum test on CPU; in GLSL, compute `dist2 = dot(L,L)` and skip if `dist2 > radius^2`, using `inversesqrt(dist2)` to avoid extra sqrt work.


# Roadmap

* UBO/TBO (or SSBO where available) for batched light upload.
* Tiled/clustered lighting.
* Optional modules: PBR/HDR/TAA as separate samples.

# Acknowledgements

* LearnOpenGL (helpers and assets)
* SDL2, GLAD, GLM, Assimp, stb\_image

