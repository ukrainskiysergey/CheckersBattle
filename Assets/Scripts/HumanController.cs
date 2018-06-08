using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanController : PlayerController
{
    public float maxDistance = 2.0f;

    public HumanController(Player player)
        : base(player)
    {
    }

    public override Move? Update()
    {
        if (Input.GetMouseButtonDown(0) && !dragging)
        {
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                if (hit.transform.IsChildOf(player.transform))
                {
                    draggedPiece = hit.transform;
                    StartDragging();
                }
            }

        }

        if (Input.GetMouseButtonUp(0))
            StopDragging();

        if (dragging)
        {
            if (Speed <= 1.0f)
                startTime = Time.time;

            if (Distance >= maxDistance)
            {
                StopDragging();
                return MakeMove();
            }
        }

        return null;
    }

    private void StartDragging()
    {
        Camera.main.GetComponent<OrbitCamera>().locked = true;
        dragging = true;
        startTime = Time.time;
        startPoint = ClickPoint;
    }

    private void StopDragging()
    {
        Camera.main.GetComponent<OrbitCamera>().locked = false;
        dragging = false;
    }

    private Vector3 Direction
    {
        get { return ClickPoint - startPoint; }
    }

    private float Distance
    {
        get { return Direction.magnitude; }
    }

    private float Speed
    {
        get { return Distance / (Time.time - startTime); }
    }

    private Move MakeMove()
    {
        return new Move(draggedPiece, Direction.normalized, Speed);
    }

    private Vector3 ClickPoint
    {
        get
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float distance;
            zeroPlane.Raycast(ray, out distance);
            return ray.GetPoint(distance);
        }
    }

    private bool dragging = false;
    private Transform draggedPiece;
    private Vector3 startPoint;
    private float startTime;
    private Plane zeroPlane = new Plane(Vector3.up, Vector3.zero);
}
