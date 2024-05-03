using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAcesUp : CardGame
{
    /// <summary>
    /// This code currently in Under heavy development. The functionality in "AcesUp.cs" is being refactored into "CardGame.cs" and this cs
    /// The code will be split into "CardGame" having functionality that remains across all Card Games, and the Games.cs's retaining logic unique to 
    /// its own game's rules
    /// </summary>

    protected override void SlotsToUse()
    {
        base.SlotsToUse();
        CardSlots = null;
        CardSlots = new List<string>[] {slot0, slot1, slot2, slot3};
    }
}
