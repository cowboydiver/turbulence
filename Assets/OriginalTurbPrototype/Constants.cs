using UnityEngine;

public static class Constants
{
    public const string MenuScene = "MenuScene";
    public const string FilamentScene = "FilamentScene";

    public const string FilamentLayerName = "FilamentLayer";

    public const string AssetBundleFileExtension = ".unity3d";

    public const string ParseFilamentClassName = "GD_Turbulence_Filament";
    public const string ParseTimeSliceClassName = "GD_Turbulence_TimeSlice";
    public const string ParseSimulationClassName = "GD_Turbulence_Simulation";
    public const string ParseUserFilamentClassName = "GD_Turbulence_UserFilament";
    public const string ParseUserInteractions = "GD_Turbulence_UserInteraction";

    public const int MaxSimulationIndex = 2;
    public const int MaxTimeSliceIndex = 69;
    public const int MaxComponentIndex = 5;

    public const int MaxFilamentsFromTimeSliceOverview = 30;
}