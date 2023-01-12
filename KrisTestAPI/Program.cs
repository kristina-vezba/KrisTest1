using KrisTest.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<KrisTestContext>(options => options
	.UseNpgsql(builder.Configuration.GetConnectionString("KrisTestContextConnection"))
	.UseSnakeCaseNamingConvention()
);

builder.Services.AddHttpContextAccessor();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

using ( var scope = app.Services.CreateScope() )
{
	var db = scope.ServiceProvider.GetRequiredService<KrisTestContext>();
	var migrationCount = db.Database.GetPendingMigrations;
	if (migrationCount != null)
	{
		db.Database.Migrate();
	}
}

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

KrisTestContextSeed.Seed(app);

app.Run();
