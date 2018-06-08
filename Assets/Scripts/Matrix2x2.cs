using UnityEngine;

public struct Matrix2x2
{
    public Matrix2x2(float m00, float m01, float m10, float m11)
    {
        this.m00 = m00;
        this.m01 = m01;
        this.m10 = m10;
        this.m11 = m11;
    }

    public float determinant
    {
        get
        {
            return m00 * m11 - m01 * m10;
        }
    }

    public Matrix2x2 inverse
    {
        get
        {
            var det = determinant;
            return new Matrix2x2(m11 / det, -m01 / det, -m10 / det, m00);
        }
    }

    public static Vector2 operator *(Matrix2x2 mat, Vector2 vec)
    {
        return new Vector2(mat.m00 * vec.x + mat.m01 * vec.y, mat.m10 * vec.x + mat.m11 * vec.y);
    }

    private float m00;
    private float m01;
    private float m10;
    private float m11;
}