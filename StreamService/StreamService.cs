using Newtonsoft.Json;

namespace StreamServiceSpace
{
    public class StreamService<T>
    {
        private readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        public async Task WriteToStreamAsync(Stream stream, IEnumerable<T> data, IProgress<string> progress)
        {
            await semaphore.WaitAsync();

            try
            {
                progress.Report($"Writing to stream on Thread {Thread.CurrentThread.ManagedThreadId}");

                int count = 0;
                foreach (var item in data)
                {
                    // Simulate slow write operation
                    await Task.Delay(10);

                    var serializedData = JsonConvert.SerializeObject(item) + Environment.NewLine;
                    var buffer = System.Text.Encoding.UTF8.GetBytes(serializedData);
                    await stream.WriteAsync(buffer, 0, buffer.Length);

                    count++;
                    var percentage = (int)((double)count / data.Count() * 100);
                    progress.Report($"Writing: {percentage}% completed on Thread {Thread.CurrentThread.ManagedThreadId}");
                }

                progress.Report($"Writing completed on Thread {Thread.CurrentThread.ManagedThreadId}");
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task CopyFromStreamAsync(Stream stream, string filename, IProgress<string> progress)
        {
            await semaphore.WaitAsync();

            try
            {
                progress.Report($"Copying from stream on Thread {Thread.CurrentThread.ManagedThreadId}");

                using (var fileStream = File.Create(filename))
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.CopyTo(fileStream);
                }

                progress.Report($"Copying completed on Thread {Thread.CurrentThread.ManagedThreadId}");
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task<int> GetStatisticsAsync(string fileName, Func<T, bool> filter)
        {
            await semaphore.WaitAsync();

            try
            {
                var count = 0;

                using (var fileStream = new FileStream(fileName, FileMode.Open))
                using (var reader = new StreamReader(fileStream))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        var item = JsonConvert.DeserializeObject<T>(line);

                        if (filter(item))
                        {
                            count++;
                        }
                    }
                }

                return count;
            }
            finally
            { 
                semaphore.Release(); 
            }
        }
    }
}