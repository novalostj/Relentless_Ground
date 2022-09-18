namespace Utility
{
    public static class MotionInOneDirection
    {
        public static float FindFinalVelocity(float distance, float initialVelocity, float time)
        {
            float finalVelocity = distance * 2 / time - initialVelocity;
            
            return finalVelocity;
        }
    }
}