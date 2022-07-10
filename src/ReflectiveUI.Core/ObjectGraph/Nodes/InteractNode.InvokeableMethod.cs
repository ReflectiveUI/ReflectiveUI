using Humanizer;
using ReflectiveUI.Core.ObjectGraph.Nodes;
using System.Reflection;

namespace ValuedTime.Quick.Host;

public abstract partial record InteractNode
{
    public record InvokeableMethod(
        NodeContext Context,
        Method Parent,
        MethodInfo MethodInfo,
        string DisplayName,
        Func<object?> InstanceGetter) 
            : InteractNode<Method, IInteractNode>(Context, Parent), IInvokableNode
    {
        public override string Identifier => ReturnType.Name;

        public override string DisplayName { get; } = DisplayName;

        public bool IsAvailable => InstanceGetter() is not null;

        public ReturnValue? LastResultNode => Children.OfType<ReturnValue>().SingleOrDefault();

        public Type ReturnType
        {
            get
            {
                Type returnType = MethodInfo.ReturnType;
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

        public List<(string?, Type)> GetParameters()
        {
            return MethodInfo.GetParameters()
                .Select(p => (p.Name, p.ParameterType))
                .ToList();
        }

        public async Task<object?> InvokeAsync(params object?[]? parameters)
        {
            var i = InstanceGetter();
            if (i is null)
                throw new InvalidOperationException("Can't invoke when the target object is null");

            object? result;
            if (MethodInfo.ReturnType.IsAssignableTo(typeof(Task)))
            {
                var task = (Task)MethodInfo.Invoke(i, parameters)!;
                await task;
                var resultProperty = task.GetType().GetProperty(nameof(Task<object>.Result));
                if (resultProperty is not null)
                    result = resultProperty!.GetValue(task);
                else
                    result = null;
            }
            else
            {
                result = MethodInfo.Invoke(i, parameters);
            }

            LastResultNode!.CalledAtTime = DateTime.Now;
            LastResultNode!.CurrentInstance = result;
            Context.NodeUpdated(this);
            return result;
        }
    }
}
