using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public PlayerType type = PlayerType.PLAYER;
   
}


public enum PlayerType
{
    DM,
    PLAYER
}