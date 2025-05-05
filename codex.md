# Progress

## To Do
- Come up with more gameplay elements.
- Add borderless fullscreen

## In Progress
- Add gravity to the player.
- Collision detection with terrain.
## Completed
- Fix block add/remove to work across all chunks (removed local-bound check in Raycast)
- Create world generation library: added `VoxelEngine.WorldGeneration` project with noise providers, world generator, integration, and tests
- Add debug wireframe toggle (F1 key)
- Fix block add/remove functionality so mouse interactions update the world and mesh correctly
- Implement multi-chunk world rendering with frustum culling
- Add fog effects and skybox background
- Provide a usage tutorial or guide
- Add on-screen GUI (FPS counter, instructions)
- Initialized .NET solution and projects
- Implemented core classes: Chunk, MeshData, MeshBuilder, GameEngine
- Created console sample game in VoxelGame
- Wrote unit tests for Chunk and MeshBuilder
- Configured Coverlet for code coverage
- Added Stryker mutation testing config
- Added README.md
- Integrated OpenTK for real-time 3D rendering
- Implemented camera controls and input handling
- Implemented greedy meshing for performance
- Implemented directional lighting (ambient + diffuse)
- Implemented ambient occlusion
- Implemented world save/load functionality
- Set up CI/CD pipeline (GitHub Actions)
- Expanded terrain generation with sinusoidal heightmap
- Added per-vertex colors (grass, dirt, stone)
- Switched to per-voxel face mesh to fix face alignment
- Added reticle overlay
- Improve documentation (XML comments, tutorials)

# Project

- Create a voxel engine (core functionality: chunks, mesh building, world management).
- Create a sample game using voxels (console-based & real-time 3D with OpenTK/MonoGame).

# Tech

- Dotnet
- Any code-first game engine for dotnet.
- Coverlet
- Stryker
- Git. Use commits as appropriate.
- Add to list as needed.

# C# style rules

- File-scoped namespaces.
- Prefer collection expressions.
- Prefer 'var'.
- Prefer immutable records.
  
## World Generation Library Design

### Overview
To support extensible and configurable world generation, we will introduce a new library project `VoxelEngine.WorldGeneration` that defines core interfaces, default noise-based generators, and integration points with `VoxelEngine.Core`.

### 1. Core Interfaces
- `INoiseProvider`: generates noise values for given coordinates and seed.
- `IWorldGenerator`: produces raw `Chunk` data (block types, heights, biomes) for a given chunk position.

### 2. Built-in Noise Implementations
- `PerlinNoiseProvider`: classic Perlin noise
- `SimplexNoiseProvider`: improved simplex noise
- `LayeredNoiseProvider`: combines multiple `INoiseProvider`s with different scales and weights.

### 3. Chunk-Level Generator API
- `Chunk GenerateChunk(int chunkX, int chunkY, int chunkZ)`: Produces a `Chunk` instance with blocks populated based on noise, biomes, and features.

### 4. Configuration & Extensibility
- `WorldGenerationConfig`: parameters (seed, height scale, biome thresholds, resource distributions).
- Support custom modules/plugins via DI of additional `IWorldFeature` implementations (e.g., caves, trees, ore veins).

### 5. Integration with VoxelEngine.Core
- `VoxelEngine.WorldGeneration` will reference `VoxelEngine.Core`.
- Extend `World` and/or `GameEngine` to accept an `IWorldGenerator` for dynamic world creation.

### 6. Testing Strategy
- Deterministic tests by seeding `INoiseProvider` implementations.
- Unit tests for `IWorldGenerator` producing expected block distributions, elevations, and biome maps.

