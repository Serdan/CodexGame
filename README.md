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
dotnet run -c Debug
```
Press ESC to exit the game loop.

## Usage Guide

**Controls:**
- W/S/A/D: Move forward/backward/left/right
- Space / Left Control: Move up / down
- Mouse Move: Look around
- Left-click: Remove a block (voxel)
- Right-click: Place a block at targeted location
- F1: Toggle wireframe view
- ESC: Exit the game

**Gameplay:**
1. Run the game to explore procedurally generated terrain across multiple chunks.
2. Use mouse-look to aim at blocks and left/right click to remove or add.
3. Toggle wireframe (F1) to inspect mesh structure.
4. Press ESC to quit.

## Mutation Testing (Stryker.NET)
Install Stryker.NET globally if not already installed:
```
dotnet tool install -g dotnet-stryker
```
Then run mutation testing from the repo root:
```
dotnet stryker
```