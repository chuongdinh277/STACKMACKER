using UnityEngine;
public enum Direct
{
    Forward,
    Back,
    Right,
    Left,
    None
}

public enum PlayerAnimState
{
    Idle = 0,
    Run = 1,
    Win = 2
}

public enum TileType
{
    None,
    Brick,
    Obstacle,
    BridgeStep,
    BrickCorner,
    Win,
    StartPoint
}