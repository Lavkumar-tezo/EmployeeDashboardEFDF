namespace EmployeeDirectory.BAL.Extension
{
    public static class StringExtension
    {
        public static bool IsEmpty(this string value) => string.IsNullOrWhiteSpace(value) && string.IsNullOrEmpty(value);
    }
}
