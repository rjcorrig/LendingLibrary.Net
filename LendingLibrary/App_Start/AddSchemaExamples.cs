using System;
using LendingLibrary.Api.Models;
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
                case nameof(UnauthorizedApiError):
                    schema.example = new UnauthorizedApiError("You must log in to use this API");
                    break;
                case nameof(ForbiddenApiError):
                    schema.example = new ForbiddenApiError("You are not allowed to view that resource");
                    break;
                case nameof(NotFoundApiError):
                    schema.example = new NotFoundApiError("That resource was not found");
                    break;
            }
        }
    }
}
