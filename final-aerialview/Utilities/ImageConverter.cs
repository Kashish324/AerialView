namespace final_aerialview.Utilities
{
    #region converting projectsettings logo
    public class ImageConverter
    {
        public static string ConvertHexToBase64(string hexString)
        {
            byte[] bytes = Enumerable.Range(0, hexString.Length)
                                     .Where(x => x % 2 == 0)
                                     .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                                     .ToArray();
            return Convert.ToBase64String(bytes);
        }
    }
    #endregion
}
