using System.Collections.Generic;
using UnityEngine;

public static class ExpeditionData
{
    public static int CurrentEncounterIndex = 1;
    public static int TotalEncounters = 3;
    
    public static bool IsExpeditionActive = false;

    public static void StartNewExpedition()
    {
        CurrentEncounterIndex = 1;
        IsExpeditionActive = true;
    }

    public static void AdvanceEncounter()
    {
        CurrentEncounterIndex++;
    }

    public static void EndExpedition()
    {
        IsExpeditionActive = false;
        CurrentEncounterIndex = 1;
    }
}
