using Unity.Mathematics;

public class MathHelper
{
    public static float GetHeading(float3 objPos, float3 targetPos)
    {
        var x = objPos.x - targetPos.x;
        var y = objPos.z - targetPos.z;
        return math.atan2(x, y) + math.PI;
    }
}
