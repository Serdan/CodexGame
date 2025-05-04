using System;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelEngine.Core;
using System.Collections.Generic;

namespace VoxelGame;

/// <summary>
/// Entry point for the VoxelGame sample application. Creates a window, builds a voxel mesh, and renders it with lighting and ambient occlusion.
/// </summary>
class Program
{
    /// <summary>
    /// Application entry point. Builds a sample voxel mesh and starts the OpenTK render window.
    /// </summary>
    static void Main()
    {
        // Build a multi-chunk world with sinusoidal heightmap
        var world = new World();
        int size = Chunk.Size;
        int extent = 1; // number of chunks in each direction
        for (int cx = -extent; cx <= extent; cx++)
            for (int cz = -extent; cz <= extent; cz++)
                for (int x = 0; x < size; x++)
                for (int z = 0; z < size; z++)
                {
                    float fx = x / (float)size * MathF.PI * 2;
                    float fz = z / (float)size * MathF.PI * 2;
                    float hf = (MathF.Sin(fx) + MathF.Sin(fz) + 2f) / 4f;
                    int height = (int)(hf * (size - 1));
                    if (height < 1) height = 1;
                    var cchunk = world.GetOrCreateChunk(new ChunkPosition(cx, 0, cz));
                    for (int y = 0; y <= height; y++)
                    {
                        byte id = y == height ? (byte)1 : y >= height - 2 ? (byte)2 : (byte)3;
                        cchunk.SetVoxel(x, y, z, id);
                    }
                }
        var meshBuilder = new MeshBuilder();
        bool wireframe = false;
        List<(ChunkPosition Position, int Vao, int IndexCount)> chunkMeshes = new();
        // Prepare reticle (simple crosshair)
        int lineShader = 0, lineVao = 0, lineVbo = 0;

        var gameSettings = GameWindowSettings.Default;
        var nativeSettings = new NativeWindowSettings
        {
            ClientSize = new Vector2i(800, 600),
            Title = "Voxel Game"
        };

        using var window = new GameWindow(gameSettings, nativeSettings);
        // Capture and hide cursor for mouse look
        window.CursorState = CursorState.Grabbed;
        // Camera for navigation
        var camera = new Camera(new Vector3(2.0f, 2.0f, 5.0f));
        // Raycast function for voxel selection
        bool Raycast(out int hx, out int hy, out int hz, out int px, out int py, out int pz)
        {
            var origin = camera.Position;
            var dir = camera.Front;
            // previous voxel coordinates
            px = (int)MathF.Floor(origin.X);
            py = (int)MathF.Floor(origin.Y);
            pz = (int)MathF.Floor(origin.Z);
            hx = hy = hz = 0;
            float maxDist = 20f;
            float step = 0.1f;
            var pos = origin;
            for (float t = 0; t < maxDist; t += step)
            {
                pos += dir * step;
                int x = (int)MathF.Floor(pos.X);
                int y = (int)MathF.Floor(pos.Y);
                int z = (int)MathF.Floor(pos.Z);
                if (x < 0 || x >= Chunk.Size || y < 0 || y >= Chunk.Size || z < 0 || z >= Chunk.Size)
                    continue;
                if (world.GetVoxel(x, y, z) != 0)
                {
                    hx = x; hy = y; hz = z;
                    return true;
                }
                px = x; py = y; pz = z;
            }
            return false;
        }
        // Handle mouse clicks for block interaction
        // Handle mouse clicks for block interaction
        window.MouseDown += args =>
        {
            if (args.Button == MouseButton.Left || args.Button == MouseButton.Right)
            {
                if (Raycast(out var hx, out var hy, out var hz, out var px2, out var py2, out var pz2))
                {
                    if (args.Button == MouseButton.Left)
                    {
                        world.SetVoxel(hx, hy, hz, 0);
                    }
                    else
                    {
                        world.SetVoxel(px2, py2, pz2, 1);
                    }
                    // Remesh world after block modification
                    chunkMeshes = SetupChunkMeshes(world, meshBuilder);
                }
            }
        };
        // Toggle wireframe mode
        window.KeyDown += args =>
        {
            if (args.Key == Keys.F1)
            {
                wireframe = !wireframe;
                GL.PolygonMode(MaterialFace.FrontAndBack, wireframe ? PolygonMode.Line : PolygonMode.Fill);
            }
        };
        int shaderProgram = 0;

        window.Load += () =>
        {
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            // Compile shaders with lighting, ambient occlusion, and vertex colors
            var vsSource = @"
#version 330 core
layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in float aAO;
layout(location = 3) in vec3 aColor;
out vec3 FragPos;
out vec3 Normal;
out float AO;
out vec3 Color;
out float Distance;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
void main()
{
    // World space position
    FragPos = vec3(model * vec4(aPosition, 1.0));
    // View space for fog distance
    vec4 posView = view * vec4(FragPos, 1.0);
    Distance = length(posView.xyz);
    // Transform normal
    Normal = mat3(transpose(inverse(model))) * aNormal;
    AO = aAO;
    Color = aColor;
    gl_Position = projection * posView;
}";
            var fsSource = @"
#version 330 core
in vec3 FragPos;
in vec3 Normal;
in float AO;
in vec3 Color;
out vec4 FragColor;
uniform vec3 lightDir;
uniform vec3 lightColor;
uniform float ambientStrength;
void main()
{
    vec3 ambient = ambientStrength * lightColor * AO;
    vec3 norm = normalize(Normal);
    float diff = max(dot(norm, normalize(-lightDir)), 0.0);
    vec3 diffuse = diff * lightColor;
    FragColor = vec4((ambient + diffuse) * Color, 1.0);
}";

            var vs = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vs, vsSource);
            GL.CompileShader(vs);
            CheckShaderCompile(vs);

            var fs = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fs, fsSource);
            GL.CompileShader(fs);
            CheckShaderCompile(fs);

