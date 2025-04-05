using System.Collections.Generic;

public static class BirdTypeUtil
{
    public static string GetBirdPinName(string birdName)
    {
        switch (birdName)
        {
            case "Pigeon": return "PigeonPin";
            case "Barn Owl": return "BarnOwlPin";
            case "African Silverbill": return "African Silverbill";
            case "House Sparrow": return "housesparrow";
            case "Hawaiian Duck": return "Hawaiian Duck";
            case "Red Junglefowl": return "redfowl";
            case "Iiwi": return "honeycreeper";
            case "White Tern": return "White Tern";
            default: return null;
        }
    }
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
            case "Red Junglefowl": return BirdType.RedFowl;
            case "Iiwi": return BirdType.HoneyCreeper;
            case "White Tern": return BirdType.WhiteTern;
            default: return BirdType.None;
        }
    }
}