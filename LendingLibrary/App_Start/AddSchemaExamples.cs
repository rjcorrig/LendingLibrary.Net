using System;
using LendingLibrary.Models;
using Swashbuckle.Swagger;

namespace LendingLibrary
{
    public class AddSchemaExamples : ISchemaFilter
    {
        public void Apply(Schema schema, SchemaRegistry schemaRegistry, Type type)
        {
            switch (type.Name)
            {
                case nameof(BookDTO):
                    schema.example = new BookDTO
                    {
                        ID = 1,
                        Author = "Terry Jones",
                        Title = "Who Killed Chaucer?",
                        Rating = 4,
                        Genre = "History",
                        ISBN = "0312335873",
                        OwnerId = "someuser-guid"
                    };
                    break;
                case nameof(WrappedUnauthorizedApiError):
                    schema.example = new WrappedUnauthorizedApiError("You must log in to use this API");
                    break;
                case nameof(WrappedForbiddenApiError):
                    schema.example = new WrappedForbiddenApiError("You are not allowed to view that resource");
                    break;
                case nameof(WrappedNotFoundApiError):
                    schema.example = new WrappedNotFoundApiError("That resource was not found");
                    break;
            }
        }
    }
}
