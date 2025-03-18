using System.Collections.Generic;

public static class BirdTypeUtil
{
    public static BirdType GetBirdType(string birdName)
    {
        switch (birdName)
        {
            case "Pigeon": return BirdType.Pigeon;
            case "Barn Owl": return BirdType.BarnOwl;
            case "African Silverbill": return BirdType.AfricanSilverbill;
            case "House Sparrow": return BirdType.HouseSparrow;
            case "Hawaiian Duck": return BirdType.HawaiianDuck;
            case "Kalij Pheasant": return BirdType.KalijPheasant;
            case "Redfowl": return BirdType.RedFowl;
            case "Iiwi": return BirdType.HoneyCreeper;
            default: return BirdType.None;
        }
    }
}