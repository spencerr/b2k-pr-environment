using System.Net;

namespace Shared.Common.Extensions;

public static class HttpResponseMessageExtensions
{

    public static async Task<Result> GetResult(this HttpResponseMessage message)
    {
        if (message.StatusCode == HttpStatusCode.NoContent)
        {
            return Result.Ok();
        }

        var content = await message.Content.ReadAsStringAsync();
        return message.StatusCode switch
        {
            _ => Result.Fail(content)
        };
    }

    public static async Task<FluentResults.Result<T>> GetResult<T>(this HttpResponseMessage message)
    {
        if (message.StatusCode == HttpStatusCode.OK)
        {
            var response = await System.Text.Json.JsonSerializer.DeserializeAsync<T>(await message.Content.ReadAsStreamAsync(), SisJsonSerialization.Options);
            return Result.Ok(response!);
        }

        return await GetResult(message);
    }
}
