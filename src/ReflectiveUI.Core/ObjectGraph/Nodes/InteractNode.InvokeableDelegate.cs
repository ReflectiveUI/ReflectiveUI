using Humanizer;
using ReflectiveUI.Core.ObjectGraph.Nodes;
using System.Reflection;

namespace ValuedTime.Quick.Host;

public abstract partial record InteractNode
{
    public record InvokeableDelegate(
        NodeContext Context,
        ITypedNode Parent,
        string DisplayName,
        Type DelegateType,
        Func<object?> DelegateGetter) 
            : InteractNode<ITypedNode, IInteractNode>(Context, Parent), IInvokableNode, IInstanceNode
    {
        public override string Identifier { get; } = DelegateType.Name;

        public override string DisplayName { get; } = DisplayName;

        public ReturnValue? LastResultNode => Children.OfType<ReturnValue>().SingleOrDefault();

        public bool IsAvailable => DelegateGetter() is not null;

        public Type Type => DelegateType;

        public Type ReturnType
        {
            get
            {
                if (DelegateType.BaseType != typeof(MulticastDelegate))
                    throw new InvalidOperationException();
                if (DelegateType.Name.StartsWith("Action"))
                {
                    return typeof(void);
                }

                if (DelegateType.Name.StartsWith("Func"))
                {
                    return DelegateType.GetGenericArguments().Last();
                }
                return DelegateType;
            }
        }

        public object? CurrentInstance => DelegateGetter();

        public List<(string?, Type)> GetParameters()
        {
            if (DelegateType.BaseType != typeof(MulticastDelegate))
                throw new InvalidOperationException();

            if (DelegateType.Name.StartsWith("Action"))
            {
                return DelegateType.GetGenericArguments()
                    .Select(x => ((string?)x.Name, x))
                    .ToList();
            }
            if (DelegateType.Name.StartsWith("Func"))
            {
                return DelegateType.GetGenericArguments()
                    .SkipLast(1)
                    .Select(x => ((string?)x.Name, x))
                    .ToList();
            }

            throw new InvalidOperationException("Unkown delegate");
        }

        public async Task<object?> InvokeAsync(params object?[]? parameters)
        {
            var d = (Delegate?)(DelegateGetter.Invoke());
            if (d is null)
                return null;

            object? result;
            if (ReturnType.IsAssignableTo(typeof(Task)))
            {
                var task = (Task)d.Method.Invoke(d.Target, parameters)!;
                await task;
                var resultProperty = task.GetType().GetProperty(nameof(Task<object>.Result));
                if (resultProperty is not null)
                    result = resultProperty!.GetValue(task);
                else
                    result = null;
            }
            else
            {
                result = d.Method.Invoke(d.Target, null);
            }

            LastResultNode!.CalledAtTime = DateTime.Now;
            LastResultNode!.CurrentInstance = result;
            Context.NodeUpdated(this);
            return result;
        }
    }
}
