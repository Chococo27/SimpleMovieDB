namespace Smdb.Core.Movies;

using Shared.Http;
using System.Net;

//Utiliza un dependency injection para recibir el repositorio de movies...
//y realizar todas las operaciones de CRUD y validacion
public class DefaultMovieService : IMovieService
{
	private IMovieRepository movieRepository;

	public DefaultMovieService(IMovieRepository movieRepository)
	{
		this.movieRepository = movieRepository;
	}

	//Condiciones de la paginacion
	//No puede tener un page o size menor de 1
	//si no hubo error intenta proveer el paged result usando repositorio
	//Si sale null la operacion fallo
	//de otro modo da pageResult
	public async Task<Result<PagedResult<Movie>>> ReadMovies(int page, int size)
	{
		if (page < 1)
		{
			return new Result<PagedResult<Movie>>(
				new Exception("Page must be >= 1."),
				(int)HttpStatusCode.BadRequest);
		}

		if (size < 1)
		{
			return new Result<PagedResult<Movie>>(
				new Exception("Page size must be >= 1."),
				(int)HttpStatusCode.BadRequest);
		}

		var pagedResult = await movieRepository.ReadMovies(page, size);
		var result = pagedResult == null
			? new Result<PagedResult<Movie>>(new Exception(
					$"Could not read movies from page {page} and size {size}."),
					(int)HttpStatusCode.NotFound)
			: new Result<PagedResult<Movie>>(pagedResult, (int)HttpStatusCode.OK);

		return result;
	}

	//Verifica si la data es valida 
	//si hubo error devuelve los errores de validacion
	//si no hubo error si intenta crear movie usando repositorio
	//Si sale null la operacion fallo (NotFound)
	//de otro modo crea el movie (Created)
	public async Task<Result<Movie>> CreateMovie(Movie newMovie)
	{
		var validationResult = ValidateMovie(newMovie);

		if (validationResult != null) { return validationResult; }

		var movie = await movieRepository.CreateMovie(newMovie);
		var result = movie == null
			? new Result<Movie>(
					new Exception($"Could not create movie {newMovie}."),
					(int)HttpStatusCode.NotFound)
			: new Result<Movie>(movie, (int)HttpStatusCode.Created);

		return result;
	}

	//Pasa el request al repository
	//Si movie es = null significa que no existe o no la encontro con aquel ID
	//De otro modo devuelve el resultado que se pidio
	public async Task<Result<Movie>> ReadMovie(int id)
	{
		var movie = await movieRepository.ReadMovie(id);
		var result = movie == null
			? new Result<Movie>(
					new Exception($"Could not read movie with id {id}."),
					(int)HttpStatusCode.NotFound)
			: new Result<Movie>(movie, (int)HttpStatusCode.OK);

		return result;
	}

	//Verifica si la data es valida 
	//si hubo error devuelve los errores de validacion
	//si no hubo error si intenta crear movie usando repositorio
	//Si sale null la operacion fallo (NotFound)
	//de otro modo hizo update del movie (OK)
	public async Task<Result<Movie>> UpdateMovie(int id, Movie newData)
	{
		var validationResult = ValidateMovie(newData);

		if (validationResult != null) { return validationResult; }

		var movie = await movieRepository.UpdateMovie(id, newData);
		var result = movie == null
			? new Result<Movie>(
					new Exception($"Could not update movie {newData} with id {id}."),
					(int)HttpStatusCode.NotFound)
			: new Result<Movie>(movie, (int)HttpStatusCode.OK);

		return result;
	}

	//Pasa el request al repository
	//Si movie es = null significa que no existe o no la encontro con aquel ID
	//De otro modo borra el movie que se pidio
	public async Task<Result<Movie>> DeleteMovie(int id)
	{
		var movie = await movieRepository.DeleteMovie(id);
		var result = movie == null
			? new Result<Movie>(
					new Exception($"Could not delete movie with id {id}."),
					(int)HttpStatusCode.NotFound)
			: new Result<Movie>(movie, (int)HttpStatusCode.OK);

		return result;
	}

	//Verifica que: lo que se envio no es null, el title no este vacio o sea muy largo,
	//que el año no sea menor de 1888 o mayor del año actual
	//de otro modo todo bueno 
	private static Result<Movie>? ValidateMovie(Movie? movieData)
	{
		if (movieData is null)
		{
			return new Result<Movie>(
				new Exception("Movie payload is required."),
				(int)HttpStatusCode.BadRequest);
		}

		if (string.IsNullOrWhiteSpace(movieData.Title))
		{
			return new Result<Movie>(
				new Exception("Title is required and cannot be empty."),
				(int)HttpStatusCode.BadRequest);
		}

		if (movieData.Title.Length > 256)
		{
			return new Result<Movie>(
				new Exception("Title cannot be longer than 256 characters."),
				(int)HttpStatusCode.BadRequest);
		}

		if (movieData.Year < 1888 || movieData.Year > DateTime.UtcNow.Year)
		{
			return new Result<Movie>(
				new Exception($"Year must be between 1888 and {DateTime.UtcNow.Year}."),
				(int)HttpStatusCode.BadRequest);
		}

		return null;
	}
}