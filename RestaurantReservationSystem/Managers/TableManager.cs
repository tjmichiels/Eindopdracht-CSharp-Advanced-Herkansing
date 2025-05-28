using System.Collections.Concurrent;
using RestaurantReservationSystem.Models;

namespace RestaurantReservationSystem.Managers;

public class TableManager
{
    private static readonly Lazy<TableManager> instance =
        new Lazy<TableManager>(() => new TableManager());

    // Concurrent Collections
    private readonly ConcurrentDictionary<int, Table> _tables = new ConcurrentDictionary<int, Table>();

    public static TableManager Instance => instance.Value;

    private TableManager()
    {
    }

    public void AddTable(Table table)
    {
        _tables.TryAdd(table.Id, table);
    }

    public Table GetAvailableTable(int guests)
    {
        return _tables.Values.FirstOrDefault(t => t.IsAvailable && t.Seats >= guests);
    }

    public Table GetTableById(int id)
    {
        return _tables.TryGetValue(id, out var table) ? table : null;
    }

    public void DisplayStructure()
    {
        var grouped = _tables.Values.GroupBy(t => t.Room?.Name ?? "Onbekende ruimte");
        foreach (var group in grouped)
        {
            Console.WriteLine($"Ruimte: {group.Key}");
            foreach (var table in group)
            {
                Console.WriteLine($"  - Tafel {table.Id} ({table.Seats} stoelen, Beschikbaar: {table.IsAvailable})");
            }
        }
    }
}