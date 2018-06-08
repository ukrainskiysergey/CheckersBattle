using System;
using UnityEngine;

public class Piece : ICloneable
{
    public Piece(Transform gameObject)
    {
        this.gameObject = gameObject;
        this.owner = gameObject.GetComponentInParent<Player>();
        this.position = gameObject.position;
        this.direction = Vector3.zero;
        this.velocity = 0.0f;
    }

    public object Clone()
    {
        return MemberwiseClone();
    }

    public Transform gameObject;
    public Player owner;
    public Vector3 position;
    public Vector3 direction;
    public float velocity;
}


