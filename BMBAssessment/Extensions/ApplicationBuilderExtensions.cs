using BMBAssessment.API.Middleware;

namespace BMBAssessment.API.Extensions;
public static class ApplicationBuilderExtensions
{
    public static WebApplication UseApiPipeline(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
        if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
        app.UseRouting();
        app.UseCors("Frontend");
        if (app.Configuration.GetValue("HttpsRedirection:Enabled", app.Environment.IsDevelopment()))
            app.UseHttpsRedirection();
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        return app;
    }
}
