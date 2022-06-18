using Microsoft.AspNetCore.Components;
using QuickApp.Blazor.Models;
using ValuedTime.Quick.Host;

namespace QuickApp.Blazor.Pages;

public partial class Index
{
    bool _showSuppressed;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            AppHost.AppUpdated += (sender, args) =>
            {
                InvokeAsync(() =>
                {
                    StateHasChanged();
                    Console.WriteLine("Re-rendered");
                });
            };
            await AppHost.RootInstance.LoadAsync();
            AppHost.Reload();
        }
    }

    //private void TraverseNodesDepthFirst(IInteractNode rootNode, Action<(int Depth, IInteractNode Node)> visitor)
    //{
    //    var appNodes = new Stack<(int Depth, IInteractNode Node)>(new[] { (0, rootNode) });

    //    while (appNodes.Any())
    //    {
    //        var depthAndNode = appNodes.Pop();

    //        visitor(depthAndNode);

    //        var (depth, node) = depthAndNode;
    //        ++depth;

    //        foreach (var n in node.Children)
    //        {
    //            appNodes.Push((depth, n));
    //        }
    //    }
    //}
}


