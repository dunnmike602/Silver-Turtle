using TurtleGraphics.Constants;

namespace TurtleGraphics.Maths
{
    public static class ExpressionComparerHelper
    {
        public static bool CheckCondition(int lhsValue, string @operator, int rhsValue)
        {
            var conditionIsTrue = false;

            switch (@operator)
            {
                case GlobalConstants.LessThan:
                    conditionIsTrue = (lhsValue < rhsValue);
                    break;
                case GlobalConstants.GreaterThan:
                    conditionIsTrue = (lhsValue > rhsValue);
                    break;
                case GlobalConstants.Equal:
                    conditionIsTrue = (lhsValue == rhsValue);
                    break;
                case GlobalConstants.NotEqual:
                    conditionIsTrue = (lhsValue != rhsValue);
                    break;
                case GlobalConstants.LessThanOrEqual:
                    conditionIsTrue = (lhsValue <= rhsValue);
                    break;
                case GlobalConstants.GreaterThanOrEqual:
                    conditionIsTrue = (lhsValue >= rhsValue);
                    break;
            }

            return conditionIsTrue;
        }
    }
}
