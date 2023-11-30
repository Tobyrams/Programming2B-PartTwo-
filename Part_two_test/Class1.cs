using System;
using System.Collections.Generic;

namespace PARTONE_CLASSLIBRARY
{

    public class Module
    {
        //Properties
        public int ID { get; set; } //This acts as a unique identifier
        public string CODE { get; set; }
        public string NAME { get; set; }
        public int NUMOFCREDITS { get; set; }
        public int NUMWEEKINSEMESTER { get; set; }
        public int CLASSHOURSPERWEEK { get; set; }
        public DateTime STARTDATE { get; set; }
        public DateTime STUDYDATE { get; set; }
        public int HOURSONSTUDYDATE { get; set; }
        public int SELFSTUDYHOURS { get; set; }
        public int REMAININGSTUDYHOURS { get; set; }


        //Module constructor
        public Module(int id,string code, string name, int numofcredits, int numweeksinsemester, int classhoursperweek, DateTime startdate, DateTime studydate, int hoursonstudydate, int selfstudyhours,int remainingstudyhours)
        {
            ID = id;
            CODE = code;
            NAME = name;
            NUMOFCREDITS = numofcredits;
            NUMWEEKINSEMESTER = numweeksinsemester;
            CLASSHOURSPERWEEK = classhoursperweek;
            STARTDATE = startdate;
            STUDYDATE = studydate;
            HOURSONSTUDYDATE = hoursonstudydate;
            SELFSTUDYHOURS = selfstudyhours;
            REMAININGSTUDYHOURS = remainingstudyhours;
        }
    }

    public class ModuleManager 
    {
        public List<Module> modules = new List<Module>();

        //This is how we will add the moduels to the modules list
        public void AddModule(Module module)
        {
            modules.Add(module);
        }

        /* Method to get all modules using IEnumerable Interface.
           IEnumerabe interface allows us to iteracte over a collection of any datatype in our 
           case its a datatype of type Module which we created.*/
        public IEnumerable<Module> GetAllModules()
        {
            return modules;
        }

    }

    public class Calculations
    {
        //Calculations
        public static int self_Study_Hours(int numCreadits, int numWeeks, int classHoursPerWeek)
        {
            return ((numCreadits * 10) / numWeeks) - classHoursPerWeek;
        }

        public static int remainingStudyHours(int studyHours, int hoursOnCertainDate)
        {
            return studyHours - hoursOnCertainDate;
        }
    }
}