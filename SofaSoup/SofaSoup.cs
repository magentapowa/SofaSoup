using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace SofaSoupApp
{
    // The apps 'regulator'.
    // Bring together all the other classes
    public class SofaSoup : IDisplayable
    {
        public Displayer Display { get; set; }
        public DBManager DB { get; set; }

        public List<User> UserList { get; set; }
        public List<Event> Soup { get; set; }
        public User curUser { get; set; }

        string dir { get; set; }

// Constractor().Initializes DBmanager, loads UserList and Soup DB.
        public SofaSoup()
        {
            dir = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).FullName).FullName + "/AppData/";

            this.Log("New session initialized.");

            this.Display = new Displayer(this);
            this.DB = new DBManager();

            try
            {
                this.UserList = DB.LoadUsers();
            }
            catch (Exception ex)
            {
                this.Display.ServerError(ex);
                this.Log("Server error.");
                this.Log("Session terminated.\n");
                throw ex;
            }
            this.Soup = DB.LoadSoup(this.UserList);
        }


// Menu Lists
        public static class Menus
        {
            public static List<string> LogSign = new List<string> { "Log in", "Sign up", "øExit" };

            public static List<string> Home = new List<string> { "Profile", "Bowl", "Soup", "About", "Log Out" };
            public static List<string> Home2 = new List<string> { "Profile", "Bowl", "Soup", "Users", "About", "Log Out" };

            public static List<string> MyBowl = new List<string> { "My Events", "Create Event", "Saved Events" };
            public static List<string> EnterOnMyEvent = new List<string> { "Delete" };
            public static List<string> EnterOnSave = new List<string> { "unSave" };

            public static List<string> EnterOnEvent1 = new List<string> { "Save" };
            public static List<string> EnterOnEvent2 = new List<string> { "Save", "Delete" };

            public static List<string> EnterOnUser = new List<string> { "View", "Promote/Demote", "Delete" };

            public static List<string> LVLs = new List<string> { "Padawan", "Jedi", "Council Member", "Yoda" };

        }


// Log in.
        #region Log in
        public User LogIn()
        {
            // Username______________________________________
            string name = "";
            string message = "";
            while (true)
            {
                Console.Clear();
                Console.WriteLine(LoadHeader());
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(message);
                Console.ForegroundColor = ConsoleColor.Black;
                message = "";
                Console.Write("\tUsername:\n\t>");
                Console.Write(name);
                var key = Console.ReadKey();

                if (key.Key == ConsoleKey.Escape)
                {
                    return null;
                }
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (name.Length > 0)
                    {
                        name = name.Remove(name.Length - 1);
                    }
                    continue;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    if (UserList.Find(i => i.username == name) is null)
                    {
                        Console.Clear();
                        message = "\tThat name is not my DB.";
                        name = "";
                    }
                    else
                    {
                        break;
                    }
                }
                if (Tools.IsSymbol(key.KeyChar) > -1 || char.IsNumber(key.KeyChar) || char.IsLetter(key.KeyChar))
                {
                    Console.Write(key);
                    name += key.KeyChar;
                }

            }
            //-------------------------------------------------------------------


            // Password______________________________________
            string password = "";
            message = "";

            while (true)
            {
                Console.Clear();
                Console.WriteLine(LoadHeader());
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(message);
                Console.ForegroundColor = ConsoleColor.Black;
                message = "";
                Console.Write("\tPassword:\n\t>");
                Console.Write("*".Times(password.Length));
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Escape)
                {
                    return null;
                }
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password = password.Remove(password.Length - 1);
                    }
                    continue;
                }
                if (key.Key == ConsoleKey.Enter)
                {
                    if (password.Length != 0)
                    {
                        if (UserList.Find(i => i.username == name).password == password)
                        {
                            break;
                        }
                        message = "\tWrong password!";
                        password = "";
                    }
                    continue;
                }


                if (Tools.IsSymbol(key.KeyChar) > -1 || char.IsNumber(key.KeyChar) || char.IsLetter(key.KeyChar))
                {
                    //Console.Write("*");
                    password += key.KeyChar;
                }
            }
            //-------------------------------------------------------------------

            // LoadSpecific UserDB
            User user = UserList.Find(i => i.username == name);
            this.Log($"User loged in(userID:{user.UserID})");
            return user;
        }
        #endregion

