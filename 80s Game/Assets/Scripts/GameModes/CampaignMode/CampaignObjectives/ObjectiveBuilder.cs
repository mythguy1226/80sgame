/// <summary>
/// Factory class for objective objects
/// </summary>
public static class ObjectiveFactory
{
    public static AbsGameObjective MakeObjective(ObjectiveType objType)
    {
        switch (objType) {
            case ObjectiveType.RoundCount:
                return new UnderObjective();
            default:
                return new OverObjective();
        }
    }
}