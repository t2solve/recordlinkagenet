namespace RecordLinkageNet.Util
{
    public static class NumberNormalizeHelper
    {
        public static double NormalizeNumberToRange0to1(double value, double max, double min = 0)
        {
            if (value > max)
                throw new System.ArgumentException("value is greater than max");

            if (value < min)
                throw new System.ArgumentException("value is less than min");

            if(min>max)
                throw new System.Exception("min is greater than max");

            return (value - min) / (max - min);
        }

        // overload for float values
        public static float NormalizeNumberToRange0to1(float value, float max, float min = 0)
        {
            if (value > max)
                throw new System.ArgumentException("value is greater than max");

            if (value < min)
                throw new System.ArgumentException("value is less than min");

            if (min > max)
                throw new System.Exception("min is greater than max");

            return (value - min) / (max - min);
        }
    }
}
