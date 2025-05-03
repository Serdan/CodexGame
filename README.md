# Voxel Engine

This repository contains a simple voxel engine core library and a sample console-based game demonstrating its usage.

## Projects
- **VoxelEngine.Core**: Core voxel engine logic (chunk management, mesh generation, game loop).
- **VoxelGame**: Console application showcasing the voxel engine.
- **VoxelEngine.Tests**: xUnit tests covering core functionality.

## Requirements
- .NET 9.0 SDK

## Building and Testing
1. Restore and build all projects:
   ```
   dotnet build
   ```
2. Run unit tests with coverage:
   ```
   dotnet test --collect:"XPlat Code Coverage"
   ```

## Running the Sample Game
Navigate to the `src/VoxelGame` directory and run:
```
dotnet run
```
Press ESC to exit the game loop.

## Mutation Testing (Stryker.NET)
Install Stryker.NET globally if not already installed:
```
dotnet tool install -g dotnet-stryker
```
Then run mutation testing from the repo root:
```
dotnet stryker
```