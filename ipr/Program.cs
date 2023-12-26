using StreamServiceSpace;
using Student;

class Program
{
    static async Task Main(string[] args)
    {
        IProgress<string> p = new Progress<string>(progress =>
        {
            Console.WriteLine(progress);
        });
        var fileName = "testFile";

        var studList = new List<StudentData>();
        for (int i = 0; i < 1000; i++)
        {
            studList.Add(new StudentData());
        }

        Console.WriteLine($"Start on Thread {Thread.CurrentThread.ManagedThreadId}");

        var streamService = new StreamService<StudentData>();
        MemoryStream stream = new MemoryStream();
        streamService.WriteToStreamAsync(stream, studList, p);
        await Task.Delay(200);
        streamService.CopyFromStreamAsync(stream, fileName, p);

        var studCount = await streamService.GetStatisticsAsync(fileName, (s) =>
        {
            return s.AverageScore > 9;
        });
        Console.WriteLine($"Studs with average score > 9: {studCount}");
    }
}