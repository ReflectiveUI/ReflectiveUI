using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using ReflectiveUI.Core.ObjectGraph.Nodes;

namespace ReflectiveUI.Blazor.Shared;

public class ReflectiveUIRouter : IComponent, IHandleAfterRender, IDisposable
{
    RenderHandle _renderHandle;
    bool _navigationInterceptionEnabled;
    string? _location;

    [Inject] private NavigationManager? NavigationManager { get; set; }
    [Inject] private INavigationInterception? NavigationInterception { get; set; }
    [Inject] private BlazorReflectedStateRoutingPolicy? StateTransformer { get; set; }

    [Parameter] public RenderFragment<IInteractNode>? Found { get; set; }
    [Parameter] public RenderFragment? NotFound { get; set; }

    public void Attach(RenderHandle renderHandle)
    {
        _renderHandle = renderHandle;
        _location = NavigationManager?.Uri;
        NavigationManager!.LocationChanged += HandleLocationChanged;
    }

    public Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        Refresh();

        return Task.CompletedTask;
    }

    public Task OnAfterRenderAsync()
    {
        if (!_navigationInterceptionEnabled)
        {
            _navigationInterceptionEnabled = true;
            return NavigationInterception!.EnableNavigationInterceptionAsync();
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        NavigationManager!.LocationChanged -= HandleLocationChanged;
    }

    private void HandleLocationChanged(object? sender, LocationChangedEventArgs args)
    {
        _location = args.Location;
        Refresh();
    }

    private void Refresh()
    {
        if (_location is null)
            return;

        var relativeUri = "/" + NavigationManager!.ToBaseRelativePath(_location);
        var parameters = ParseQueryString(relativeUri);

        if (relativeUri.IndexOf('?') > -1)
        {
            relativeUri = relativeUri.Substring(0, relativeUri.IndexOf('?'));
        }

        //var segments = relativeUri.Trim().Split('/', StringSplitOptions.RemoveEmptyEntries);
        var node = StateTransformer!.FindRoutableNode(relativeUri);
        if (node is not null)
        {
            _renderHandle.Render(Found!(node));
        }
        else
        {
            _renderHandle.Render(NotFound!);
        }
    }

    private Dictionary<string, object> ParseQueryString(string uri)
    {
        var querystring = new Dictionary<string, object>();

        foreach (string kvp in uri.Substring(uri.IndexOf("?") + 1).Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries))
        {
            if (kvp != "" && kvp.Contains("="))
            {
                var pair = kvp.Split('=');
                querystring.Add(pair[0], pair[1]);
            }
        }

        return querystring;
    }
}