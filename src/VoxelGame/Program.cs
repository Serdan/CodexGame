using System;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using VoxelEngine.Core;

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
        // Build voxel chunk and mesh
        var chunk = new Chunk();
        for (var x = 0; x < 2; x++)
        for (var y = 0; y < 2; y++)
        for (var z = 0; z < 2; z++)
            chunk.SetVoxel(x, y, z, 1);

        var mesh = new MeshBuilder().GenerateMesh(chunk);

        var gameSettings = GameWindowSettings.Default;
        var nativeSettings = new NativeWindowSettings
        {
            ClientSize = new Vector2i(800, 600),
            Title = "Voxel Game"
        };

        using var window = new GameWindow(gameSettings, nativeSettings);
        // Camera for navigation
        var camera = new Camera(new Vector3(2.0f, 2.0f, 5.0f));
        int vbo = 0, nbo = 0, abo = 0, ebo = 0, vao = 0, shaderProgram = 0;

        window.Load += () =>
        {
            GL.ClearColor(0.1f, 0.1f, 0.1f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            // Compile shaders with lighting and ambient occlusion
            var vsSource = @"
#version 330 core
layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in float aAO;
out vec3 FragPos;
out vec3 Normal;
out float AO;
uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
void main()
{
    FragPos = vec3(model * vec4(aPosition, 1.0));
    Normal = mat3(transpose(inverse(model))) * aNormal;
    AO = aAO;
    gl_Position = projection * view * vec4(FragPos, 1.0);
";
            var fsSource = @"
#version 330 core
in vec3 FragPos;
in vec3 Normal;
in float AO;
out vec4 FragColor;
uniform vec3 lightDir;
uniform vec3 lightColor;
uniform vec3 objectColor;
uniform float ambientStrength;
void main()
{
    // Ambient with occlusion
    vec3 ambient = ambientStrength * lightColor * AO;
    // Diffuse
    vec3 norm = normalize(Normal);
    float diff = max(dot(norm, normalize(-lightDir)), 0.0);
    vec3 diffuse = diff * lightColor;
    vec3 result = (ambient + diffuse) * objectColor;
    FragColor = vec4(result, 1.0);
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
            GL.Uniform3(GL.GetUniformLocation(shaderProgram, "objectColor"), new Vector3(1.0f, 0.5f, 0.2f));
            GL.Uniform1(GL.GetUniformLocation(shaderProgram, "ambientStrength"), 0.3f);

            // Setup buffers
            vao = GL.GenVertexArray();
            vbo = GL.GenBuffer();
            ebo = GL.GenBuffer();
            GL.BindVertexArray(vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, mesh.Vertices.Length * sizeof(float), mesh.Vertices, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, mesh.Indices.Length * sizeof(uint), mesh.Indices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            // Normal attribute
            nbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, nbo);
            GL.BufferData(BufferTarget.ArrayBuffer, mesh.Normals.Length * sizeof(float), mesh.Normals, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            // Ambient Occlusion attribute
            abo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, abo);
            GL.BufferData(BufferTarget.ArrayBuffer, mesh.AmbientOcclusion.Length * sizeof(float), mesh.AmbientOcclusion, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(2, 1, VertexAttribPointerType.Float, false, sizeof(float), 0);
            GL.EnableVertexAttribArray(2);
        };

        window.RenderFrame += args =>
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.UseProgram(shaderProgram);

            var model = Matrix4.Identity;
            var view = camera.GetViewMatrix();
            var projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45f), window.Size.X / (float)window.Size.Y, 0.1f, 100f);

            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "model"), false, ref model);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "view"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(shaderProgram, "projection"), false, ref projection);

            GL.BindVertexArray(vao);
            GL.DrawElements(PrimitiveType.Triangles, mesh.Indices.Length, DrawElementsType.UnsignedInt, 0);

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
