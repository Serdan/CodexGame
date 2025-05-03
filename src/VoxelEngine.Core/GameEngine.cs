namespace VoxelEngine.Core;

using System;
using System.Diagnostics;
using System.Threading;

public class GameEngine
{
    private readonly Action<float> _update;
    private readonly Action _render;
    private bool _running;

    public GameEngine(Action<float> update, Action render)
    {
        _update = update;
        _render = render;
    }

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

    public void Stop() => _running = false;
}