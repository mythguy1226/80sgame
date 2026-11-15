using GameplayEvents;


/// <summary>
/// Handler for the point manager on when a target resets
/// </summary>
public class TargetResetPointHandler : IGameplayEventHandler<TargetMessage>
{
    PointsManager pm;

    public TargetResetPointHandler(PointsManager pm)
    {
        this.pm = pm;
    }

    public void Handle(TargetMessage message)
    {
        Target t = message.Target;
        if (t.FSM.InUnscorableState())
        {
            return;
        }

        if (message.RelatedPlayer == null)
        {
            return;
        }

        pm.AddRoundPoints(message.RelatedPlayer.Order, t.pointValue * message.RelatedPlayer.scoreController.pointsMod);
    }
}