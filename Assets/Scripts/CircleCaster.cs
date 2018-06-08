using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CircleCaster
{
    public class Circle
    {
        public Circle(Vector3 position, float radius)
        {
            this.position = position;
            this.radius = radius;
        }


        public Vector3 position;
        public float radius;
    }

    public class HitInfo
    {
        public Vector3 circlePosition;
        public int targetIndex;
        public float distance;
    }

    public class Caster
    {
        public Caster(IEnumerable<Circle> circles)
        {
            this.circles = circles.ToArray();
        }

        public IEnumerable<HitInfo> CastAll(int index, Vector3 direction)
        {
            List<HitInfo> infos = new List<HitInfo>();
            var current = circles[index];

            for (var i = 0; i < circles.Length; i++)
            {
                if (i == index)
                    continue;
                var target = circles[i];
                if (target == null)
                    continue;
                var a = direction.x * direction.x + direction.z * direction.z;
                var b = 2.0f * (direction.x * (current.position.x - target.position.x) + direction.z * (current.position.z - target.position.z));
                var c = (target.position.x - current.position.x) * (target.position.x - current.position.x) + (target.position.z - current.position.z) * (target.position.z - current.position.z) - (current.radius + target.radius) * (current.radius + target.radius);

                var d = b * b - 4.0f * a * c;

                if (d < 0)
                    continue;

                var t1 = (-b - Mathf.Sqrt(d)) / (2.0f * a);
                var t2 = (-b + Mathf.Sqrt(d)) / (2.0f * a);
                var t = t1;

                if (t < 0.0f)
                    t = t2;
                if (t < 0.0f)
                    continue;

                HitInfo info = new HitInfo();
                info.circlePosition = current.position + direction * t;
                info.targetIndex = i;
                info.distance = Vector3.Distance(info.circlePosition, current.position);
                infos.Add(info);
            }

            return infos;
        }

        public HitInfo CastFirst(int index, Vector3 direction)
        {
            return CastAll(index, direction).MinBy(info => info.distance);
        }

        public Circle this[int i]
        {
            get { return circles[i]; }
            set { circles[i] = value; }
        }

        private Circle[] circles;
    }
}
