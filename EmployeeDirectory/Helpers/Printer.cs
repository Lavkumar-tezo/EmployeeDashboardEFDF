using EmployeeDirectory.BAL.Extension;
namespace EmployeeDirectory.Helpers
{
    internal static class Printer
    {
        /// <summary>
        /// // Print messages on screen
        /// </summary>
        /// <param name="next"></param>
        /// <param name="data"></param>
        public static void Print(bool next, params string[] data)
        {
            if (!next)
            {
                Console.Write(data[0]);
            }
            else
            {
                foreach (var item in data)
                {
                    Console.WriteLine(item);
                }
            }
        }

    }
}