// Sign Up
        #region Sign up
        public User SignUp(string breadcrumb = "")
        {
            // Username______________________________________
            string name = "";
            string message1 = "Choose a username:";
            string message2 = "";
            while (true)
            {
                Console.Clear();
                Console.WriteLine(LoadHeader());
                Console.WriteLine(breadcrumb);
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine("\t" + message2);
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("\t" + message1);
                Console.Write("\t>");
                Console.Write(name);
                var key = Console.ReadKey();

                if (key.Key == ConsoleKey.Escape)
                {
                    return null;
                }
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (name.Length > 0)
                    {
                        name = name.Remove(name.Length - 1);
                    }
                    continue;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    if (!(UserList.Find(i => i.username == name) is null))
                    {
                        Console.Clear();
                        message2 = "That username is taken.";
                        message1 = "Try another one:";
                        name = "";
                        // Prompt to sign up or try again for username?
                        // How will the user go back to main menu?
                    }
                    else
                    {
                        break;
                    }
                }
                if (Tools.IsSymbol(key.KeyChar) > -1 || char.IsNumber(key.KeyChar) || char.IsLetter(key.KeyChar))
                {
                    Console.Write(key);
                    name += key.KeyChar;
                }
            }
            //-------------------------------------------------------------------

            // Password______________________________________
            string password = "";
            string passHeader = "\tYour password must contain at least:";
            string[] checks = {"\t  -one lower case letter",
                "\t  -one upper case letter",
                "\t  -one symbol",
                "\t  -one number",
                "\t  -and be 6 to 10 charachters long"};

            bool[] flags = { false, false, false, false, false };

            while (true)
            {
                Console.Clear();
                Console.WriteLine(LoadHeader());
                Console.WriteLine();
                Console.WriteLine(passHeader);
                for (int i = 0; i < checks.Length; i++)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    if (flags[i])
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                    }
                    Console.WriteLine(checks[i]);
                }
                Console.ForegroundColor = ConsoleColor.Black;


                Console.Write("\t>");
                Console.Write("*".Times(password.Length));

                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Escape)
                {
                    return null;
                }
                if (char.IsLower(key.KeyChar))
                {
                    flags[0] = true;
                }
                else if (char.IsUpper(key.KeyChar))
                {
                    flags[1] = true;
                }
                else if (Tools.IsSymbol(key.KeyChar) > -1)
                {
                    flags[2] = true;
                }
                else if (char.IsNumber(key.KeyChar))
                {
                    flags[3] = true;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (password.Length > 0)
                    {
                        password = password.Remove(password.Length - 1);
                        for (int i = 0; i < 4; i++)
                        {
                            flags[i] = false;
                        }
                        for (int i = 0; i < password.Length; i++)
                        {

                            if (char.IsLower(password[i]))
                            {
                                flags[0] = true;
                            }
                            else if (char.IsUpper(password[i]))
                            {
                                flags[1] = true;
                            }
                            else if (Tools.IsSymbol(password[i]) > -1)
                            {
                                flags[2] = true;
                            }
                            else if (char.IsNumber(password[i]))
                            {
                                flags[3] = true;
                            }
                        }
                        flags[4] = false;
                        if (password.Length > 5 && password.Length < 11)
                        {
                            flags[4] = true;
                        }
                    }
                    continue;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    if (password.Length != 0 && flags[0] && flags[1] && flags[2] && flags[3] && flags[4])
                    {
                        break;
                    }
                    continue;
                }

                if (Tools.IsSymbol(key.KeyChar) > -1 || char.IsNumber(key.KeyChar) || char.IsLetter(key.KeyChar))
                {

                    password += key.KeyChar;
                }
                flags[4] = false;
                if (password.Length > 5 && password.Length < 11)
                {
                    flags[4] = true;
                }
            }
            //-------------------------------------------------------------------


            User newUser = new User(name, password);
            newUser.UserID = DB.SaveNewUser(newUser); // Could be inside the newUser constractor??
            UserList.Add(newUser);
            this.Log($"New user signed up(userID:{newUser.UserID})");
            return newUser;

        }
        #endregion


