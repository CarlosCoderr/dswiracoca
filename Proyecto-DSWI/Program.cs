using Proyecto_DSWI.Data;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// Repositorios (DI)
builder.Services.AddScoped<UsuarioRepository>();
builder.Services.AddScoped<VoluntarioRepository>();
builder.Services.AddScoped<OrganizacionRepository>();
builder.Services.AddScoped<HabilidadRepository>();
builder.Services.AddSession();
builder.Services.AddScoped<EventoRepository>();
builder.Services.AddScoped<CategoriaEventoRepository>();
builder.Services.AddScoped<EventoDetalleRepository>();

builder.Services.AddScoped<CrearEventoRepository>();

builder.Services.AddScoped<MisEventosRepository>();
builder.Services.AddScoped<MisInscripcionesRepository>();

builder.Services.AddScoped<TipoOrganizacionRepository>();

builder.Services.AddScoped<DistritoRepository>();

builder.Services.AddScoped<PerfilRepository>();


// Session (Wizard Registro)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//  Necesario para Registro multi-paso
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
