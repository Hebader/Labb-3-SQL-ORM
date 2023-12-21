using Labb_3_SQL___ORM.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace Labb_3_SQL___ORM
{
    internal class Program
    {
        static void Main(string[] args)
        {
            bool Meny = true;

            while (Meny) // Meny körs så länge Meny = true
            {
                Console.WriteLine("Welcome, choose one of the options:");
                Console.WriteLine("1.Get all staff or staff within a specefic category.");
                Console.WriteLine("2.Get all students.");
                Console.WriteLine("3.Get all students whithin a sepecfic class.");
                Console.WriteLine("4.Get grades for all students within the last month.");
                Console.WriteLine("5.Get courses and average grades.");
                Console.WriteLine("6.Add stundets.");
                Console.WriteLine("7.Add staff.");
                Console.WriteLine("8.Exit program.");

                string userInput = Console.ReadLine(); // Användaren matar in en siffra från Menyn

                switch (userInput) //Lägger in siffran från användaren i switch satsen
                {

                    case "1":
                        Console.WriteLine("Type the category of staff you want to view: (To view all staff type enter)");
                        string category = Console.ReadLine(); // Använderen matar in category/trycker enter
                        GetStaff(category); // category läggs in i metoden
                        break;
                    case "2":
                        GetStudents(); // Lägger in metod
                        break;
                    case "3":
                        GetStudentsByClass();
                        break;
                    case "4":
                        GetGradesLastMonth();
                        break;
                    case "5":
                        GetCourseAverages();
                        break;
                    case "6":
                        AddStudent();
                        break;
                    case "7":
                        AddStaff();
                        break;
                    case "8":
                        Meny = false; // Avslutar programmet när meny blir false
                        break;


                    default:
                        Console.WriteLine("incorecct option. Try again.");
                        break;
                }

            }

        }

        static public void GetStaff(string category = "") // Hämta staff metod som tar emot category och har ett tom sträng som standardvärde
        {
            using (SchoolContext context = new SchoolContext()) // Skapar en instans av SchoolContext (databasen)
            {
                var query = context.staff // Skapar query för staff i databasen
                    .Join( // Använder join för att slå ihop staff och Positions tabellen
                        context.Positions, // hämtar Positions tabllen från databasen
                        staff => staff.FkpositionId,
                        position => position.PositionId,
                        (staff, position) => new { Staff = staff, Position = position } // kopplas samman i tabllen

                    )//Visar resultatet, om cateogry är tom visas alla anställda, annars visas postionen som anges
                    .Where(joinResult => string.IsNullOrEmpty(category) || joinResult.Position.PositionName == category)
                    .Select(joinResult => joinResult.Staff) // Select hämtar endast de anställda från queryn genom JoinResult.Staff
                    .OrderBy(staff => staff.StaffId) // Sorterar de antällda urifån StaffId
                    .ToList(); // retunerar en lista av anställda

                foreach (var staffMember in query)
                {
                    Console.WriteLine($"{staffMember.FirstName} {staffMember.LastName}"); //Skriver ut alla antälldas namn+efternamn
                }
            }
        }

        static public void GetStudents()
        {
            using (SchoolContext context = new SchoolContext())
            {
                Console.WriteLine("Sort by (FirstName/LastName): ");
                string sortBy = Console.ReadLine();

                Console.WriteLine("Choose order (ascending/descending): ");
                string sortOrder = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(sortBy)) // Om användaren inte anger något ska det sorteras efter FirstName
                    sortBy = "FirstName";

                if (string.IsNullOrWhiteSpace(sortOrder))
                    sortOrder = "ascending";

                IQueryable<Student> query = context.Students; //Skapas för att sortera en lista utifrån användarens val

                string sort = sortBy.ToLower(); // Har med ToLower(); för att undvika fel oavsett om användaren använder små/stora bokstäver
                string order = sortOrder.ToLower();

                // Sorterar utifrån användarens input
                switch (sort)
                {
                    // Villkor om order = ascending är sant kommer namnen vara stigande, om det är falskt fallande
                    case "firstname":
                        query = order == "ascending" ? query.OrderBy(s => s.FirstName) : query.OrderByDescending(s => s.FirstName);
                        break;
                    case "lastname":
                        query = order == "ascending" ? query.OrderBy(s => s.LastName) : query.OrderByDescending(s => s.LastName);
                        break;
                    default:
                        Console.WriteLine("Invalid input! Sorting by FirstName in ascending order."); //Om fel inmatning anges
                        query = query.OrderBy(s => s.FirstName);
                        break;
                }

                var studentList = query.ToList(); // Lägger till listan från query

                foreach (var student in studentList) //Skriver ut listan
                {
                    Console.WriteLine($"{student.FirstName} {student.LastName}");
                }
            }
        }
        static public void GetStudentsByClass()
        {
            using (SchoolContext context = new SchoolContext())
            {
                var classes = context.Classes.ToList(); //Hämta alla  Classes

                Console.WriteLine("Available classes:");

                foreach (var schoolClass in classes)
                {
                    Console.WriteLine($"{schoolClass.ClassId}: {schoolClass.ClassName}");
                }

                Console.WriteLine("Enter class name for the chosen class: ");

                string className = Console.ReadLine();

                var selectedClass = context.Classes
                    .FirstOrDefault(c => c.ClassName == className); //Använder FrstOrDefault så det blir en matchning eller ingen alls

                if (selectedClass != null) // Om klassnamnet exisisterar
                {
                    var studentsInClass = context.Students
                        .Where(s => s.FkclassId == selectedClass.ClassId) //Där ClassId matchar med det valda klassens ClasssId
                        .ToList(); //Läggs till i studentsInClass

                    if (studentsInClass.Any()) //Any metoden retunerar true om studentsInClass har minst ett element
                    {
                        Console.WriteLine($"Students in the class:");

                        foreach (var student in studentsInClass)
                        {
                            Console.WriteLine($"{student.FirstName} {student.LastName}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No students found in the selected class.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid class name.");
                }
            }
        }

        static public void GetGradesLastMonth()
        {
            using (SchoolContext context = new SchoolContext())
            {
                var grades = context.Grades //Hämtar betyg från databasen
                    .Join(context.Students, // Kopplar ihop Students med Grades genom Join
                        grade => grade.FkstudentId, //grade repreneterar FkstundentId och dess data
                        student => student.StudentId,
                        (grade, student) => new
                        { //Skapar ett objekt som innehåller infromation från Students och Grade tabllen
                            StudentName = student.FirstName,
                            CourseId = grade.FkcourseId,
                            Grade = grade.Grade1,
                            GradeDate = grade.Date
                        })
                    .Join(context.Courses,  //Hämtar Kurser
                        g => g.CourseId, // Tidigare kombinerad infomtation från grades = "g" och kommer innehålla CourseId
                        course => course.CourseId, // course represtnerar data från CourseId
                        (g, course) => new
                        {
                            //Här hämtas infotmation från flera tabeller för att skapa ett objekt
                            g.StudentName,
                            CourseName = course.CourseName,
                            g.Grade,
                            g.GradeDate
                        })
                    .ToList(); //Sparar och läggs till i databasen

                foreach (var grade in grades) //Går igenom varje objekt i listan grades med foreach
                {
                    DateTime gradeDate = DateTime.Parse(grade.GradeDate); //Konverterar inlags sträng till DateTime objekt

                    if (gradeDate >= DateTime.Now.AddMonths(-1) && gradeDate <= DateTime.Now) //Om datumet ligger inom en månad från dagens datum
                    {
                        Console.WriteLine($"Student: {grade.StudentName}, Course: {grade.CourseName}, Grade: {grade.Grade}, Date: {grade.GradeDate}"); //Utskrift
                    }
                }
            }
        }

        static public void GetCourseAverages()
        {
            using (SchoolContext context = new SchoolContext())
            {

                var courseAverages = context.Courses // Hämtar alla kurser
                    .Select(course => new //Select används för att skapa nytt objekt för varje kurs
                    {
                        CourseName = course.CourseName,
                        Grades = context.Grades
                            .Where(grade => grade.FkcourseId == course.CourseId) //Där FkcourseId är samma som CourseId
                            .Select(grade => Convert.ToInt64(grade.Grade1)) //Konverterar till int
                            .ToList() //Sparar och lägger till objekt
                    })

                    .Select(course => new //Skapar nytt obejekt med resultat för kurserna
                    {
                        CourseName = course.CourseName,
                        AverageGrade = course.Grades.Any() ? course.Grades.Average() : 0, //Om det finns betyg för kursen körs Average() metoden och räknar ut genomsnittet av betyg
                        MaxGrade = course.Grades.Any() ? course.Grades.Max() : 0, //O det finns betyg visas högsta betyget för kursen
                        MinGrade = course.Grades.Any() ? course.Grades.Min() : 0
                    })
                    .ToList(); //Lägger till och sparar objekt

                foreach (var course in courseAverages) //Skriver ut alla objekt som finns i courseAverages
                {
                    Console.WriteLine($"Course: {course.CourseName}, Average Grade: {course.AverageGrade}, " +
                        $"Max Grade: {course.MaxGrade}, Min Grade: {course.MinGrade}");
                }
            }


        }

        static public void AddStudent() // Metod för att lägga till studenter
        {
            Console.WriteLine("Enter student details:");
            Console.Write("Firstname: ");
            string firstName = Console.ReadLine();
            Console.Write("Lastname: ");
            string lastName = Console.ReadLine();

            using (SchoolContext context = new SchoolContext())  //Skapar en instans av SchoolContext genom "using" kontruktonen 
            {
                var newStudent = new Student // skaoar en kopia av Student klassen
                {
                    FirstName = firstName, // användarebs svar = "FirstName" som finns i Stuent klassen/tabellen
                    LastName = lastName
                };

                context.Students.Add(newStudent); // Lägger till den nya studenten i konekten där Student tabellen finns
                context.SaveChanges(); // Sparar ändringen i databasen

                Console.WriteLine("The student has been added!");
            }

        }
        static public void AddStaff() //Skapar metod för att lägga till anställda
        {
            Console.WriteLine("Enter staff details:");
            Console.Write("Firstname: ");
            string firstName = Console.ReadLine();
            Console.Write("Lastname: ");
            string lastName = Console.ReadLine();

            using (var context = new SchoolContext()) //Skapar en instans av SchoolContext genom "using" kontruktonen 
            {
                var newstaff = new staff //skapar en instans som heter nesstaff
                {
                    FirstName = firstName, //Lägger in förnamn och efternamn från användaren
                    LastName = lastName
                };

                context.staff.Add(newstaff); //infogar instansen i databaskontexten
                context.SaveChanges(); // Sparar ändingen i databasen

                Console.WriteLine("The staff has been added!");
            }

        }
    }
}
