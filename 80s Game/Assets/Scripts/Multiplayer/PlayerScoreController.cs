using System.Runtime.InteropServices.WindowsRuntime;

public class PlayerScoreController
{
    // This class encapsulates the accuracy-keeping responsibility for each player object.
    private int _shotsFired;
    private int _shotsLanded;

    public int ShotsFired { get => _shotsFired; }
    public int ShotsLanded { get => _shotsLanded; }

    public int pointsMod = 1;


    public PlayerScoreController()
    {
        _shotsFired = 0;
        _shotsLanded = 0;
    }

    // Record keeping functions for managing accuracy
    public void AddShot()
    {
        _shotsFired++;
    }
    public void AddHit()
    {
        _shotsLanded++;
    }

    public float GetAccuracy()
    {
        if (_shotsFired == 0)
        {
            return 0.0f;
        }
        return _shotsLanded / (float)_shotsFired;
    }
}