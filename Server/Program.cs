using tic_tac_toe.backend;
using tic_tac_toe.backend.Models;
using tic_tac_toe.backend.Services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddSignalR();

builder.Services.Configure<RoomDbSettings>(builder.Configuration.GetSection("Tic-Tac-Toe.Rooms"));
builder.Services.AddSingleton<RoomService>();

builder.Services.Configure<StoryDbSettings>(builder.Configuration.GetSection("Tic-Tac-Toe.Stories"));
builder.Services.AddSingleton<StoryService>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
        });
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseCors();

app.UseAuthorization();

app.MapRazorPages();
app.MapHub<TicTacToeHub>("/gameHub");

app.Run();
