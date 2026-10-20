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