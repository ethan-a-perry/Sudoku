using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Sudoku.DataAccess.Models;

public class PuzzleModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public string Constructor { get; set; }
    public string Title { get; set; }
    public string Difficulty { get; set; }
    public int NumRows { get; set; }
    public int NumCols { get; set; }
    public string[][] Grid { get; set; }
}