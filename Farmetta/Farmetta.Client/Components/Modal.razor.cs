using Microsoft.AspNetCore.Components;

namespace Farmetta.Client.Components;

public partial class Modal : ComponentBase
{
    [Parameter] 
    public string Title { get; set; } = "Modal Title";
    
    public bool IsOpen { get; set; } = false;
    
    [Parameter] 
    public required RenderFragment ChildContent { get; set; }

    public void Open()
    {
        IsOpen = true;
        
        StateHasChanged();
    }
    public void Close()
    {
        IsOpen = false;
        
        StateHasChanged();
    }
    
}