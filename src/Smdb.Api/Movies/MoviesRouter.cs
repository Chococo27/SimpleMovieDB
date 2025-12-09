
namespace Smdb.Api.Movies;

using Shared.Http;

//Este router recive moviesController como dependencia
//Utiliza parametrized route machine y posee 5 rutas
//MapGet / para leer movies
//MapPost / para leer body como texto y llama a moviesController.CreateMovie para atender el request,...
//MapGet /:id para leer movies
//MapPut /:id para leer body como texto y llama a moviesController.UpdateMovie para atender el request y
//MapDelete /:id llama a moviesController DeleteMovie para atender el request
public class MoviesRouter : HttpRouter
{
	public MoviesRouter(MoviesController moviesController)
	{
		UseParametrizedRouteMatching();

		// GET /api/v1/movies/
		MapGet("/", moviesController.ReadMovies);

		// POST /api/v1/movies/
		MapPost("/", HttpUtils.ReadRequestBodyAsText, moviesController.CreateMovie);

		// GET /api/v1/movies/1
		MapGet("/:id", moviesController.ReadMovie);

		// PUT /api/v1/movies/1
		MapPut("/:id", HttpUtils.ReadRequestBodyAsText, moviesController.UpdateMovie);

		// DELETE /api/v1/movies/1
		MapDelete("/:id", moviesController.DeleteMovie);
	}
}