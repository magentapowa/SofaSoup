using System;
using System.Collections.Generic;

namespace SofaSoupApp
{
    
    public interface IDisplayable
    {
        string LoadHeader();
        bool CustomTryParseDate(string sDate, out DateTime Date, bool withHour);
    }

    // Generic console app class/tool.
    // Provides the ability to build navigable menu lists. 
    //
    // Use the returned index from a method in your 
    // main programm flow to determine the actions taken for each option.
    // The caller must implement the IDisplayable Interface.
    public class Displayer
    {
        public IDisplayable caller { get; set; }

        public ConsoleColor TextColor { get; set; }
        public ConsoleColor BackColor { get; set; }
        public ConsoleColor SelectionTextColor { get; set; }
        public ConsoleColor SelectionBackColor { get; set; }
        public ConsoleColor staticSelectionColor { get; set; } // for 2 level Menus and above.
        public ConsoleColor warningColor { get; set; }

        // Constractor
        //
        public Displayer(IDisplayable caller)
        {
            this.caller = caller;

            //Default colors
            this.TextColor = ConsoleColor.Black;
            this.BackColor = ConsoleColor.DarkCyan;
            this.SelectionTextColor = ConsoleColor.DarkCyan;
            this.SelectionBackColor = ConsoleColor.Magenta;
            this.staticSelectionColor = ConsoleColor.Magenta;
            this.warningColor = ConsoleColor.DarkRed;
        }



        // Display promt for aproval(Yes, No) of action. (1 level)
        // 
        // - escape does NOT work
        //(- same functionality can be achieved with 'Menu' method specifing the right parameteres:
        //(  Menu(new List<string>{"Yes", "No"},1,true,false, breadcrumb="", text="Are you sure you want to proceed?");
        public bool AreYouSure( string message = "Are you sure you want to proceed?",string breadcrumb="",int defaultOpption=1)
        {
            List<string> yesno = new List<string> { "Yes", "No" };
            int position = defaultOpption;
            while (true)
            {
                Console.Clear();
                Console.WriteLine(caller.LoadHeader());
                Console.WriteLine(breadcrumb);
                Console.WriteLine();
                Console.WriteLine("\t" + message);

                for (int i = 0; i < yesno.Count; i++)
                {
                    Console.ForegroundColor = TextColor;
                    Console.Write("\t\t");
                    if (position == i)
                    {
                        Console.ForegroundColor = SelectionTextColor;
                        Console.BackgroundColor = SelectionBackColor;
                    }
                    Console.Write(yesno[i]);

                    Console.ForegroundColor = TextColor;
                    Console.BackgroundColor = BackColor;
                }

                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Enter)
                {
                    if (position == 0)
                    {
                        return true;
                    }
                    return false;
                }
                if (key.Key == ConsoleKey.LeftArrow)
                {
                    position = 0;
                }
                else if (key.Key == ConsoleKey.RightArrow)
                {
                    position = 1;
                }
            }
        }

        // Make menu with navigation ability. Horizontal or vertical. (1 level)
        //
        // - menu (List<T>): the options list
        // - menuIndex (int): the starting selected position
        // - isHorizontal (bool): vertical or horizontal display of the options
        // - hasEscape (bool): if hitting escape takes you back
        // - breadcrumb (string): for displaying the trail for nested menus.
        // - text (string): message to be displayed above the options.
        //
        // returns the index of the item selected from the 'menu' List.
        // returns -1 for escape.
        public int Menu<T>(List<T> menu, int menuIndex, bool isHorizontal, bool hasEscape, string breadcrumb = "", string text = "")
        {

            string separator = "\t\t";
            ConsoleKey Next = ConsoleKey.RightArrow;
            ConsoleKey Previous = ConsoleKey.LeftArrow;
            if (isHorizontal == false)
            {
                separator = "\n\t";
                Next = ConsoleKey.DownArrow;
                Previous = ConsoleKey.UpArrow;
            }

            int position = menuIndex;
            while (true)
            {
                Console.Clear();
                Console.WriteLine(caller.LoadHeader());
                Console.WriteLine(breadcrumb);
                Console.WriteLine();
                Console.WriteLine("\t" + text);
                Console.Write("\t");

                for (int i = 0; i < menu.Count; i++)
                {
                    if (position == i)
                    {
                        Console.ForegroundColor = SelectionTextColor;
                        Console.BackgroundColor = SelectionBackColor;
                    }
                    Console.Write(menu[i]);

                    Console.ForegroundColor = TextColor;
                    Console.BackgroundColor = BackColor;
                    Console.Write(separator);
                }


                var key = Console.ReadKey();
                if (hasEscape)
                {
                    if (key.Key == ConsoleKey.Escape)
                    {
                        return -1;
                    }
                }


                if (key.Key == ConsoleKey.Enter)
                {
                    return position;
                }
                if (key.Key == Previous)
                {
                    if (position > 0)
                    {
                        position -= 1;
                    }
                }
                else if (key.Key == Next)
                {
                    if (position < menu.Count - 1)
                    {
                        position += 1;
                    }
                }
            }
        }


