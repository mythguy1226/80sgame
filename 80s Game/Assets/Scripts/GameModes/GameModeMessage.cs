namespace GameplayEvents
{
    public struct TargetScoreParameters
    {
        public bool ShowText;
        public bool ShowPlayerColor;

        public TargetScoreParameters(bool showText, bool showPlayerColor)
        {
            this.ShowText = showText;
            this.ShowPlayerColor = showPlayerColor;
        }
    }

    public abstract class AbsGameModeMessage :IGameplayMessage
    {
        public EGameMode GameMode { get; private set; }
        public AbsGameModeMessage(EGameMode gameMode) { GameMode = gameMode; }
    }

    public class ModeConfigurationMessage : AbsGameModeMessage
    {
        public TargetScoreParameters TargetScoreParameters { get; private set; }
        public ModeConfigurationMessage(EGameMode mode, TargetScoreParameters scoreParameters) : base(mode)
        {
            TargetScoreParameters = scoreParameters;
        }
    }
}