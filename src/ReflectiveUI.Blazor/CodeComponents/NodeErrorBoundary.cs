
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace ReflectiveUI.Blazor.CodeComponents;

/// <summary>
/// Captures errors thrown from its child content.
/// </summary>
public class NodeErrorBoundary : ErrorBoundaryBase
{
    [Inject] private IErrorBoundaryLogger? ErrorBoundaryLogger { get; set; }

    private bool _showStackTrace;

    /// <summary>
    /// Invoked by the base class when an error is being handled. The default implementation
    /// logs the error.
    /// </summary>
    protected override async Task OnErrorAsync(Exception exception)
    {
        await ErrorBoundaryLogger!.LogErrorAsync(exception);
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (CurrentException is null)
        {
            builder.AddContent(0, ChildContent);
        }
        else if (ErrorContent is not null)
        {
            builder.AddContent(1, ErrorContent(CurrentException));
        }
        else
        {
            builder.OpenElement(2, "div");
            builder.AddAttribute(3, "class", "blazor-error-boundary");

            if (_showStackTrace)
            {
                builder.OpenElement(4, "div");
                builder.AddAttribute(5, "class", "blazor-error-boundary--trace");
                foreach (var line in CurrentException.ToString().Split(Environment.NewLine))
                {
                    builder.AddContent(6, line);
                    builder.AddMarkupContent(7, "<br>");
                }
                builder.CloseElement();
            }
            else
            {
                builder.OpenElement(8, "div");
                builder.AddAttribute(9, "class", "blazor-error-boundary--message");
                builder.AddContent(10, CurrentException.Message);
                builder.CloseElement();
            }

            builder.OpenElement(11, "div");
            builder.AddAttribute(12, "class", "blazor-error-boundary--options");
            builder.OpenElement(13, "button");
            builder.AddAttribute(14, "class", "btn btn-dark me-2");
            builder.AddAttribute(15, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => _showStackTrace = !_showStackTrace));
            builder.AddContent(16, "Show/Hide Stack Trace");
            builder.CloseElement();
            builder.OpenElement(17, "button");
            builder.AddAttribute(18, "class", "btn btn-warning");
            builder.AddAttribute(19, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, Recover));
            builder.AddContent(20, "Continue");
            builder.CloseElement();
            builder.CloseElement();

            builder.CloseElement();
        }
    }
}