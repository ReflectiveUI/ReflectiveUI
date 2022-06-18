
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace QuickApp.Blazor.CodeComponents;

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
            var i = 2;
            builder.OpenElement(i++, "div");
            builder.AddAttribute(i++, "class", "blazor-error-boundary");

            if (_showStackTrace)
            {
                builder.OpenElement(i++, "div");
                builder.AddAttribute(i++, "class", "blazor-error-boundary--trace");
                foreach (var line in CurrentException.ToString().Split(Environment.NewLine))
                {
                    builder.AddContent(i++, line);
                    builder.AddMarkupContent(i++, "<br>");
                }
                builder.CloseElement();
            }
            else
            {
                builder.OpenElement(i++, "div");
                builder.AddAttribute(i++, "class", "blazor-error-boundary--message");
                builder.AddContent(i++, CurrentException.Message);
                builder.CloseElement();
            }

            builder.OpenElement(i++, "div");
            builder.AddAttribute(i++, "class", "blazor-error-boundary--options");
            builder.OpenElement(i++, "button");
            builder.AddAttribute(i++, "class", "btn btn-dark me-2");
            builder.AddAttribute(i++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, () => _showStackTrace = !_showStackTrace));
            builder.AddContent(i++, "Show Stack Trace");
            builder.CloseElement();
            builder.OpenElement(i++, "button");
            builder.AddAttribute(i++, "class", "btn btn-warning");
            builder.AddAttribute(i++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, Recover));
            builder.AddContent(i++, "Continue");
            builder.CloseElement();
            builder.CloseElement();

            builder.CloseElement();
        }
    }
}