        // Display INLINE(horizontal) submenu of selected item on VERTICAL basemenu. (2 levels)
        //
        public int SubMenu<T>(List<string> submenu, int subIndex, List<T> basemenu, int baseIndex, string breadcrumb = "", string text="") //
        {
            // make base menu 
            string separator = "\n\t";
            ConsoleKey Next = ConsoleKey.RightArrow;
            ConsoleKey Previous = ConsoleKey.LeftArrow;

            int position = subIndex;
            while (true)
            {
                Console.Clear();
                Console.WriteLine(caller.LoadHeader());
                Console.WriteLine(breadcrumb);
                Console.WriteLine();
                Console.WriteLine("\t" + text);
                Console.Write("\t");

                for (int i = 0; i < basemenu.Count; i++)
                {
                    if (i == baseIndex)
                    {
                        Console.ForegroundColor = staticSelectionColor;
                        Console.Write(basemenu[i]);

                        // Make submenu
                        Console.Write(" .");

                        for (int j = 0; j < submenu.Count; j++)
                        {
                            Console.ForegroundColor = TextColor;
                            Console.Write(" ");
                            if (position == j)
                            {
                                Console.ForegroundColor = SelectionTextColor;
                                Console.BackgroundColor = SelectionBackColor;
                            }
                            Console.Write(submenu[j]);
                            Console.ForegroundColor = TextColor;
                            Console.BackgroundColor = BackColor;
                        }
                    }
                    else
                    {
                        Console.Write(basemenu[i]);
                    }
                    Console.ForegroundColor = TextColor;
                    Console.BackgroundColor = BackColor;
                    Console.Write(separator);
                }


                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Escape)
                {
                    return -1;
                }
                if (key.Key == ConsoleKey.Enter)
                {
                    return position;
                }
                if (key.Key == Previous)
                {
                    if (position > 0)
                    {
                        position -= 1;
                    }
                }
                else if (key.Key == Next)
                {
                    if (position < submenu.Count - 1)
                    {
                        position += 1;
                    }
                }
            }

        }

        // Display VERTICAL generic list/result of selected item from VERTICAL basemenu. (2 levels)
        //
        // Can be used to display list of objects(make sure you ovveride ToString() of object T), results, strings etc.
        // - isNavigable (bool) is to make it more generic so you can display whatever text you want next to the base
        //   menu. Just pass the text in a List<string> of rows and make isNavigable=false. It will display the text and 
        //   you can exit only with escape.
        public int SubList<T>(List<T> list, int listIndex,bool hasHeaders,  List<string> basemenu, int baseIndex, bool isNavigable,string breadcrumb = "",string message= "")
        {
            // make base menu 
            string separator = "\n\t";
            ConsoleKey Next = ConsoleKey.DownArrow;
            ConsoleKey Previous = ConsoleKey.UpArrow;
            int start = 0;
            if (hasHeaders)
                start = 2;

            while (true)
            {
                Console.Clear();
                Console.WriteLine(caller.LoadHeader());
                Console.WriteLine(breadcrumb);
                Console.WriteLine();
                Console.WriteLine("\t" + " ".Times(10) + message);
                Console.Write("\t");

                for (int i = 0; i < Math.Max(basemenu.Count, list.Count); i++)
                {
                    if (i < basemenu.Count)
                    {
                        if (i == baseIndex)
                        {
                            Console.ForegroundColor = staticSelectionColor;
                        }
                        Console.Write(basemenu[i]);
                        Console.ForegroundColor = TextColor;
                        Console.Write(" ".Times(10 - basemenu[i].Length));
                    }
                    else
                    {
                        Console.Write(" ".Times(10));
                    }


                    if (i < list.Count)
                    {
                        if (isNavigable && listIndex == i)
                        {
                            Console.ForegroundColor = SelectionTextColor;
                            Console.BackgroundColor = SelectionBackColor;
                        }
                        Console.Write(list[i]);
                    }


                    Console.ForegroundColor = TextColor;
                    Console.BackgroundColor = BackColor;
                    Console.Write(separator);
                }


                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Escape)
                {
                    return -1;
                }
                if (isNavigable)
                {
                    if (key.Key == ConsoleKey.Enter)
                    {
                        return listIndex;
                    }
                    if (key.Key == Previous)
                    {
                        if (listIndex > start)
                        {
                            listIndex -= 1;
                        }
                    }
                    else if (key.Key == Next)
                    {
                        if (listIndex < list.Count - 1)
                        {
                            listIndex += 1;
                        }
                    }
                }

            }
        }

        // Display INLINE(horizontal) submenu of selected item from VERTICANL List of selected item of VERTICAL basemenu. (3 levels)
        //
        public int SubMenuFromSubList<T>(List<string> submenu, int subIndex, List<T> list, int listIndex, List<string> basemenu, int baseIndex, string breadcrumb="",string text="")
        {
            // make base menu 
            string separator = "\n\t";
            ConsoleKey Next = ConsoleKey.RightArrow;
            ConsoleKey Previous = ConsoleKey.LeftArrow;

            int position = subIndex;
            while (true)
            {
                Console.Clear();
                Console.WriteLine(caller.LoadHeader());
                Console.WriteLine(breadcrumb);
                Console.WriteLine();
                Console.WriteLine("\t" + text);
                Console.Write("\t");

                for (int i = 0; i < Math.Max(basemenu.Count, list.Count); i++)
                {
                    if (i < basemenu.Count)
                    {
                        if (i == baseIndex)
                        {
                            Console.ForegroundColor = staticSelectionColor;
                        }
                        Console.Write(basemenu[i]);
                        Console.ForegroundColor = TextColor;
                        Console.Write(" ".Times(10 - basemenu[i].Length));
                    }
                    else
                    {
                        Console.Write(" ".Times(10));
                    }

                    if (i < list.Count)
                    {
                        if (i == listIndex)
                        {
                            Console.ForegroundColor = staticSelectionColor;
                            Console.Write(list[i]);
                            Console.Write(" .");

                            for (int j = 0; j < submenu.Count; j++)
                            {
                                Console.ForegroundColor = TextColor;
                                Console.Write(" ");
                                if (position == j)
                                {
                                    Console.ForegroundColor = SelectionTextColor;
                                    Console.BackgroundColor = SelectionBackColor;
                                }
                                Console.Write(submenu[j]);
                                Console.ForegroundColor = TextColor;
                                Console.BackgroundColor = BackColor;
                            }

                        }
                        else
                        {
                            Console.Write(list[i]);
                        }

                    }
                    Console.ForegroundColor = TextColor;
                    Console.BackgroundColor = BackColor;
                    Console.Write(separator);
                }


                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Escape)
                {
                    return -1;
                }
                if (key.Key == ConsoleKey.Enter)
                {
                    return position;
                }
                if (key.Key == Previous)
                {
                    if (position > 0)
                    {
                        position -= 1;
                    }
                }
                else if (key.Key == Next)
                {
                    if (position < submenu.Count - 1)
                    {
                        position += 1;
                    }
                }
            }
        }


        // Input methods
        //
        //
        // Display promt for taking a Datetime input
        //
        public DateTime TakeInputDate(string message, string breadcrumb = "",bool hasHour=true)
        {
            string ErrorMessage = "";

            while (true)
            {
                Console.Clear();
                Console.WriteLine(caller.LoadHeader());
                Console.WriteLine(breadcrumb);

                Console.ForegroundColor = warningColor;
                Console.WriteLine(ErrorMessage);
                Console.ForegroundColor = this.TextColor;
                ErrorMessage = "";

                Console.WriteLine("\t" + message);

                Console.Write("\t>");
                DateTime date;
                if (caller.CustomTryParseDate(Console.ReadLine(),out date,hasHour))
                {
                    return date;
                }
                ErrorMessage = "That is not a valid date input. Try something like 17/2/2018.";
            }
        }


        // Display promt for taking a notNullNorEmpty string input
        //
        public string TakeInput(string message, string breadcrumb = "")
        {
            string ErrorMessage = "";

            while (true)
            {
                Console.Clear();
                Console.WriteLine(caller.LoadHeader());
                Console.WriteLine(breadcrumb);

                Console.ForegroundColor = warningColor;
                Console.WriteLine(ErrorMessage);
                Console.ForegroundColor = this.TextColor;
                ErrorMessage = "";

                Console.WriteLine("\t" + message);

                Console.Write("\t>");
                string input = Console.ReadLine();
                if (!string.IsNullOrEmpty(input))
                {
                    return input;
                }
                ErrorMessage = "You have to enter something.";
            }




        }


        public void ServerError(Exception ex)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(caller.LoadHeader());
                Console.WriteLine();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("\t" + ex.Message);
                Console.ForegroundColor = ConsoleColor.Black;

                var key = Console.ReadKey();
                if (key.Key == ConsoleKey.Escape || key.Key == ConsoleKey.Enter)
                {
                    return;
                }
            }
        }

    }
}
