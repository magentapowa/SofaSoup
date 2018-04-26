using System;

namespace SofaSoupApp
{
    public class Event
    {
        public int EventID { get;  set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Address { get; set; }
        public User User {get; set;}

        public static string[,] HeadersSize =
            {
                {"who","10"},
                { "when", "16"},
                {"where","16"},
                { "what","16"},
            };

        public string[] Values
        {
            get
            {
                return new string[]{
                    this.User==null?"xxxx":this.User.username,
                    this.Date.ToString("dd/MM/yyyy HH:mm"),
                    this.Address,
                    this.Description
                };
            }

        }

        public override string ToString()
        {
            string[] parts = new string[4];
            for (int i = 0; i < this.Values.Length; i++)
            {
                parts[i] = Values[i].Length > int.Parse(HeadersSize[i, 1]) ? Values[i].Substring(0,int.Parse(HeadersSize[i, 1])-3)+"..." :Values[i]+ " ".Times(int.Parse(HeadersSize[i, 1])-Values[i].Length) ;
            }
            return string.Join(" | ", parts);
        }


        public string Preview()
        {
            string description = "";
            for (int i = 0; i < this.Values[3].Length; i++)
            {
                if (i%50==0 && i != 0)
                {
                    description += "\n" + " ".Times(12);
                }
                description += this.Description[i];
            }
            string preview = 
                "\n\n\tWho   : " + this.Values[0] + 
                "\n\n\tWhen  : " + this.Values[1]  +
                "\n\n\tWhere : " + this.Values[2]  +
                "\n\n\tWhat  : " + description + "\n";

            return preview;

        }


        public Event(User User, DateTime Date, string Address, string Description )
        {
            this.User = User;
            this.Description = Description;
            this.Date = Date;
            this.Address = Address;
        }
        public Event()
        {
        }

    }
}