            shaderProgram = GL.CreateProgram();
            GL.AttachShader(shaderProgram, vs);
            GL.AttachShader(shaderProgram, fs);
            GL.LinkProgram(shaderProgram);
            CheckProgramLink(shaderProgram);
            GL.DeleteShader(vs);
            GL.DeleteShader(fs);
            // Configure lighting uniforms
            GL.UseProgram(shaderProgram);
            GL.Uniform3(GL.GetUniformLocation(shaderProgram, "lightDir"), new Vector3(-0.5f, -1.0f, -0.3f));
            GL.Uniform3(GL.GetUniformLocation(shaderProgram, "lightColor"), new Vector3(1.0f, 1.0f, 1.0f));
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "ambientStrength"), 0.3f);
            // Fog parameters
            GL.Uniform3(GL.GetUniformLocation(shaderProgram, "fogColor"), new Vector3(0.6f, 0.7f, 0.8f));
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "fogStart"), 5.0f);
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "fogEnd"), 20.0f);

            // Setup viewport
            GL.Viewport(0, 0, window.ClientSize.X, window.ClientSize.Y);
            // Setup viewport and capture cursor
            GL.Viewport(0, 0, window.ClientSize.X, window.ClientSize.Y);
            window.CursorState = CursorState.Grabbed;
            // Generate and upload meshes for all chunks
            chunkMeshes = SetupChunkMeshes(world, meshBuilder);
            // Compile overlay shader for reticle
            var vsOverlay = @"#version 330 core
layout(location=0) in vec2 aPos;
void main(){ gl_Position = vec4(aPos, 0, 1); }";
            var fsOverlay = @"#version 330 core
