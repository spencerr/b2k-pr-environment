using Microsoft.AspNetCore.JsonPatch;

namespace Shared.Common.Extensions;

public static class JsonPatchDocumentExtensions
{

    public static bool ApplyTo<T>(this JsonPatchDocument<T> patchDocument, T dto, out Result result) where T : class
    {
        var errors = new List<Error>();
        patchDocument.ApplyTo(dto, error =>
        {
            errors.Add(new Error(error.ErrorMessage));
        });

        result = Result.Ok().WithErrors(errors);

        return errors.Count == 0;
    }

}
