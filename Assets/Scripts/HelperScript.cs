using System.Collections;
using System.Collections.Generic;
using System;

public enum PlayState
{
    Wait,Move,Win,Lose,Pause
}


public enum TileKind
{
    Blank,Normal,Breakable,Lock,Concrete,Slime
}


public class Tags
{
    public static string Board = "Board";
    public static string MatchFinder = "MatchFinder";
    public static string ScoreManager = "ScoreManager";
    public static string Color = "Color";
    public static string AudioController = "AudioController";
}//class




