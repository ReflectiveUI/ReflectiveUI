
using Humanizer;

namespace ValuedTime.Quick.Host;

public abstract partial record InteractNode
{
    public record ReturnValue(NodeContext Context, IInvokableNode Parent) 
            : InteractNode<IInvokableNode, IInstanceNode>(Context, Parent), ITypedNode
    { 
        public override string Identifier => "Return";
        public DateTime? CalledAtTime { get; set; }

        public void ReleaseLastResult()
        {
            CurrentInstance = null;
            Context.NodeUpdated(this);
        }

        public object? CurrentInstance { get; internal set; }

        public override string DisplayName => "Return Value";// TODO: break this out
        //public override string DisplayName => Parent!.MethodInfo.ReturnTypeCustomAttributes
        //    .GetCustomAttributes(typeof(DisplayAttribute), true)
        //    .Cast<DisplayAttribute>()
        //    .FirstOrDefault()?.Name
        //        ?? Type.Name.Humanize(LetterCasing.Title);

        public bool IsVoid => Type == typeof(void);

        public Type Type
        {
            get
            {
                Type returnType = Parent!.ReturnType;
                if (returnType.IsAssignableTo(typeof(Task)))
                {
                    if (returnType.IsGenericType)
                    {
                        return returnType.GetGenericArguments()[0];
                    }
                    else
                    {
                        return typeof(void);
                    }
                }
                return returnType;
            }
        }
    }
}