out vec4 FragColor;
uniform vec3 uColor;
void main(){ FragColor = vec4(uColor,1); }";
            // Vertex shader
            var vs2 = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vs2, vsOverlay);
            GL.CompileShader(vs2);
            CheckShaderCompile(vs2);
            // Fragment shader
            var fs2 = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fs2, fsOverlay);
            GL.CompileShader(fs2);
            CheckShaderCompile(fs2);
            // Program
            lineShader = GL.CreateProgram();
            GL.AttachShader(lineShader, vs2);
            GL.AttachShader(lineShader, fs2);
            GL.LinkProgram(lineShader);
            CheckProgramLink(lineShader);
            GL.DeleteShader(vs2);
            GL.DeleteShader(fs2);
            // Reticle geometry in NDC
            float[] reticle = { -0.02f, 0f, 0.02f, 0f, 0f, -0.02f, 0f, 0.02f };
            lineVao = GL.GenVertexArray();
            lineVbo = GL.GenBuffer();
            GL.BindVertexArray(lineVao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, lineVbo);
            GL.BufferData(BufferTarget.ArrayBuffer, reticle.Length * sizeof(float), reticle, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        };

        // Adjust viewport on window resize
        window.Resize += args =>
        {
            GL.Viewport(0, 0, window.ClientSize.X, window.ClientSize.Y);
        };
        // Mouse movement for camera look
        const float mouseSensitivity = 0.2f;
        window.MouseMove += args =>
        {
            // Adjust camera orientation based on mouse movement
            camera.Yaw += args.DeltaX * mouseSensitivity;
            camera.Pitch -= args.DeltaY * mouseSensitivity;
            camera.Pitch = MathHelper.Clamp(camera.Pitch, -89f, 89f);
        };
        window.RenderFrame += args =>
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.UseProgram(shaderProgram);

            // Set view and projection once
            var view = camera.GetViewMatrix();
            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), window.Size.X / (float)window.Size.Y, 0.1f, 100f);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "view"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "projection"), false, ref projection);

            // Draw all chunk meshes
            foreach (var (pos, vaoID, count) in chunkMeshes)
            {
                var model = Matrix4.CreateTranslation(new Vector3(pos.X * Chunk.Size, pos.Y * Chunk.Size, pos.Z * Chunk.Size));
                GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "model"), false, ref model);
                GL.BindVertexArray(vaoID);
                GL.DrawElements(PrimitiveType.Triangles, count, DrawElementsType.UnsignedInt, 0);
            }
            // Draw reticle overlay
            GL.Disable(EnableCap.DepthTest);
            GL.UseProgram(lineShader);
            GL.BindVertexArray(lineVao);
            // White color crosshair
            GL.Uniform3(GL.GetUniformLocation(lineShader, "uColor"), new Vector3(1f, 1f, 1f));
            GL.DrawArrays(PrimitiveType.Lines, 0, 4);
            GL.Enable(EnableCap.DepthTest);

            window.SwapBuffers();
        };

        window.UpdateFrame += args =>
        {
            var dt = (float)args.Time;
            var input = window.KeyboardState;
            // Exit
            if (input.IsKeyDown(Keys.Escape))
                window.Close();
            // Movement
            if (input.IsKeyDown(Keys.W)) camera.Position += camera.Front * camera.Speed * dt;
            if (input.IsKeyDown(Keys.S)) camera.Position -= camera.Front * camera.Speed * dt;
            var right = Vector3.Cross(camera.Front, Vector3.UnitY).Normalized();
            if (input.IsKeyDown(Keys.D)) camera.Position += right * camera.Speed * dt;
            if (input.IsKeyDown(Keys.A)) camera.Position -= right * camera.Speed * dt;
            if (input.IsKeyDown(Keys.Space)) camera.Position += Vector3.UnitY * camera.Speed * dt;
            if (input.IsKeyDown(Keys.LeftControl)) camera.Position -= Vector3.UnitY * camera.Speed * dt;
            // Rotation
            if (input.IsKeyDown(Keys.Left)) camera.Yaw -= camera.Sensitivity * dt;
            if (input.IsKeyDown(Keys.Right)) camera.Yaw += camera.Sensitivity * dt;
            if (input.IsKeyDown(Keys.Up)) camera.Pitch += camera.Sensitivity * dt;
            if (input.IsKeyDown(Keys.Down)) camera.Pitch -= camera.Sensitivity * dt;
            camera.Pitch = MathHelper.Clamp(camera.Pitch, -89f, 89f);
        };

        window.Run();
    }

    static void CheckShaderCompile(int shader)
    {
        GL.GetShader(shader, ShaderParameter.CompileStatus, out var success);
        if (success == 0)
            Console.WriteLine(GL.GetShaderInfoLog(shader));
    }

    static void CheckProgramLink(int program)
    {
        GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var success);
        if (success == 0)
            Console.WriteLine(GL.GetProgramInfoLog(program));
    }
    
    /// <summary>
    /// Generates meshes for all chunks in the world, creating VAOs and buffer objects.
    /// </summary>
    private static List<(ChunkPosition Position, int Vao, int IndexCount)> SetupChunkMeshes(World world, MeshBuilder meshBuilder)
    {
        var list = new List<(ChunkPosition Position, int Vao, int IndexCount)>();
        foreach (var (pos, voxels) in world.GetChunks())
        {
            var chunk = world.GetOrCreateChunk(pos);
            var mesh = meshBuilder.GenerateMesh(chunk);
            int vao = GL.GenVertexArray();
            GL.BindVertexArray(vao);
            int vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, mesh.Vertices.Length * sizeof(float), mesh.Vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);
            int nbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, nbo);
            GL.BufferData(BufferTarget.ArrayBuffer, mesh.Normals.Length * sizeof(float), mesh.Normals, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(1);
            int abo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, abo);
            GL.BufferData(BufferTarget.ArrayBuffer, mesh.AmbientOcclusion.Length * sizeof(float), mesh.AmbientOcclusion, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(2);
            int cbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, cbo);
            GL.BufferData(BufferTarget.ArrayBuffer, mesh.Colors.Length * sizeof(float), mesh.Colors, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(3);
            int ebo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, mesh.Indices.Length * sizeof(uint), mesh.Indices, BufferUsageHint.StaticDraw);
            list.Add((pos, vao, mesh.Indices.Length));
        }
        return list;
    }
}

/// <summary>
/// Simple first-person camera supporting position, orientation (yaw/pitch), and movement controls.
/// </summary>
class Camera
{
    public Vector3 Position;
    public float Pitch;
    public float Yaw = -90f;
    public float Speed = 4f;
    public float Sensitivity = 45f;

    /// <summary>
    /// Initializes a new camera at the given position.
    /// </summary>
    /// <param name="position">Initial world-space position of the camera.</param>
    public Camera(Vector3 position)
    {
        Position = position;
    }

    /// <summary>
    /// Gets the normalized forward vector based on the camera's yaw and pitch.
    /// </summary>
    public Vector3 Front => new Vector3(
        MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch)),
        MathF.Sin(MathHelper.DegreesToRadians(Pitch)),
        MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch))
    ).Normalized();

    /// <summary>
    /// Builds a view matrix looking from the camera's position along its forward vector.
    /// </summary>
    /// <returns>A view transform matrix.</returns>
    public Matrix4 GetViewMatrix()
        => Matrix4.LookAt(Position, Position + Front, Vector3.UnitY);
}
