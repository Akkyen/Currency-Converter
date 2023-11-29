using System.Net.Http;

namespace Currency_Converter.ViewModel;

public static class Utils
{
    public static async Task<string> WebApiCall(string url)
    {
        string rValue = "";

        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url).ConfigureAwait(false);

                // Throws an exception if the HTTP response status is an error
                response.EnsureSuccessStatusCode();

               rValue = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
        }

        return rValue;
    }
}