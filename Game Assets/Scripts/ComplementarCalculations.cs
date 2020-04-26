using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplementarCalculations
{

    /* Calculations for a parabolic movement */
    public static Vector3 CalculateParabola(Vector3 start, Vector3 vertex)
    {
        //Gittata
        float half_range = Mathf.Sqrt((start.x - vertex.x)*(start.x - vertex.x) + (start.z - vertex.z)*(start.z - vertex.z));
        //Parabola's parameters
        float a, c;
        //Angle of parabolic movement
        float angle;
        //Buffers
        float velocity_module;
        Vector3 velocity_vers;
        Quaternion rotation_buffer;

        //Calculation of parabola's parameters
        c = vertex.y - start.y;
        a = -c / (half_range * half_range);
        //Calculation of motion angle through derivative
        angle = Mathf.Atan(2*a* half_range);

        //Module of velocity vector trough parabolic movement formulas
        velocity_module = Mathf.Sqrt(2.0f * half_range * 9.81f / Mathf.Abs(Mathf.Sin(2.0f * angle)));

        //Velocity Versor (unit vector)
        velocity_vers = vertex - start;
        velocity_vers.y = 0.0f;
        velocity_vers.Normalize();
        rotation_buffer = Quaternion.AngleAxis(angle*180.0f/3.1415f, Vector3.Cross(velocity_vers, Vector3.up));

        velocity_vers = rotation_buffer * velocity_vers;

        //Store result
        return velocity_vers * velocity_module + Vector3.up * 9.80665f;
    }
}
