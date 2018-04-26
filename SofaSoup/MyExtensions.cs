namespace SofaSoupApp
{
    
    public static class MyExtensions
    {
        // Replicating pythons string*int
        public static string Times(this string str, int times)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int i = 0; i < times; i++)
                sb.Append(str);

            return sb.ToString();
        }
    }
}
