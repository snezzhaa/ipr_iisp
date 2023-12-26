namespace Student
{
    public class StudentData
    {
        private Guid _id;
        private string _studentName;
        private int _averageScore;

        public Guid Id { get { return _id; } }
        public string StudentName { get { return _studentName; } }
        public int AverageScore { get { return _averageScore; } }

        public StudentData()
        {
            _id = Guid.NewGuid();
            _studentName = "testName";
            _averageScore = new Random().Next(11);
        }

        public StudentData(string name, int score)
        {
            _id = Guid.NewGuid();
            _studentName = name;
            _averageScore = score;
        }
    }
}