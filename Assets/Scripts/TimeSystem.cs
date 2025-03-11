using UnityEngine;

public class TimeSystem
{
    private static float Scale = Time.timeScale;
    public static bool Paused { get; private set; } = false;

    public static void Pause()
    {
        if (Paused) return;
        Scale = Time.timeScale;
        Time.timeScale = 0;
        Paused = true;
    }

    public static void Resume()
    {
        if (!Paused) return;
        Time.timeScale = Scale;
        Paused = false;
    }

    public static void TogglePause()
    {
        if (Paused)
            Resume();
        else
            Pause();
    }
}