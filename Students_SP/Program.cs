using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;

namespace AttendanceManagement
{
    public class Student
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public List<AttendanceRecord> Attendance { get; set; } = new List<AttendanceRecord>();
        public Dictionary<string, List<int>> Grades { get; set; } = new Dictionary<string, List<int>>();

        public Student(int id, string name, string email)
        {
            Id = id;
            Name = name;
            Email = email;
            Grades.Add("Русский язык", new List<int>());
            Grades.Add("Английский язык", new List<int>());
            Grades.Add("Математика", new List<int>());
            Grades.Add("География", new List<int>());
            Grades.Add("Основы программирования", new List<int>());
        }

        public double GetAverageGrade()
        {
            int totalGrades = 0;
            int totalSubjects = 0;

            foreach (var subject in Grades.Keys)
            {
                if (Grades[subject].Count > 0)
                {
                    totalGrades += Grades[subject].Sum();
                    totalSubjects++;
                }
            }

            return totalSubjects > 0 ? (double)totalGrades / totalSubjects : 0;
        }
    }


    public class AttendanceRecord
    {
        public DateTime Date { get; set; }
        public bool IsPresent { get; set; }

        public AttendanceRecord(DateTime date, bool isPresent)
        {
            Date = date;
            IsPresent = isPresent;
        }
    }


    public class AttendanceManager
    {
        public List<Student> students = new List<Student>();
        public int nextStudentId = 1;


        public string usersFilePath = "users.xml";


        public Dictionary<string, User> users = new Dictionary<string, User>();

        public AttendanceManager()
        {
            LoadUsers();
        }


        public void LoadUsers()
        {
            if (File.Exists(usersFilePath))
            {
                XDocument doc = XDocument.Load(usersFilePath);
                foreach (var userElement in doc.Root.Elements("user"))
                {
                    string login = userElement.Element("login").Value;
                    string password = userElement.Element("password").Value;
                    string userType = userElement.Element("type").Value;

                    users.Add(login, new User(login, password, userType));
                }
            }
        }


        public void AddStudent(Student student)
        {
            if (string.IsNullOrWhiteSpace(student.Name))
            {
                Console.WriteLine("Имя студента не может быть пустым.");
                return;
            }

            if (!IsValidEmail(student.Email))
            {
                Console.WriteLine("Некорректный формат email. Попробуйте снова.");
                return;
            }

            student.Id = nextStudentId++;
            students.Add(student);
            Console.WriteLine("Студент успешно добавлен.");
        }

        public void RemoveStudent(int id)
        {
            Student studentToRemove = students.Find(s => s.Id == id);
            if (studentToRemove != null)
            {
                students.Remove(studentToRemove);
                Console.WriteLine("Студент успешно удален.");
            }
            else
            {
                Console.WriteLine("Студент с таким идентификатором не найден.");
            }
        }


        public void EditStudent(int id, Student updatedStudent)
        {
            Student studentToUpdate = students.Find(s => s.Id == id);
            if (studentToUpdate != null)
            {

                if (string.IsNullOrWhiteSpace(updatedStudent.Name))
                {
                    Console.WriteLine("Имя студента не может быть пустым.");
                    return;
                }


                if (!IsValidEmail(updatedStudent.Email))
                {
                    Console.WriteLine("Некорректный формат email. Попробуйте снова.");
                    return;
                }

                studentToUpdate.Name = updatedStudent.Name;
                studentToUpdate.Email = updatedStudent.Email;
                Console.WriteLine("Данные студента успешно обновлены.");
            }
            else
            {
                Console.WriteLine("Студент с таким идентификатором не найден.");
            }
        }

        public void ListStudents()
        {
            if (students.Count == 0)
            {
                Console.WriteLine("Список студентов пуст.");
                return;
            }

            Console.WriteLine("Список студентов:");
            Console.WriteLine("---------------------");
            foreach (var student in students)
            {
                Console.WriteLine($"ID: {student.Id}, Имя: {student.Name}, Email: {student.Email}");
            }
        }

        public void AddAttendance(int studentId, AttendanceRecord record)
        {
            Student student = students.Find(s => s.Id == studentId);
            if (student != null)
            {
                student.Attendance.Add(record);
                Console.WriteLine("Запись о посещении успешно добавлена.");
            }
            else
            {
                Console.WriteLine("Студент с таким идентификатором не найден.");
            }
        }

        public void ListAttendance(int studentId)
        {
            Student student = students.Find(s => s.Id == studentId);
            if (student != null)
            {
                if (student.Attendance.Count == 0)
                {
                    Console.WriteLine("Записей о посещении нет.");
                    return;
                }

                Console.WriteLine($"Записи о посещении для студента {student.Name}:");
                Console.WriteLine("---------------------");
                foreach (var record in student.Attendance)
                {
                    Console.WriteLine($"Дата: {record.Date.ToShortDateString()}, Присутствие: {record.IsPresent}");
                }
            }
            else
            {
                Console.WriteLine("Студент с таким идентификатором не найден.");
            }
        }

        public void AddGrade(int studentId, string subject, int grade)
        {
            Student student = students.Find(s => s.Id == studentId);
            if (student != null)
            {
                if (student.Grades.ContainsKey(subject))
                {
                    student.Grades[subject].Add(grade);
                    Console.WriteLine($"Оценка {grade} по предмету {subject} добавлена.");
                }
                else
                {
                    Console.WriteLine("Неверный предмет. Доступные предметы:");
                    foreach (var s in student.Grades.Keys)
                    {
                        Console.WriteLine(s);
                    }
                }
            }
            else
            {
                Console.WriteLine("Студент с таким идентификатором не найден.");
            }
        }

        public void ListGrades(int studentId)
        {
            Student student = students.Find(s => s.Id == studentId);
            if (student != null)
            {
                if (student.Grades.Count == 0)
                {
                    Console.WriteLine("Оценок нет.");
                    return;
                }

                Console.WriteLine($"Оценки студента {student.Name}:");
                Console.WriteLine("---------------------");
                foreach (var subject in student.Grades.Keys)
                {
                    Console.WriteLine($"{subject}: {string.Join(", ", student.Grades[subject])}");
                }


                double averageGrade = student.GetAverageGrade();
                Console.WriteLine($"Средняя оценка: {averageGrade:F2}");
            }
            else
            {
                Console.WriteLine("Студент с таким идентификатором не найден.");
            }
        }
        public bool IsValidEmail(string email)
        {

            if (!email.Contains("@") || !email.Contains("."))
            {
                return false;
            }


            if (email.Contains(" "))
            {
                return false;
            }


            foreach (char c in email)
            {
                if (!((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || (c >= '0' && c <= '9') ||
                      c == '@' || c == '.' || c == '-' || c == '_' || c == '+' || c == '='))
                {
                    return false;
                }
            }

            return true;
        }
    }


    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public string Type { get; set; }

        public User(string login, string password, string type)
        {
            Login = login;
            Password = password;
            Type = type;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            AttendanceManager attendanceManager = new AttendanceManager();
            User currentUser = null;
            bool ImAStudent = true;
            while (true)
            {
                if (currentUser == null)
                {

                    Console.WriteLine("Введите логин:");
                    string login = Console.ReadLine();

                    Console.WriteLine("Введите пароль:");
                    string password = Console.ReadLine();

                    if (attendanceManager.users.ContainsKey(login))
                    {
                        currentUser = attendanceManager.users[login];
                        if (currentUser.Password == password)
                        {
                            Console.WriteLine($"Добро пожаловать, {currentUser.Login}!");


                            if (currentUser.Type == "student")
                            {
                                Console.WriteLine("\nВаша информация:");
                                Student student = attendanceManager.students.Find(s => s.Email == currentUser.Login);
                                if (student != null)
                                {
                                    attendanceManager.ListGrades(student.Id);
                                    attendanceManager.ListAttendance(student.Id);
                                }
                            }
                        }
                        else
                        {
                            Console.WriteLine("Неверный пароль.");
                            currentUser = null;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Пользователь не найден.");
                        currentUser = null;
                    }
                }
                else
                {

                    if (currentUser.Type == "student")
                    {
                        while (ImAStudent)
                        {
                            Console.WriteLine("\nМеню для ученика:");
                            Console.WriteLine("1. Отобразить оценки");
                            Console.WriteLine("2. Отобразить посещаемость");
                            Console.WriteLine("3. Выход из профиля");
                            Console.Write("Введите номер действия: ");

                            int choice = int.Parse(Console.ReadLine());

                            switch (choice)
                            {
                                case 1:
                                    Console.Write("Введите ID студента: ");
                                    int studentId = int.Parse(Console.ReadLine());
                                    attendanceManager.ListGrades(studentId);
                                    break;
                                case 2:
                                    Console.Write("Введите ID студента: ");
                                    int attendanceStudentId = int.Parse(Console.ReadLine());
                                    attendanceManager.ListAttendance(attendanceStudentId);
                                    break;
                                case 3:
                                    Console.WriteLine("Выход из профиля.");
                                    currentUser = null;
                                    ImAStudent = false;
                                    break;
                                default:
                                    Console.WriteLine("Некорректный выбор.");
                                    break;
                            }
                        }
                    }

                    else if (currentUser.Type == "teacher")
                    {
                        bool ImATeacher = true;
                        while (ImATeacher)
                        {
                            Console.WriteLine("\nМеню для преподавателя:");
                            Console.WriteLine("1. Добавить студента");
                            Console.WriteLine("2. Удалить студента");
                            Console.WriteLine("3. Редактировать студента");
                            Console.WriteLine("4. Отобразить список студентов");
                            Console.WriteLine("5. Добавить запись о посещении");
                            Console.WriteLine("6. Отобразить записи о посещении");
                            Console.WriteLine("7. Добавить оценку");
                            Console.WriteLine("8. Отобразить оценки");
                            Console.WriteLine("9. Выход из профиля");
                            Console.Write("Введите номер действия: ");

                            int choice = int.Parse(Console.ReadLine());

                            switch (choice)
                            {
                                case 1:
                                    Console.Write("Введите имя студента: ");
                                    string name = Console.ReadLine();
                                    Console.Write("Введите email студента: ");
                                    string email = Console.ReadLine();
                                    attendanceManager.AddStudent(new Student(0, name, email));
                                    break;
                                case 2:
                                    Console.Write("Введите ID студента для удаления: ");
                                    int idToRemove = int.Parse(Console.ReadLine());
                                    attendanceManager.RemoveStudent(idToRemove);
                                    break;
                                case 3:
                                    Console.Write("Введите ID студента для редактирования: ");
                                    int idToEdit = int.Parse(Console.ReadLine());
                                    Console.Write("Введите новое имя студента: ");
                                    string newName = Console.ReadLine();
                                    Console.Write("Введите новый email студента: ");
                                    string newEmail = Console.ReadLine();
                                    attendanceManager.EditStudent(idToEdit, new Student(idToEdit, newName, newEmail));
                                    break;
                                case 4:
                                    attendanceManager.ListStudents();
                                    break;
                                case 5:
                                    Console.Write("Введите ID студента: ");
                                    int studentId = int.Parse(Console.ReadLine());
                                    Console.Write("Введите дату посещения (ДД.ММ.ГГГГ): ");
                                    DateTime date = DateTime.Parse(Console.ReadLine());
                                    Console.Write("Присутствовал ли студент (да/нет)? ");
                                    bool isPresent = Console.ReadLine().ToLower() == "да";
                                    attendanceManager.AddAttendance(studentId, new AttendanceRecord(date, isPresent));
                                    break;
                                case 6:
                                    Console.Write("Введите ID студента: ");
                                    int attendanceStudentId = int.Parse(Console.ReadLine());
                                    attendanceManager.ListAttendance(attendanceStudentId);
                                    break;
                                case 7:
                                    Console.Write("Введите ID студента: ");
                                    int gradeStudentId = int.Parse(Console.ReadLine());
                                    Console.Write("Введите предмет: ");
                                    string subject = Console.ReadLine();
                                    Console.Write("Введите оценку: ");
                                    int grade = int.Parse(Console.ReadLine());
                                    attendanceManager.AddGrade(gradeStudentId, subject, grade);
                                    break;
                                case 8:
                                    Console.Write("Введите ID студента: ");
                                    int gradeListStudentId = int.Parse(Console.ReadLine());
                                    attendanceManager.ListGrades(gradeListStudentId);
                                    break;
                                case 9:
                                    Console.WriteLine("Выход из профиля.");
                                    currentUser = null;
                                    ImATeacher = false;
                                    break;
                                default:
                                    Console.WriteLine("Некорректный выбор.");
                                    break;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Неверный тип пользователя.");
                    }

                    Console.WriteLine("\nВыйти из профиля (да/нет)?");
                    string exitChoice = Console.ReadLine().ToLower();
                    if (exitChoice == "да")
                    {
                        currentUser = null;
                        Console.WriteLine("Выход из профиля.");
                    }
                }
            }
        }
    }
}