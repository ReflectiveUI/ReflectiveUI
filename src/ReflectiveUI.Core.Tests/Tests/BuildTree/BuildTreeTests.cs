using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using ReflectiveUI.Core.ObjectGraph;
using ReflectiveUI.Core.ObjectGraph.Nodes;
using VerifyTests;

namespace ReflectiveUI.Core.Tests.Tests.BuildTree;

[UsesVerify]
public class BuildTreeTests
{
    [Fact]
    public Task ActionPropertyNotInvoked() => VerifyObjectGraph(new ObjectWithActions());

    public class ObjectWithActions
    {
        public ObjectWithActions()
        {
            VoidAction = Call;
        }

        public Action VoidAction { get; }
        public Action? NullVoidAction { get; }

        private void Call()
        {
            Called = true;
        }

        public bool Called { get; private set; }
    }

    [Fact]
    public async Task ActionPropertyInvoked()
    {
        var root = new ObjectWithActions();
        var sut = new ReflectedStateGraph<ObjectWithActions>(root);
        sut.Reload();

        var node = (InteractNode.InvokeableDelegate)sut.RootInteractNode!.Children[0].Children[0];
        await node.InvokeAsync();
        sut.Reload();

        await VerifyAppHost(sut);
    }

    [Fact]
    public Task Property() => VerifyObjectGraph(new ObjectWithProperties());

    public class ObjectWithProperties
    {
        public ObjectOneStringProperty? ObjectNullProperty { get; set; }
        public ObjectOneStringProperty ReadWrite { get; set; } = new ObjectOneStringProperty();
        public ObjectOneStringProperty ReadOnly { get; } = new ObjectOneStringProperty();
        public ObjectOneStringProperty ReadInternalWrite { get; internal set; } = new ObjectOneStringProperty();
        public ObjectOneStringProperty ReadPrivateWrite { get; internal set; } = new ObjectOneStringProperty();
    }

    [Fact]
    public Task MethodNotInvoked() => VerifyObjectGraph(new ObjectWithMethod());

    [Fact]
    public async Task MethodInvoked()
    {
        var root = new ObjectWithMethod();
        var sut = new ReflectedStateGraph<ObjectWithMethod>(root);
        sut.Reload();

        var methodNode = (InteractNode.InvokeableMethod)sut.RootInteractNode!.Children[0].Children[0];
        var result = await methodNode.InvokeAsync("Test Parameter Value");
        var methodNode2 = (InteractNode.InvokeableMethod)sut.RootInteractNode!.Children[1].Children[0];
        var result2 = await methodNode2.InvokeAsync("Test Parameter Value 2");

        await VerifyAppHost(sut, new
        {
            InvocationObjectResult = result,
            InvocationValueResult = result2
        });
    }

    public class ObjectWithMethod
    {
        public ObjectOneStringProperty MethodReturnsObject(string name)
        {
            return new ObjectOneStringProperty()
            {
                TestStringProp = name
            };
        }

        public string MethodReturnsValue(string name) => name;
    }

    [Fact]
    public async Task MethodEnumerableInvoked()
    {
        var root = new ObjectWithMethodEnumerable();
        var sut = new ReflectedStateGraph<ObjectWithMethodEnumerable>(root);
        sut.Reload();

        var methodNode = (InteractNode.InvokeableMethod)sut.RootInteractNode!.Children[0].Children[0];
        var result = await methodNode.InvokeAsync("Test Parameter Value");

        await VerifyAppHost(sut, new
        {
            InvocationResult = result
        });
    }

    public class ObjectWithMethodEnumerable
    {
        public List<string> MethodReturnsEnumerable(string name) => DataFactory.CreateStringList();
    }

    [Fact]
    public Task AsyncMethodNotInvoked() => VerifyObjectGraph(new ObjectWithAsyncMethod());

    [Fact]
    public async Task AsyncMethodInvoked()
    {
        var root = new ObjectWithAsyncMethod();
        var sut = new ReflectedStateGraph<ObjectWithAsyncMethod>(root);
        sut.Reload();

        var methodNode = (InteractNode.InvokeableMethod)sut.RootInteractNode!.Children[0].Children[0];
        var result = await methodNode.InvokeAsync("Test Parameter Value");
        var methodNode2 = (InteractNode.InvokeableMethod)sut.RootInteractNode!.Children[1].Children[0];
        var result2 = await methodNode2.InvokeAsync("Test Parameter Value 2");

        await VerifyAppHost(sut, new
        {
            InvocationObjectResult = result,
            InvocationValueResult = result2
        });
    }

    public class ObjectWithAsyncMethod
    {
        public async Task<ObjectOneStringProperty> AsyncMethodReturnsObject(string name)
        {
            await Task.Run(async () => await Task.Delay(1));
            return new ObjectOneStringProperty()
            {
                TestStringProp = name
            };
        }

        public Task<string> AsyncMethodReturnsValue(string name) => Task.FromResult(name);
    }

    [Fact]
    public async Task AsyncEnumerableMethodInvoked()
    {
        var root = new ObjectWithAsyncEnumerableMethod();
        var sut = new ReflectedStateGraph<ObjectWithAsyncEnumerableMethod>(root);
        sut.Reload();

        var methodNode = (InteractNode.InvokeableMethod)sut.RootInteractNode!.Children[0].Children[0];
        var result = await methodNode.InvokeAsync("Test Parameter Value");

        await VerifyAppHost(sut, new
        {
            InvocationResult = result
        });
    }