// Refresh DB
        public void RefreshDB()
        {
            this.UserList = DB.LoadUsers();
            this.Soup = DB.LoadSoup(this.UserList);
        }

// Load current user db
        public void LoadCurrentUsersDB(User user)
        {
            this.Log("Retrieving users personal data from server.");
            user.Saves = DB.LoadSaves(user, UserList);
            user.MyEvents = Soup.FindAll(e => e.User == user);
        }

// Pormote/Demote a user
        public void UpgradeUser(User user)
        {
            string breadcrumb = $"  Users > Promote/Demote user:{user.username}";
            string message = "Select level:\n\t-------------";

            int newLVLindex = Display.Menu(SofaSoup.Menus.LVLs, (int)user.LVL - 1, false, true, breadcrumb, message);
            if (newLVLindex != -1)
            {
                user.Upgrade(newLVLindex + 1);
                DB.UpdateUserValue(user, "LVL", newLVLindex + 1);
                this.Log($"User promoted/demoted user(userID:{user.UserID}) to level {user.LVL})");
            }
        }

// Delete User
        public void DeleteUser(User user, bool deleteEventsAsWell)
        {
            UserList.Remove(user);
            DB.DropUser(user);
            this.Log($"User deleted user(userID:{user.UserID})");
            // His events too.
            if (deleteEventsAsWell)
            {
                foreach (var item in Soup.FindAll(u => u.User == user))
                {
                    this.Log($"User deleted users(userID:{user.UserID}) event(eventID:{item.EventID}");
                }
                Soup.RemoveAll(u => u.User == user);
                DB.DropEventsCreatedBy(user);
            }
        }

// SaveEvent
        public void SaveEvent(User caller,Event evnt)
        {    
            caller.Saves.Add(evnt);
            DB.AddSave(caller, evnt);
            this.Log($"User added event to saves(eventID:{evnt.EventID})");
        }
// unSaveEvent
        public void unSaveEvent(User caller, Event evnt)
        {
            caller.Saves.Remove(evnt);
            DB.DropSave(caller, evnt);
            this.Log($"User removed event from saves(eventID:{evnt.EventID})");
        }

// Delete Event
        public void DeleteEvent(User caller, Event evnt)
        {
            User author = evnt.User;
            if (caller == author)
            {
                caller.MyEvents.Remove(evnt);
                caller.Saves.Remove(evnt);
            }
            Soup.Remove(evnt);
            this.Log($"User deleted event(eventID:{evnt.EventID})");
            DB.DropEvent(evnt);
        }

