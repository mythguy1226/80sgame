public class PlayerScoreController
{
    private int _roundScore;
    private int _totalScore;
    private int _shotsFired;
    private int _shotsLanded;



    public PlayerScoreController()
    {
        _roundScore = 0;
        _totalScore = 0;
        _shotsFired = 0;
        _shotsLanded = 0;
    }

    public void AddRoundPoints(int score)
    {
        _roundScore += score;
        _totalScore += score;
    }

    public void ResetRoundPoints()
    {
        _roundScore = 0;
    }

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

    public int GetRoundScore()
    {
        return _roundScore;
    }
}