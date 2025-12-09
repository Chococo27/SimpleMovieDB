namespace Smdb.Csr;

using Shared.Http;

//Tiene la funcionalidad de crear un Http listener en el puerto y 
//en el host indicado en appsetting.cfg
public class App : HttpServer
{
	public App()
	{
	}

	//Se rescribe el metodo Init para ensamblar nuestro App
	public override void Init()
	{
		router.Use(HttpUtils.StructuredLogging);
		router.Use(HttpUtils.CentralizedErrorHandling);
		router.Use(HttpUtils.AddResponseCorsHeaders);
		router.Use(HttpUtils.DefaultResponse);//Por si se intenta acceder a una ruta que no existe (404)
		router.Use(HttpUtils.ParseRequestUrl);//Obsoleto, C# ya lo hace default
		router.Use(HttpUtils.ParseRequestQueryString);//Obsoleto, C# ya lo hace default
		router.Use(HttpUtils.ServeStaticFiles);//Lo unico que hace es servir Static files
		router.UseSimpleRouteMatching();//Routing machine de MapGet

		//Cuando se quiera acceder al folder de landing page o movies sin poner /index.html 
		//se le redirige para que la pueda acceder
		router.MapGet("/", async (req, res, props, next) => { res.Redirect("/index.html"); await next(); });
		router.MapGet("/movies", async (req, res, props, next) => { res.Redirect("/movies/index.html"); await next(); });
	}
}