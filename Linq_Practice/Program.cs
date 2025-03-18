// See https://aka.ms/new-console-template for more information
using Linq_Practice;
using System.Collections.Concurrent;
using System.Diagnostics;

Console.WriteLine("Hello, World!");

#region Input


int[] scores = new int[5] { 10, 222, 33, 4,50 };
List<Student> slist = new List<Student>();

List<Department> dlist = new List<Department>();
dlist.Add(new Department(1, "It"));
dlist.Add(new Department(2, "operations"));
dlist.Add(new Department(3, "marketing"));

slist.Add(new Student(1, "ravi",1));
slist.Add(new Student(2, "somesha",2));
slist.Add(new Student(3, "chandu",3));
slist.Add(new Student(4, "ravi",2));


slist[0].marks.Add(44);
slist[0].marks.Add(55);
slist[0].marks.Add(66);

slist[1].marks.Add(55);
slist[1].marks.Add(55);
slist[1].marks.Add(66);

slist[2].marks.Add(64);
slist[2].marks.Add(55);
slist[2].marks.Add(66);

slist[3].marks.Add(64);
slist[3].marks.Add(67);
slist[3].marks.Add(66);

List<int> nums = Enumerable.Range(1, 100000000).ToList();
#endregion


#region orderBy


IEnumerable<int> sortedScores = from score in scores orderby score select score;

//foreach (int score in sortedScores)
//{
//    Console.WriteLine("the score is:" + score);
//}

#endregion

#region Multi Query

var students = slist.Where(s => s.marks[0] > 50);
var rstudents = students.Where(s => s.name.Contains('a'));

//foreach(Student s in rstudents)
//{
//    Console.WriteLine(s.name);
//}
#endregion

#region group By
var sameStudents = from student in slist group student by student.name;

//foreach (var s in sameStudents)
//{
//    Console.WriteLine("Students with name:-" + s.Key);
//    foreach(Student a in s)
//    {
//        Console.WriteLine("  id:-" + a.id +" name:-"+a.name);

//    }
//}
#endregion

#region Joins
var studept = from student in slist join dept in dlist on student.deptId equals dept.dId
                select new { name = student.name, dname = dept.deptName };

//foreach (var a in studept)
//{
//    Console.WriteLine(a);
//}

#endregion

#region SubQuery
var marksDept = from student in slist
                group student by student.deptId into deptgroup
                select new
                {
                    name = deptgroup.Key,
                    avgMarks = (from student2 in deptgroup select student2.marks.Average()).Average()
                };

//foreach (var s in marksDept)
//{
//    Console.WriteLine("dept id:-" + s.name+" avg marks:-"+s.avgMarks);
   
//}

#endregion

#region subQuery with Join

var avgDept = from student in slist
              join dept in dlist
               on student.deptId equals dept.dId
                group student by dept.deptName into deptgroup
                select new
                {
                    name = deptgroup.Key,
                    avgMarks = (from student2 in deptgroup select student2.marks.Average()).Average()
                };

//foreach (var s in avgDept)
//{
//    Console.WriteLine("dept name:-" + s.name + " avg marks:-" + s.avgMarks);

//}
#endregion

#region Parallel Linq
var parallelResultEven = nums.AsParallel().AsOrdered().Where(a => isEven(a)).OrderByDescending(n=>n);


//foreach (int n in parallelResultEven.Take(50))
//{
//    Console.WriteLine("Even no:-"+n);
//}


    static bool isEven(int n)
    {
        //Thread.Sleep(1);
        return n % 2 == 0;
    }
#endregion

#region Batch Result Yield
    static IEnumerable<Student> GetStudentsInBatch(int batchSize,List<Student> students)
    {
        for (int i = 0; i < students.Count; i += batchSize) 
        {
            Thread.Sleep(1000);
            IEnumerable<Student> batch = students.Skip(i).Take(batchSize);
            foreach(var student in batch)
            {
             yield return student;
            }
        }
    }

//adding large no of student
//List<Student> stulist = new List<Student>();
//for (int i = 1; i <= 1000; i++)  // Simulate 1000 students
//{
//    stulist.Add(new Student(i, $"Student {i}", i % 3 + 1));
//}

int batchsize = 100;
//foreach(Student student in GetStudentsInBatch(batchsize, stulist))
//{
//    Console.WriteLine(student.ToString());
//    Thread.Sleep(1);
//}
#endregion

#region Batch result
List<Student> stulist = new List<Student>();
for (int i = 1; i <= 1000; i++)  // Simulate 1000 students
{
    stulist.Add(new Student(i, $"Student {i}", i % 3 + 1));
}

Stopwatch sw = Stopwatch.StartNew();
ConcurrentBag<Student> result = new ConcurrentBag<Student>();

int chunksize = 100;
var studentchunk = stulist.Chunk(chunksize);


Parallel.ForEach(studentchunk, chunk =>            ///Takes only 1444 ms to add 1000 students
{

    foreach (Student student in chunk)
    {

        Thread.Sleep(5);
        result.Add(student);
    }
});
//foreach(Student student in stulist)             //Without parallel takes 14000 ms to add students
//{
//    Thread.Sleep(5);
//    result.Add(student);
//}

sw.Stop();

Console.WriteLine($"Total execution time: {sw.ElapsedMilliseconds} ms");
Console.WriteLine($"Number of students processed: {result.Count}");

#endregion