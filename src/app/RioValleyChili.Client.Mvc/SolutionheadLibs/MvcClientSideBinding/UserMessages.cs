namespace RioValleyChili.Client.Mvc.SolutionheadLibs.MvcClientSideBinding
{
    public static class UserMessages
    {
        /// <summary>
        /// Format parameters: {0} = actual return type of expression
        /// </summary>
        public static string ExpressionCannotReturnContainerObject
        {
            get
            {
                return "The expression's return type is a container object. The method expects the expression to return a simple value type such as a primitive, enum, or a System struct (i.e. DateTime). The return type was '{0}'";
            }
        }

        public static string ExpressionMustReturnContainerObject
        {
            get { return "The expression's return type is not a container type. The method expects the expression to return a container object such as a custom class or struct. The return type was '{0}'"; }
        }
    }
}