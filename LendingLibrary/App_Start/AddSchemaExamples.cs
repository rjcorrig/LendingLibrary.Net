using System;
using System.Net;
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
                case nameof(UnauthorizedApiError):
                    var unauth = new UnauthorizedApiError("You must log in to use this API");
					unauth.Details = new ApiError[] {
						new ApiError {
							Code = HttpStatusCode.InternalServerError.ToString(),
							Message = "Something happened in the auth function"
						}
					};
					unauth.InnerError = new InnerApiError
					{
						Code = "BadPassword",
						InnerError = new InnerApiError
						{
							Code = "BlankPassword"
						}
					};
					schema.example = unauth;
					break;
                case nameof(ForbiddenApiError):
					var forbidden = new ForbiddenApiError("You are not allowed to view that resource");
					forbidden.Target = "Books";
					schema.example = forbidden;
					break;
                case nameof(NotFoundApiError):
                    schema.example = new NotFoundApiError("That resource was not found");
                    break;
            }
        }
    }
}
