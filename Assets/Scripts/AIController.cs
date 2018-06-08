using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class AIController : PlayerController
{
    public AIController(Player player)
        : base(player)
    {
        enemy = GameObject.FindObjectsOfType<Player>().Where(x => x != player).First();
        var piece = player.transform.GetChild(0);
        var bounds = piece.GetComponent<MeshFilter>().mesh.bounds.size;
        pieceRadius = Mathf.Max(bounds.x, bounds.y, bounds.z) / 2.0f;
        var material = piece.GetComponent<MeshCollider>().material;
        dynamicFriction = material.dynamicFriction;
    }

    private Move? selectedMove = null;

    public override Move? Update()
    {
        if (worker == null)
        {
            worker = new Thread(Evaluation);
            allPieces = new Piece[player.transform.childCount + enemy.transform.childCount];
            playerPieces = new Transform[player.transform.childCount];
            for (var i = 0; i < playerPieces.Length; i++)
            {
                playerPieces[i] = player.transform.GetChild(i);
                allPieces[i] = new Piece(playerPieces[i]);
            }

            for (var i = 0; i < enemy.transform.childCount; i++)
            {
                allPieces[playerPieces.Length + i] = new Piece(enemy.transform.GetChild(i));
            }

            worker.Start();
        }

        if (!worker.IsAlive)
        {
            worker = null;
            return selectedMove;
        }

        return null;
    }

    private void Evaluation()
    {
        Move? maxMove = null;
        float maxValue = float.MinValue;
        System.Random random = new System.Random();
        var pieceCount = allPieces.Count();
        foreach (var piece in playerPieces)
        {
            for (float angle = 0.0f; angle < 2.0f * Mathf.PI; angle += angleStep)
            {
                var direction = VectorFromAngle(angle);
                for (float velocity = velocityMin; velocity <= velocityMax; velocity += velocityStep)
                {
                    var move = new Move(piece, direction, velocity);
                    var evaluation = Evaluate(move) + ((float)random.NextDouble() * 2.0f - 1.0f) * pieceCount * difficultyError * (1.0f - GameSettings.Instance.Difficulty);
                    if (evaluation > maxValue)
                    {
                        maxMove = move;
                        maxValue = evaluation;
                    }
                }
            }
        }

        selectedMove = maxMove;
    }

    private float Evaluate(Move move)
    {
        var allPieces = this.allPieces.Select(piece => piece.Clone() as Piece).ToList();
        var movePieceIndex = allPieces.FindIndex(piece => piece.gameObject == move.piece);
        allPieces[movePieceIndex].direction = move.direction;
        allPieces[movePieceIndex].velocity = move.velocity;
        var caster = new CircleCaster.Caster(allPieces.Select(x => new CircleCaster.Circle(x.position, pieceRadius)));
        var queue = new Queue<int>();
        queue.Enqueue(movePieceIndex);
        while (queue.Count != 0)
        {
            var currentFigureIndex = queue.Dequeue();
            var currentFigure = allPieces[currentFigureIndex];
            if (currentFigure == null)
                continue;
            var hitInfo = caster.CastFirst(currentFigureIndex, currentFigure.direction);
            if (hitInfo != null && hitInfo.distance > 1e-3)
            {
                var target = allPieces[hitInfo.targetIndex];
                if (hitInfo.distance < DistanceFromVelocity(currentFigure.velocity))
                {
                    currentFigure.position = hitInfo.circlePosition;
                    caster[currentFigureIndex].position = currentFigure.position;

                    var dir = (target.position - currentFigure.position).normalized;
                    var y_new = new Vector2(dir.x, dir.z);
                    var x_new = new Vector2(y_new.y, -y_new.x);
                    var A = new Matrix2x2(x_new.x, y_new.x, x_new.y, y_new.y);
                    var A_inv = A.inverse;

                    var v = new Vector2(currentFigure.direction.x * currentFigure.velocity, currentFigure.direction.z * currentFigure.velocity);
                    var v_new = A_inv * v;
                    var vn0 = new Vector2(v_new.x, v_new.y / 2.0f);
                    var vn1 = new Vector2(0.0f, v_new.y / 2.0f);

                    var v0 = A * vn0;
                    var v1 = A * vn1;

                    var v0_v3 = new Vector3(v0.x, 0.0f, v0.y);
                    var v1_v3 = new Vector3(v1.x, 0.0f, v1.y);

                    currentFigure.direction = v0_v3.normalized;
                    currentFigure.velocity = v0_v3.magnitude;
                    target.direction = v1_v3.normalized;
                    target.velocity = v1_v3.magnitude;

                    queue.Enqueue(hitInfo.targetIndex);
                    queue.Enqueue(currentFigureIndex);
                }
            }
            else
            {
                currentFigure.position += currentFigure.direction * DistanceFromVelocity(currentFigure.velocity);
                if (Mathf.Abs(currentFigure.position.x) > 9.05f || Mathf.Abs(currentFigure.position.z) > 9.05f)
                {
                    allPieces[currentFigureIndex] = null;
                    caster[currentFigureIndex] = null;
                }
            }
        }
        return allPieces.Where(x => x != null).Aggregate(0.0f, (count, piece) =>
        {
            return count + (IsAlliesPiece(piece) ? 1.0f : -1.0f) *
                (1.0f - piece.position.magnitude / 1000.0f);
        });
    }

    private float VelocityFromDistance(float distance, float startVelocity)
    {
        return Mathf.Sqrt(2.0f * Acceleration * distance + startVelocity * startVelocity);
    }

    private float DistanceFromVelocity(float velocity)
    {
        var acceleration = Acceleration;
        // time = (v_end - v_start) / acceleration
        var time = -velocity / acceleration;
        // S = t * V0 + 1/2 * a * t^2
        return time * velocity + 1.0f / 2.0f * acceleration * time * time;
    }

    private Vector3 VectorFromAngle(float angle)
    {
        return new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
    }

    private float Acceleration
    {
        get
        {
            // F_friction = N * friction_coef
            // A_friction = N / mass * friction_coef
            var normalAcceleration = gravity_magnitude;
            var frictionAcceleration = -normalAcceleration * dynamicFriction * 2.0f;  // * 2.0 - the unity's workaround
            return frictionAcceleration;
        }
    }

    private bool IsAlliesPiece(Piece piece)
    {
        return player == piece.owner;
    }

    private bool IsEnemyPiece(Piece piece)
    {
        return enemy == piece.owner;
    }

    private Player enemy;
    private const float angleStep = 0.05f;
    private const float velocityMin = 2.0f;
    private const float velocityMax = 50.0f;
    private const float velocityStep = 2.0f;

    private const float difficultyError = 0.5f;

    private float gravity_magnitude = Physics.gravity.magnitude;

    private Thread worker;
    private Piece[] allPieces;
    private Transform[] playerPieces;
    private float pieceRadius;
    private float dynamicFriction;
}
