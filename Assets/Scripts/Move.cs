using System;
using UnityEngine;

public struct Move
{
    public Move(Transform piece, Vector3 direction, float velocity)
    {
        this.piece = piece;
        this.direction = direction;
        this.velocity = velocity;
    }

    public Transform piece;
    public Vector3 direction;
    public float velocity;
}

