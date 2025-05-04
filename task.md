# Workflow

- Create a new branch for the task (e.g., git checkout -b feature/<task-name>).
- Move selected task from "To Do" to "In Progress".
- Implement code changes addressing the task.
- Run builds, tests, and manual verification.
- Move task from "In Progress" to "Completed".
- Commit changes to the branch with descriptive message.
- Push branch to remote (git push -u origin feature/<task-name>).
- Open a pull request and undergo code review.
- Merge pull request into main and delete the branch.

# Progress

## To Do
- Add multiple chunks
- Add on-screen GUI (FPS counter, instructions)
- Provide a usage tutorial or guide
- Add debug wireframe toggle (F1 key)

## In Progress
- Add multiple chunks

## Completed
- Fix block add/remove functionality so mouse interactions update the world and mesh correctly
- Implement multi-chunk world rendering with frustum culling
- Add fog effects and skybox background
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

# Tasks

- Create a voxel engine (core functionality: chunks, mesh building, world management).
- Create a sample game using voxels (console-based & real-time 3D with OpenTK/MonoGame).
- Add more features as listed in progress section.

# Tech

- Dotnet
- Any code-first game engine for dotnet.
- Coverlet
- Stryker
- Git. Use commits as appropriate.
- Add to list as needed.

## Multi-chunk World Refactor Plan

- Extract world construction into a BuildWorld(int extent) method that populates a World instance with multiple chunks.
- Extract mesh generation and GPU resource setup into a SetupChunkMeshes(World, MeshBuilder) method that returns a list of (ChunkPosition pos, int vao, int indexCount).
- In window.Load, call SetupChunkMeshes to initialize VAOs and index counts for each chunk in the GPU.
- In window.RenderFrame, iterate over the chunk render data, set the model matrix per chunk, bind its VAO, and call DrawElements.
- Remove legacy single-chunk mesh code once the multi-chunk path is verified.
- Validate project builds and renders correctly after each refactor step.

# C# style rules

- File-scoped namespaces.
- Prefer collection expressions.
- Prefer 'var'.
- Prefer immutable records.

