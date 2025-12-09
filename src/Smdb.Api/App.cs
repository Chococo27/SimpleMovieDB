namespace Smdb.Api;

using Shared.Http;
using Smdb.Api.Movies;
using Smdb.Core.Movies;

public class App : HttpServer
{
	public override void Init()
	{
		var db = new MemoryDatabase(); //Inisializa el database
		var movieRepo = new MemoryMovieRepository(db);//MemoryDatabase se pasa al repositorio
		var movieServ = new DefaultMovieService(movieRepo);//Repositorio se pasa como dependencia a Service
		var movieCtrl = new MoviesController(movieServ);//Service se pasa como dependencia al Controller
		var movieRouter = new MoviesRouter(movieCtrl);//Controller se pasa como dependencia al Router
		var apiRouter = new HttpRouter();//Separa todas las rutas bajo su API

		//Routers de movies
		router.Use(HttpUtils.StructuredLogging);
		router.Use(HttpUtils.CentralizedErrorHandling);
		router.Use(HttpUtils.AddResponseCorsHeaders);
		router.Use(HttpUtils.DefaultResponse);
		router.Use(HttpUtils.ParseRequestUrl);//Obsoleto, C# ya lo hace default
		router.Use(HttpUtils.ParseRequestQueryString);//Obsoleto, C# ya lo hace default
		router.UseParametrizedRouteMatching();

		router.UseRouter("/api/v1", apiRouter);
		apiRouter.UseRouter("/movies", movieRouter);
	}

}