// Create Event
        public void CreateEvent(User author,string breadcrumb)
        {
            List<string> options = new List<string> { "Save", "Discard" };
            int selected = 0;

            string messageL1 = "";
            string messageL2 = "";
            DateTime date = new DateTime();
            string where = "";
            string description = "";

            while (true)
            {
                Console.Clear();
                Console.WriteLine(LoadHeader());
                Console.WriteLine(breadcrumb);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(messageL1);
                Console.WriteLine(messageL2);
                messageL1 = "";
                messageL2 = "";
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine();




                Console.WriteLine("\tWho : " + author.username);
                Console.WriteLine();

                Console.Write($"\tWhen : ");
                if (date.Year == 1)
                {
                    if (!Tools.TryParseDate(Console.ReadLine(),out date))
                    {
                        messageL1 = "\tThat is not a valid date format.";
                        messageL2 = "\tTry something like 17/2/2018 21:00";
                    }
                    continue;
                }
                else
                {
                    if (date<DateTime.Now)
                    {
                        messageL1 = "\tUnless you have invented time travel";
                        messageL2 = "\tI don't think anyone will be interested in that event.";
                        date = new DateTime();
                        continue;
                    }
                    Console.WriteLine(date.ToString("dd/MM/yyyy HH:mm"));
                }
                Console.WriteLine();

                Console.Write("\tWhere : ");
                if (string.IsNullOrEmpty(where))
                {
                        where = Console.ReadLine();
                    if (string.IsNullOrEmpty(where) || string.IsNullOrWhiteSpace(where))
                    {
                        messageL1 = "\tYou have to enter an Address.";
                        where = "";
                    }
                    continue;
                }
                else
                {
                    Console.WriteLine(where);
                }
                Console.WriteLine();


                Console.Write("\tWhat : ");
                if (string.IsNullOrEmpty(description))
                {
                    description = Console.ReadLine();
                    if (string.IsNullOrEmpty(description) || string.IsNullOrWhiteSpace(description))
                    {
                        messageL1 = "\tYou have to enter a description.";
                        description = "";
                    }
                    continue;
                }
                else
                {
                    Console.WriteLine(description);
                }
                Console.WriteLine();



                for (int i = 0; i < options.Count; i++)
                {
                    Console.Write("\t");
                    if (i == selected)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Magenta;
                    }
                    Console.Write(options[i]);
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.BackgroundColor = ConsoleColor.DarkCyan;
                }

                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.LeftArrow)
                {
                    selected = 0;
                }
                else if (key.Key == ConsoleKey.RightArrow)
                {
                    selected = 1;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    if (selected==1)
                    {
                        return;
                    }
                    else
                    {
                        Event evnt = new Event(author, date, where, description);
                        int eventID = DB.SaveNewEvent(author, evnt);
                        evnt.EventID = eventID;
                        author.MyEvents.Add(evnt);
                        this.Soup.Add(evnt);
                        this.Log($"User created event(eventID:{evnt.EventID})");
                        return;
                    }
                }
            }
        }//end: Create Event


        //
        // Interface methods
        //
        // Load header from txt file.
        public string LoadHeader()
        {
            string header = "";
            string[] logoLeft = File.ReadAllLines(dir + "header.txt");
            string[] logoRight = File.ReadAllLines(dir + "logo.txt");

            for (int i = 0; i < logoLeft.Length; i++)
            {
                string fill = " ";
                if (i == 2)
                {
                    fill = "_";
                }
                header += logoLeft[i] + fill.Times(Console.BufferWidth - logoLeft[i].Length - logoRight[i].Length) + logoRight[i];
            }
            string moto = "the secret not-so-secret community for jam sessions\n";
            if (!(this.curUser is null))
            {
                moto += " ".Times(Console.BufferWidth -this.curUser.ToString().Length-2) + this.curUser.ToString()+ "  ";
            }
            header += moto;
            return header;
        }
        //
        public bool CustomTryParseDate(string sDate, out DateTime Date, bool withHour)
        {
            return Tools.TryParseDate(sDate, out Date, withHour);
        }//end: Interface methods


        public void Log(string log)
        {
            using (StreamWriter file =
                   new StreamWriter(dir + "log.txt", true))
            {
                file.WriteLine(DateTime.Now + " - " + log);
            }

        }

        public void About()
        {
            Console.Clear();
            Console.WriteLine(LoadHeader());
            Console.WriteLine();

            string[] rows = File.ReadAllLines(dir + "About.txt");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            int padding = (Console.BufferWidth - rows[0].Length) / 2;
            for (int i = 0; i < rows.Length; i++)
            {
                Console.Write(" ".Times(padding));
                for (int j = 0; j < rows[i].Length; j++)
                {
                    Console.Write(rows[i][j]);
                    Thread.Sleep(2);
                }
                Console.WriteLine();
            }
            while (true)
            {
                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Escape)
                {
                    break;
                }
            }
            Console.ForegroundColor = ConsoleColor.Black;
        }

    }
}
