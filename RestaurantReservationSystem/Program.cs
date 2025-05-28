using Microsoft.AspNetCore.SignalR;
using RestaurantReservationSystem.Builders;
using RestaurantReservationSystem.Commands;
using RestaurantReservationSystem.Composites;
using RestaurantReservationSystem.Decorators;
using RestaurantReservationSystem.Enums;
using RestaurantReservationSystem.Facades;
using RestaurantReservationSystem.Hubs;
using RestaurantReservationSystem.Managers;
using RestaurantReservationSystem.Models;
using RestaurantReservationSystem.Services;
using RestaurantReservationSystem.Strategies;
using RestaurantReservationSystem.States;

var builder = WebApplication.CreateBuilder(args);

// Services & SignalR
builder.Services.AddControllers().AddNewtonsoftJson(opt =>
    opt.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection voor HubContext
// builder.Services.AddSingleton<IHubContext<ReservationHub>, HubContext<ReservationHub>>();

var app = builder.Build();

// if (app.Environment.IsDevelopment())
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ReservationHub>("/hub/reservations");

// Initialisatie van restaurantstructuur
InitializeRestaurantStructure();
ShowDemoOutput();

app.Run();

// ↓ Restaurantstructuur opzetten (Composite Pattern)
void InitializeRestaurantStructure()
{
    var hoofdzaal = new RestaurantRoom("Hoofdzaal");
    var terras = new RestaurantRoom("Terras");
    var mainBranch = new RestaurantBranch("Almere Centrum");
    mainBranch.AddComponent(hoofdzaal);
    mainBranch.AddComponent(terras);

    var tableManager = TableManager.Instance;
    tableManager.AddTable(new Table { Id = 1, Seats = 6, Room = hoofdzaal });
    tableManager.AddTable(new Table { Id = 2, Seats = 4, Room = terras });

    hoofdzaal.AddTable(tableManager.GetTableById(1));
    terras.AddTable(tableManager.GetTableById(2));
}

// ↓ Laat zien hoe patterns samenwerken
async void ShowDemoOutput()
{
    Console.WriteLine("Restaurant structuur:");
    TableManager.Instance.DisplayStructure();
}

// Builder Pattern + Decorator Pattern
var reservation = new ReservationBuilder()
    .SetGuestName("Anna de Vries")
    .SetDateTime(new DateTime(2025, 11, 28, 23, 59, 0))
    .SetNumberOfGuests(4)
    .SetDetails("Komen misschien een kwartier later")
    .Build();

reservation = new SpecialRequestDecorator(reservation, "1 persoon gluten- en lactosevrij");

// Strategy Pattern
var strategy = new OnlineReservationStrategy();

// Facade Pattern
var success = await new ReservationFacade().CreateReservationAsync(reservation, strategy);
if (!success)
{
    Console.WriteLine("Geen beschikbare tafel voor reservering.");
    return;
}

Console.WriteLine("Reservering succesvol aangemaakt.");
Console.WriteLine(reservation.ToString());

// Command Pattern + Observer Pattern + SignalR
var confirmCommand = new ConfirmReservationCommand(reservation);
confirmCommand.Execute();

Console.WriteLine("Reservering bevestigd. Status: " + reservation.ReservationState.Name);
Console.WriteLine("Bekijk SignalR-hub op /hub/reservations voor realtime updates.");