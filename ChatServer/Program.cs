using ChatServer.Hubs;
using ChatServer.Repositories;
using CryptoChat.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var _tom = new User
{
    Id = 1,
    Name = "Bob",
    Password = "1234",
    PublicKey = 3
};

var _jerry = new User
{
    Id = 2,
    Name = "Alice",
    Password = "1234",
    PublicKey = 4
};

var userRepository = new UserRepository();
userRepository.AddUser(_tom);
userRepository.AddUser(_tom);



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder => builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.Run();
