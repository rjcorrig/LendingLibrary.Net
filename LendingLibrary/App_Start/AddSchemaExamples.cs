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
        }
    }
}
