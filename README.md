# Vossers.BlazorWasmSignalrIssue
This repo contains code can be used to reproduce an issue I found with Blazor WebAssembly as a SignalR client where client callbacks don't fire in some circumstances, but only after publishing the WebAssembly project.

Note that I am on a Windows machine using Visual Studio 2022 Preview 3.1 

# Overview
This solution contains 3 projects.

- a SignalR server
- a Blazor WebAssembly Client
- a Shared project

When  you run both the SignalR server and the Blazor WebAssembly Client from Visual Studio everything works as expected. That is, once the browser has loaded the home page of the Blazor WebAssembly project it should output the following:

    Signal callback count: 2
    _immutableViewModel.Items.Count: 3
    _mutableViewModel.Items.Count: 3

However, when you publish the Blazor WebAssembly client from Visual Studio to a local folder and serve it from there while running the SignalR Server project in Visual Studio then a problem will occur, which is that one of the two SignalR Callbacks in the Blazor WebAssembly client does not fire (the other one does fire). In this case the output will be:

    Signal callback count: 1
    _immutableViewModel is null
    _mutableViewModel.Items.Count: 3

To serve the published Blazor WebAssembly project from the local folder it was published to you can use the following approach:

1. Install **http-server** globally using `npm install http-server -g`
2. Run `http-server -a localhost -p 5003` from the directory containing the published files where index.html lives (typically <project>\bin\Release\net6.0\browser-wasm\publish\wwwroot) 
3. Open http://localhost:5003 in the browser (note that you will need to update the CORS config in the SignalR Server project's Startup.cs if you choose to use a different port)

# What is going on?
It looks like the **ImmutableViewModelUpdated** callback that is defined in **Index.razor** never fires if the client project went through the publishing process. It works fine in Visual Studio, whereas **MutableViewModelUpdated** always fires, even when using a "published" client.
  
The main difference between both callbacks is the structure of the parameter passed being in. I am thinking it might be related to the use of Immutable collections / lack of parameterless public constructor so I could work around it by just using Lists, Arrays, Dictionaries etc, but what I don't understand is why it works fine when running from Visual Studio.  

    protected override async Task OnInitializedAsync()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:5001/demo")               
            .Build();

        _hubConnection.On<ImmutableViewModel> ("ImmutableViewModelUpdated", (immutableViewModel) =>
        {
            _callbackCount++;
            _immutableViewModel = immutableViewModel;
            StateHasChanged();
        });

        _hubConnection.On<MutableViewModel> ("MutableViewModelUpdated", (mutableViewModel) =>
        {
            _callbackCount++;
            _mutableViewModel = mutableViewModel;
            StateHasChanged();
        });
            
        await _hubConnection.StartAsync();
    }

# Relevant code files

- [WebAssemblyClient/Pages/Index.razor](https://github.com/jvossers/Vossers.BlazorWasmSignalrIssue/blob/main/src/Vossers.BlazorWasmSignalrIssue.WebAssemblyClient/Pages/Index.razor)
- [SignalrServer/Hubs/DemoHub.cs](https://github.com/jvossers/Vossers.BlazorWasmSignalrIssue/blob/main/src/Vossers.BlazorWasmSignalrIssue.SignalrServer/Hubs/DemoHub.cs)
- [Shared/ImmutableViewModel.cs](https://github.com/jvossers/Vossers.BlazorWasmSignalrIssue/blob/main/src/Vossers.BlazorWasmSignalrIssue.Shared/ImmutableViewModel.cs)
- [Shared/MutableViewModel.cs](https://github.com/jvossers/Vossers.BlazorWasmSignalrIssue/blob/main/src/Vossers.BlazorWasmSignalrIssue.Shared/MutableViewModel.cs)
