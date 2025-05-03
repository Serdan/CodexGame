namespace VoxelEngine.Core;

using System;
using System.Diagnostics;
using System.Threading;

/// <summary>
/// A simple game engine loop that invokes update and render actions at a fixed interval.
/// </summary>
public class GameEngine
{
    private readonly Action<float> _update;
    private readonly Action _render;
    private bool _running;

    /// <summary>
    /// Initializes a new instance of the GameEngine class.
    /// </summary>
    /// <param name="update">Callback invoked each frame with delta time (seconds).</param>
    /// <param name="render">Callback invoked each frame to render.</param>
    public GameEngine(Action<float> update, Action render)
    {
        _update = update;
        _render = render;
    }

    /// <summary>
    /// Starts the game loop, repeatedly calling update and render until stopped.
    /// </summary>
    public void Run()
    {
        var stopwatch = Stopwatch.StartNew();
        var last = stopwatch.ElapsedMilliseconds;
        _running = true;
        while (_running)
        {
            var now = stopwatch.ElapsedMilliseconds;
            var delta = (now - last) / 1000f;
            last = now;
            _update(delta);
            _render();
            Thread.Sleep(100);
        }
    }

    /// <summary>
    /// Stops the game loop.
    /// </summary>
    public void Stop() => _running = false;
}