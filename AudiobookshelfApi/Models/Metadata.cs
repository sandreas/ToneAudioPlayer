namespace AudiobookshelfApi.Models;

public class Metadata
{
  public string Title { get; set; } = "";
  public string Subtitle { get; set; } = ""; 
  public string Description { get; set; } = "";
  public string Publisher { get; set; } = "";

  public List<Person> Authors { get; set; } = new();
  public List<string> Narrators { get; set; } = new();
  public List<Series> Series { get; set; } = new();
  public List<string> Genres { get; set; } = new();
  
  

}
/*
"title": "Wizards First Rule",
            "titleIgnorePrefix": "Wizards First Rule",
            "subtitle": null,
            "authors": [
              {
                "id": "aut_z3leimgybl7uf3y4ab",
                "name": "Terry Goodkind"
              }
            ],
            "narrators": [
              "Sam Tsoutsouvas"
            ],
            "series": [
              {
                "id": "ser_cabkj4jeu8be3rap4g",
                "name": "Sword of Truth",
                "sequence": "1"
              }
            ],
            "genres": [
              "Fantasy"
            ],
            "publishedYear": "2008",
            "publishedDate": null,
            "publisher": "Brilliance Audio",
            "description": "The masterpiece that started Terry Goodkind's New York Times bestselling epic Sword of Truth In the aftermath of the brutal murder of his father, a mysterious woman, Kahlan Amnell, appears in Richard Cypher's forest sanctuary seeking help...and more. His world, his very beliefs, are shattered when ancient debts come due with thundering violence. In a dark age it takes courage to live, and more than mere courage to challenge those who hold dominion, Richard and Kahlan must take up that challenge or become the next victims. Beyond awaits a bewitching land where even the best of their hearts could betray them. Yet, Richard fears nothing so much as what secrets his sword might reveal about his own soul. Falling in love would destroy them - for reasons Richard can't imagine and Kahlan dare not say. In their darkest hour, hunted relentlessly, tormented by treachery and loss, Kahlan calls upon Richard to reach beyond his sword - to invoke within himself something more noble. Neither knows that the rules of battle have just changed...or that their time has run out. Wizard's First Rule is the beginning. One book. One Rule. Witness the birth of a legend.",
            "isbn": null,
            "asin": "B002V0QK4C",
            "language": null,
            "explicit": false,
            "authorName": "Terry Goodkind",
            "authorNameLF": "Goodkind, Terry",
            "narratorName": "Sam Tsoutsouvas",
            "seriesName": "Sword of Truth"
          }
          */