using System;
using LendingLibrary.Models;
using Swashbuckle.Swagger;

namespace LendingLibrary
{
    public class AddSchemaExamples : ISchemaFilter
    {
        private BookDTO book1 = new BookDTO
        {
            ID = 1,
            Author = "Terry Jones",
            Title = "Who Killed Chaucer?",
            Rating = 4,
            Genre = "History",
            ISBN = "0312335873",
            OwnerId = "someuser-guid"
        };

        public void Apply(Schema schema, SchemaRegistry schemaRegistry, Type type)
        {
            if (type == typeof(BookDTO))
            {
                schema.example = book1;
            }
            else if (type == typeof(WrappedUnauthorizedApiError))
            {
                schema.example = new WrappedUnauthorizedApiError("You must log in to use this API");
            }
            else if (type == typeof(WrappedForbiddenApiError))
            {
                schema.example = new WrappedForbiddenApiError("You are not allowed to view that resource");
            }
            else if (type == typeof(WrappedNotFoundApiError))
            {
                schema.example = new WrappedNotFoundApiError("That resource was not found");
            }
        }
    }
}
