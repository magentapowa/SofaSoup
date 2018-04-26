using System;
using System.Collections.Generic;

namespace SofaSoupApp
{
    class MainClass
    {
        //
        // The entry point of the program.
        //
        public static void Main()
        {
            Console.ReadKey();
            MainProgrammFlow();
        }


        /*
         *  Main Programm Flow
         *  ------------------
         *  every decision/movement in the menu
         *  is determined and handled inside this method.
         *  
         *  It interacts mainly with the SofaSoup class
         *  (SofaSoup is the "regulator", it brings all
         *  the other classes together.
         * 
         */
        public static void MainProgrammFlow()
        {

            Console.Title = "SofaSoup v0.3  VHS_studio";
            Console.BackgroundColor = ConsoleColor.DarkCyan;

            // Instansiate the apps Regulator. 
            SofaSoup sofo;
            try
            {
                sofo = new SofaSoup();
            }
            catch (Exception)
            {
                return;
            }


            //  - LogIn or SignUp 
            User currentUser = null;
            int LogSignIndex = 0;
            while (true)
            {
                sofo.RefreshDB();

                LogSignIndex = sofo.Display.Menu(SofaSoup.Menus.LogSign,LogSignIndex, true,false);
                if (LogSignIndex == 0)
                {
                    currentUser = sofo.LogIn();
                }
                else if (LogSignIndex == 1)
                {
                    currentUser = sofo.SignUp();
                }
                else
                {
                    sofo.Log("Session terminated.\n");
                    return;
                }
                if (currentUser is null)
                {
                    continue;
                }


                sofo.curUser = currentUser;
                sofo.LoadCurrentUsersDB(currentUser);


                // Menu Options.
                int homeIndex = 0;
                while (true)
                {
                    List<string> HomeMenu = SofaSoup.Menus.Home;
                    int UsersIndex = -999;
                    int AboutIndex = 3;
                    int logOutIndex = 4;
                    if (currentUser.LVL == LVL.Master_Yoda || currentUser.LVL == LVL.Sith_Lord)
                    {
                        HomeMenu = SofaSoup.Menus.Home2;
                        UsersIndex = 3;
                        AboutIndex = 4;
                        logOutIndex = 5;
                    }

                    homeIndex = sofo.Display.Menu(HomeMenu,homeIndex, false,false);
                    // Log out
                    if (homeIndex == logOutIndex)  
                        break;

                    // Profile
                    if (homeIndex == 0)
                    {  
                        sofo.Display.SubList(currentUser.ToTableView(), 0,false, HomeMenu, homeIndex, false);
                    }
                    // Bowl
                    else if (homeIndex == 1) 
                    {
                        int submenuIndex = 0;
                        while (true)
                        {
                            submenuIndex = sofo.Display.SubMenu(SofaSoup.Menus.MyBowl,submenuIndex, HomeMenu, homeIndex);
                            //Escape
                            if (submenuIndex == -1) 
                                break;

                            // My Events
                            if (submenuIndex == 0) 
                            {
                                string breadcrumb = "  Bowl > My Events";
                                int selectedEvent = 0;
                                while (true)
                                {
                                    if (currentUser.MyEvents.Count == 0)
                                    {
                                        sofo.Display.Menu(new List<string> { "Nothing to show." }, 0, false, true, breadcrumb);
                                        break;
                                    }

                                    selectedEvent = sofo.Display.Menu(currentUser.MyEvents, selectedEvent,false,true,breadcrumb);
                                    // Escape
                                    if (selectedEvent == -1) 
                                        break;

                                    int EventOptionsIndex = 0;
                                    while (true)
                                    {
                                        EventOptionsIndex = sofo.Display.Menu(SofaSoup.Menus.EnterOnMyEvent, EventOptionsIndex, true, true, breadcrumb, currentUser.MyEvents[selectedEvent].Preview());
                                        // Escape
                                        if (EventOptionsIndex == -1) 
                                            break;
                                        
                                        //Delete
                                        if (EventOptionsIndex == 0) 
                                        {
                                            if (sofo.Display.AreYouSure(breadcrumb:"  Bowl > My Events > Delete Event"))
                                            {
                                                Event evnt = currentUser.MyEvents[selectedEvent];
                                                sofo.DeleteEvent(currentUser, evnt);
                                                if (selectedEvent>0)
                                                    selectedEvent--;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            // Create
                            else if (submenuIndex == 1) 
                            {
                                string breadcrumb = "  Bowl > Create Event";
                                if (currentUser.LVL == LVL.Padawan)
                                {
                                    sofo.Display.SubMenu( new List<string> { "You have to be a Jedi or above to create an event. Sorry.." },0, HomeMenu, homeIndex);
                                    continue;
                                }
                                sofo.CreateEvent(currentUser,breadcrumb);
                            }
                            // Saved
                            else if (submenuIndex == 2) 
                            {
                                string breadcrumb = "  Bowl > My Saves";
                                int selectedEvent = 0;
                                while (true)
                                {
                                    if (currentUser.Saves.Count == 0)
                                    {
                                        sofo.Display.Menu(new List<string> { "Nothing to show." }, 0, false, true, breadcrumb);
                                        break;
                                    }
                                    selectedEvent = sofo.Display.Menu(currentUser.Saves, selectedEvent,false,true,breadcrumb);
                                    // Escape
                                    if (selectedEvent == -1) 
                                        break;

                                    int EventOptionsIndex = sofo.Display.Menu(SofaSoup.Menus.EnterOnSave, 0, true, true, breadcrumb, currentUser.Saves[selectedEvent].Preview());
                                    //Remove
                                    if (EventOptionsIndex == 0) 
                                    {
                                        Event evnt = currentUser.Saves[selectedEvent];
                                        sofo.unSaveEvent(currentUser, evnt);
                                        if (selectedEvent>0)
                                            selectedEvent--;
                                    }
                                }
                            }
                        }

                    }
                    // Soup
                    else if (homeIndex == 2) 
                    {

                        int option = 0;
                        while (true)
                        {
                            option = sofo.Display.SubMenu(new List<string> { "View All", "Filter" }, option, HomeMenu, homeIndex);
                            if (option == -1)
                                break;

                            string breadcrumb = "";
                            List<Event> filteredEvents = null;

                            List<string> submenu = SofaSoup.Menus.EnterOnEvent2;
                            if (currentUser.LVL == LVL.Padawan || currentUser.LVL == LVL.Jedi)
                                submenu = SofaSoup.Menus.EnterOnEvent1;

                            // View All
                            if (option == 0)
                            {
                                breadcrumb = "  Soup > View All";
                                filteredEvents = sofo.Soup;
                            }
                            // Filter by
                            else if (option == 1)
                            {
                                breadcrumb = "  Soup > Filter";
                                List<string> filterBy = new List<string> { "user", "date", "description", "location" };
                                int filterIndex = sofo.Display.Menu(filterBy, 0, false, true, breadcrumb, "Filter by:");
                                // Escape
                                if (filterIndex == -1)
                                    break;

                                // Filter by User
                                if (filterIndex == 0)
                                {
                                    breadcrumb = "  Soup > Filter by user";
                                    int selectedUser = sofo.Display.Menu(sofo.UserList, filterIndex, false, true, breadcrumb, "Select user:");
                                    if (selectedUser == -1)
                                        continue;
                                    filteredEvents = sofo.Soup.FindAll(u => u.User == sofo.UserList[selectedUser]);
                                    breadcrumb += ": " + sofo.UserList[selectedUser].username;
                                }
                                // Filter by date
                                else if (filterIndex == 1)
                                {
                                    breadcrumb = "  Soup > Filter by date";
                                    DateTime filterDate = sofo.Display.TakeInputDate("Enter a date:", breadcrumb, false);
                                    filteredEvents = sofo.Soup.FindAll(u => u.Date > filterDate);
                                    breadcrumb += ": " + filterDate.ToShortDateString();
                                }
                                // Filter by description
                                else if (filterIndex == 2)
                                {
                                    breadcrumb = "  Soup > Filter by description";
                                    string filterDesc = sofo.Display.TakeInput("Please enter a keyword:", breadcrumb);
                                    filteredEvents = sofo.Soup.FindAll(u => u.Description.ToLower().Contains(filterDesc.ToLower()));
                                    breadcrumb += ": " + filterDesc;
                                }
                                // Filter by location
                                else if (filterIndex == 3)
                                {
                                    breadcrumb = "  Soup > Filter by location";
                                    string filterLocation = sofo.Display.TakeInput("Please enter a keyword:", breadcrumb);
                                    filteredEvents = sofo.Soup.FindAll(u => u.Address.ToLower().Contains(filterLocation.ToLower()));
                                    breadcrumb += ": " + filterLocation;
                                }
                            }

                            // Results
                            int selectedEvent = 0;
                            while (true)
                            {
                                // Empty Result
                                if (filteredEvents.Count == 0)
                                {
                                    sofo.Display.Menu(new List<string> { "Nothing to show." }, 0, false, true, breadcrumb);
                                    break;
                                }

                                selectedEvent = sofo.Display.Menu(filteredEvents, selectedEvent, false, true, breadcrumb);
                                // Escape
                                if (selectedEvent == -1)
                                    break;

                                int eventOptionIndex = 0;
                                while (true)
                                {
                                    submenu[0] = "Save";

                                    if (currentUser.Saves.Exists(e => e.EventID == filteredEvents[selectedEvent].EventID))
                                        submenu[0] = "unSave";

                                    eventOptionIndex = sofo.Display.Menu(submenu, eventOptionIndex, true, true, breadcrumb, filteredEvents[selectedEvent].Preview());
                                    // Escape
                                    if (eventOptionIndex == -1)
                                        break;

                                    //Save
                                    if (eventOptionIndex == 0)
                                    {
                                        if (submenu[0] == "Save")
                                            sofo.SaveEvent(currentUser, filteredEvents[selectedEvent]);
                                        else if (submenu[0] == "unSave")
                                            sofo.unSaveEvent(currentUser, filteredEvents[selectedEvent]);
                                    }
                                    //Delete
                                    if (eventOptionIndex == 1)
                                    {
                                        if (sofo.Display.AreYouSure(breadcrumb: breadcrumb))
                                        {
                                            sofo.DeleteEvent(currentUser, filteredEvents[selectedEvent]);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }// Soup
                    // Users
                    else if (homeIndex == UsersIndex) 
                    {
                        //string breadcrumb = "";
                        int selectedUser = 0;
                        while (true)
                        {
                            selectedUser = sofo.Display.SubList(sofo.UserList, selectedUser,false, HomeMenu, homeIndex,true);
                            if (selectedUser == -1) // Escape
                                break;
                            
                            if (currentUser == sofo.UserList[selectedUser] || sofo.UserList[selectedUser].LVL==LVL.Sith_Lord)
                                continue;

                            if (currentUser.LVL == LVL.Master_Yoda || currentUser.LVL == LVL.Sith_Lord)
                            {
                                List<string> submenu = SofaSoup.Menus.EnterOnUser;

                                int userAction = 0;
                                while (true)
                                {
                                    userAction = sofo.Display.SubMenuFromSubList(submenu,userAction, sofo.UserList, selectedUser, HomeMenu, homeIndex);
                                    // Escape
                                    if (userAction == -1) 
                                        break;

                                    // View
                                    if (userAction == 0)
                                        sofo.Display.SubList(sofo.UserList[selectedUser].ToTableView(), 0,false, HomeMenu, homeIndex, false);
                                    // Promote/Demote
                                    else if (userAction == 1)
                                        sofo.UpgradeUser(sofo.UserList[selectedUser]);
                                    // Delete
                                    else if (userAction == 2)
                                    {
                                        string breadcrumb = "  Users > Delete";
                                        if (sofo.Display.AreYouSure(breadcrumb:breadcrumb))
                                        {
                                            User tempUser = sofo.UserList[selectedUser];
                                            sofo.DeleteUser(tempUser, sofo.Display.AreYouSure("Delete all of the users events as well?",breadcrumb));
                                            if (selectedUser > 0)
                                                selectedUser--;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // About
                    else if (homeIndex == AboutIndex) 
                        sofo.About();
                }
                sofo.Log("User logged out");
                currentUser = null;

            }
        }
    }
}
