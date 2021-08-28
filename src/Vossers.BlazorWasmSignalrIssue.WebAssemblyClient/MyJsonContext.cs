using System.Text.Json.Serialization;
using Vossers.BlazorWasmSignalrIssue.Shared;

namespace Vossers.BlazorWasmSignalrIssue.WebAssemblyClient
{

    [JsonSerializable(typeof(ImmutableViewModel))]
    internal partial class MyJsonContext : JsonSerializerContext
    {

    }
}