    public class ObjectWithAsyncEnumerableMethod
    {
        public Task<List<string>> MethodReturnsEnumerable(string name) => Task.FromResult(DataFactory.CreateStringList());
    }

    [Fact]
    public Task Object() => VerifyObjectGraph(new ObjectWithObjectProperty());

    public class ObjectWithObjectProperty
    {
        public ObjectOneStringProperty ObjectProperty { get; set; } = new ObjectOneStringProperty();
        public ObjectOneStringProperty? ObjectNullProperty { get; set; }
    }

    [Fact]
    public Task Value() => VerifyObjectGraph(new ObjectWithValueProperty());

    [Fact]
    public Task SetPropertyValue()
    {
        var root = new ObjectWithValueProperty();
        var sut = new ReflectedStateGraph<ObjectWithValueProperty>(root);
        sut.Reload();

        var propertyNode = (InteractNode.Property)sut.RootInteractNode!.Children[0];
        propertyNode.CurrentInstance = "Test Value Updated";
        var propertyNode2 = (InteractNode.Property)sut.RootInteractNode!.Children[1];
        propertyNode2.CurrentInstance = "Test Value Updated Not Null";

        return VerifyAppHost(sut);
    }

    public class ObjectWithValueProperty
    {
        public string ValueProperty { get; set; } = "Test Value";
        public string? ValueNullProperty { get; set; }
    }

    [Fact]
    public Task EnumerableValue() => VerifyObjectGraph(new ObjectWithValueListProperty());

    public class ObjectWithValueListProperty
    {
        public List<string> ValueListProperty { get; set; } = DataFactory.CreateStringList();
        public List<string>? ValueListNullProperty { get; set; }
    }

    [Fact]
    public Task EnumerableObject() => VerifyObjectGraph(new ObjectWithObjectListProperty());

    public class ObjectWithObjectListProperty
    {
        public List<ObjectOneStringProperty> ObjectListProperty { get; set; } = DataFactory.ObjectList();
        public List<ObjectOneStringProperty>? ObjectListNullProperty { get; set; } = null;

    }

    private static Task VerifyObjectGraph<T>(T root, object? testValues = null) where T : notnull
    {
        var sut = new ReflectedStateGraph<T>(root);
        sut.Reload();
        return VerifyAppHost(sut, testValues);
    }

    private static Task VerifyAppHost<T>(ReflectedStateGraph<T> sut, object? testValues = null) where T : notnull
    {
        object? data = sut.RootInteractNode;
        if (testValues is not null)
        {
            data = new
            {
                TestValues = testValues,
                NodeTree = sut.RootInteractNode
            };
        }
        return Verifier.Verify(data)
            .AddExtraSettings(s =>
            {
                s.Converters.Insert(0, new InteractNodeConverter());
            });
    }
}

public class InteractNodeConverter :
    WriteOnlyJsonConverter<IInteractNode>
{
    Dictionary<Type, ObjectIDGenerator> instanceIds = new();
    ObjectIDGenerator nodeIds = new();

    public override void Write(VerifyJsonWriter writer, IInteractNode node)
    {
        writer.WriteStartObject();

        writer.WriteProperty(node, node.GetType().Name, "NodeType");
        writer.WriteProperty(node, node.DisplayName, nameof(node.DisplayName));
        writer.WriteProperty(node, nodeIds.GetId(node, out _), "Id");

        long? parentId = null;
        if (node.Parent is not null)
            parentId = nodeIds.GetId(node.Parent, out _);
        writer.WriteProperty(node, parentId, "ParentId");

        if (node is InteractNode.Value v)
        {
            writer.WriteProperty(v, v.CurrentInstance, nameof(v.CurrentInstance) + "_Value");
        }
        else if (node is InteractNode.Property p)
        {
            writer.WriteProperty(p, p.IsEditable, nameof(p.IsEditable));
            writer.WriteProperty(p, p.CurrentInstance, nameof(p.CurrentInstance));
        }
        else if (node is InteractNode.ReturnValue ir)
        {
            writer.WriteProperty(ir, ir.CalledAtTime, nameof(ir.CalledAtTime));
        }

        if (node is IInstanceNode i)
        {
            writer.WriteProperty(i, GetId(i.CurrentInstance), nameof(i.CurrentInstance) + "_Id");
        }

        writer.WriteProperty(node, node.Children, nameof(node.Children));

        writer.WriteEndObject();
    }

    public string? GetId(object? obj)
    {
        if (obj is null)
            return null;

        var type = obj.GetType();

        if (!instanceIds.TryGetValue(type, out var typeIdGenerator))
        {
            typeIdGenerator = new ObjectIDGenerator();
            instanceIds.Add(type, typeIdGenerator);
        }

        var id = typeIdGenerator.GetId(obj!, out _);
        return $"{type.Name}_{id}";
    }
}

public static class DataFactory
{
    public static List<string> CreateStringList() => new List<string>()
    {
        "One",
        "Two",
    };

    public static List<ObjectOneStringProperty> ObjectList() => Enumerable.Repeat(new ObjectOneStringProperty(), 3).ToList();
}

public class ObjectOneStringProperty
{
    public string TestStringProp { get; set; } = "Test string prop value";
}


