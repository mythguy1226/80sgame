/// <summary>
/// Abstract level objective class
/// This sets the logic for how to evaluate how much progress a player has achieved in a campaign level
/// </summary>
public abstract class AbsGameObjective
{
    protected float OneStarThreshold;
    protected float TwoStarThreshold;
    protected float ThreeStarThreshold;

    protected abstract int Evaluate(float playerValue);
    public void SetThresholds(float One, float Two, float Three)
    {
        OneStarThreshold = One;
        TwoStarThreshold = Two;
        ThreeStarThreshold = Three;
    }
}

/// <summary>
/// Objective type that measures if the player is over certain thresholds
/// (e.g. score objectives, stun counts, power-up collections)
/// </summary>
public class OverObjective : AbsGameObjective {
    protected override int Evaluate(float playerValue)
    {
        if (playerValue >= ThreeStarThreshold)
        {
            return 3;
        }

        if (playerValue >= TwoStarThreshold)
        {
            return 2;
        }

        if (playerValue >= OneStarThreshold) {
            return 1;
        }

        return 0;
    }
}

/// <summary>
/// Objective that measures if the player is under certain thresholds
/// (e.g. time trials, damage received, stun counts)
/// </summary>
public class UnderObjective : AbsGameObjective
{
    protected override int Evaluate(float playerValue)
    {
        if (playerValue <= ThreeStarThreshold)
        {
            return 3;
        }

        if (playerValue <= TwoStarThreshold)
        {
            return 2;
        }

        if (playerValue <= OneStarThreshold)
        {
            return 1;
        }

        return 0;
    }
}