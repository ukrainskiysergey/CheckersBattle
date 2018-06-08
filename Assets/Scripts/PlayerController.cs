using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class PlayerController
{
    protected PlayerController(Player player)
    {
        this.player = player;
    }

    public abstract Move? Update();
    protected Player player;
}
