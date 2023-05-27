using System.Windows.Forms;

namespace PyroProblem
{
    public static class Validator
    {
        public static bool IsDouble(params string[] values)
        {
            foreach (string value in values)
            {
                if (!double.TryParse(value, out _))
                {
                    return false;
                }
            }

            return true;
        }
        
        public static bool IsInt(params string[] values)
        {
            foreach (string value in values)
            {
                if (!int.TryParse(value, out _))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsTextEmpty(params string[] values)
        {
            foreach (string value in values)
            {
                if (value == string.Empty)
                {
                    return true;
                }
            }
            return false;
        }
    }
}