public class StandingsResponse
{
    public List<Standing> Standings { get; set; }
}

public class Standing
{
    public string Stage { get; set; }
    public string Type { get; set; }
    public string Group { get; set; }
    public List<Table> Table { get; set; }
}

public class Table
{
    public int Position { get; set; }
    public Team Team { get; set; }
    public int PlayedGames { get; set; }
    public int Won { get; set; }
    public int Draw { get; set; }
    public int Lost { get; set; }
    public int Points { get; set; }
    public int GoalsFor { get; set; }
    public int GoalsAgainst { get; set; }
    public int GoalDifference { get; set; }
}

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string Tla { get; set; }
    public string Crest { get; set; }
}
