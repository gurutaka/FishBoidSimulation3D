using UnityEngine;
using System.Collections.Generic;

public class Boid : MonoBehaviour
{
    public Simulation simulation { get; set; }
    public Param param { get; set; } //paramのclass
    public Vector3 pos { get; private set; }
    public Vector3 velocity { get; private set; }
    Vector3 accel = Vector3.zero;
    List<Boid> neighbors = new List<Boid>();

    void Start()
    {
        pos = transform.position;
        velocity = transform.forward * param.initSpeed;
    }

    void Update()
    {
        UpdateNeighbors();
        UpdateSeparation();
        UpdateAlignment();
        UpdateCohesion();
        UpdateWalls();
        UpdateMove();
    }

    void UpdateMove()
    {
        var dt = Time.deltaTime;

        velocity += accel * dt;
        var dir = velocity.normalized;
        var speed = velocity.magnitude;
        velocity = Mathf.Clamp(speed, param.minSpeed, param.maxSpeed) * dir;
        pos += velocity * dt;

        var rot = Quaternion.LookRotation(velocity);
        rot = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime*3);
        transform.SetPositionAndRotation(pos, rot);
        accel = Vector3.zero;
    }

    void UpdateWalls()
    {
        if (!simulation) return;

        var scale = param.wallScale * 0.5f;//wallScaleの半分が座標
        accel +=
            CalcAccelAgainstWall(-scale - pos.x, Vector3.right) +
            CalcAccelAgainstWall(-scale - pos.y, Vector3.up) +
            CalcAccelAgainstWall(-scale - pos.z, Vector3.forward) +
            CalcAccelAgainstWall(+scale - pos.x, Vector3.left) +
            CalcAccelAgainstWall(+scale - pos.y, Vector3.down) +
            CalcAccelAgainstWall(+scale - pos.z, Vector3.back);
    }

    Vector3 CalcAccelAgainstWall(float distance, Vector3 dir)
    {
        if (Mathf.Abs(distance) < param.wallDistance)
        {
            return dir * (param.wallWeight / Mathf.Abs(distance / param.wallDistance));
        }
        else
        {
            return Vector3.zero;
        }
    }


    //近傍の場所を算出
    void UpdateNeighbors()
    {
        neighbors.Clear();//list内をすべて削除

        if (!simulation) return;

        var prodThresh = Mathf.Cos(param.neighborFov * Mathf.Deg2Rad);//角度をラジアンに変換
        var distThresh = param.neighborDistance;

        foreach (var other in simulation.boids)
        {
            if (other == this) continue;//自分はスルー

            var to = other.pos - pos;
            var dist = to.magnitude;//他の個体との距離
            if (dist < distThresh)
            {
                var dir = to.normalized;
                var fwd = velocity.normalized;
                var prod = Vector3.Dot(fwd, dir);//正規化したベクトル同士の内積→cosが算出
                if (prod > prodThresh)//cosよりも小さい値が視界範囲にいることになる
                {
                    neighbors.Add(other);
                }
            }
        }
    }

    void UpdateSeparation()
    {
        if (neighbors.Count == 0) return;

        Vector3 force = Vector3.zero;
        foreach (var neighbor in neighbors)
        {
            force += (pos - neighbor.pos).normalized;
        }
        force /= neighbors.Count;

        accel += force * param.separationWeight;//最後に力を渡す
    }

    void UpdateAlignment()
    {
        if (neighbors.Count == 0) return;

        var averageVelocity = Vector3.zero;
        foreach (var neighbor in neighbors)
        {
            averageVelocity += neighbor.velocity;
        }
        averageVelocity /= neighbors.Count;

        accel += (averageVelocity - velocity) * param.alignmentWeight;
    }

    void UpdateCohesion()
    {
        if (neighbors.Count == 0) return;

        var averagePos = Vector3.zero;
        foreach (var neighbor in neighbors)
        {
            averagePos += neighbor.pos;
        }
        averagePos /= neighbors.Count;

        accel += (averagePos - pos) * param.cohesionWeight;
    }

}