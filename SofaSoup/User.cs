using System;
using System.Collections;
using System.Collections.Generic;

namespace SofaSoupApp
{      
    public enum LVL
    {
        Padawan = 1,
        Jedi = 2,
        Council_Member = 3,
        Master_Yoda = 4,
        Sith_Lord = 5
    }


    public class User
    {
        public int UserID { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public LVL LVL { get; set; }
        public DateTime MemberSince { get; set; }

        public List<Event> MyEvents { get; set; }
        public List<Event> Saves { get; set; }

        public static string[] Headers = 
        {
                "username",
                "LVL",
                "Member Since",
                "MyEvents",
                "Saves"
        };
        public static string[,] HeadersSize =
            {
                {"username","10"},
                { "LVL", "14"},
                {"Member Since","16"},
                { "MyEvents","8"},
                {"Saves","5"}
            };
        public string[] Values
        {
            get
            {
                return new string[]{
                    this.username,
                    this.LVL.ToString(),
                    this.MemberSince.ToString("dd/MM/yyyy"),
                    this.MyEvents.Count.ToString(),
                    this.Saves.Count.ToString()
                };
            }

        }


        public override string ToString()
        {
            string[] parts = new string[2];
            for (int i = 0; i < 2; i++)
            {
                parts[i] = Values[i].Length > int.Parse(HeadersSize[i, 1]) ? Values[i].Substring(0, int.Parse(HeadersSize[i, 1]) - 3) + "..." : Values[i] + " ".Times(int.Parse(HeadersSize[i, 1]) - Values[i].Length);
            }
            return string.Join(" | ",parts);
        }


        public List<string> ToTableView()
        {
            return Tools.BuildSingleRowTableView(User.Headers, this.Values);
        }


        public void Upgrade(int LVLindex)
        {
            this.LVL = (LVL)Enum.Parse(typeof(LVL), LVLindex.ToString());

        }



        //Constractor for new Users.
        public User(string name, string pass)
        {
            this.username = name;
            this.password = pass;
            this.LVL = LVL.Padawan;
            this.MemberSince = DateTime.Now;

            // Initialize lists.
            this.Saves = new List<Event>();
            this.MyEvents = new List<Event>();
        }


        // Constractor for users loaded from DB.
        public User(int UserID,string name, string pass, LVL LVL,DateTime memberSince)
        {
            this.UserID = UserID;
            this.username = name;
            this.password = pass;
            this.LVL = LVL;
            this.MemberSince = memberSince;

            // Initialize lists.
            this.Saves = new List<Event>();
            this.MyEvents = new List<Event>();
        }
    }
}
