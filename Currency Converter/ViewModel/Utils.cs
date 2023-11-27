using System.Net.Http;

namespace Currency_Converter.View;

public static class Utils
{
    public static async Task<String> ApiCall(String call)
    {
        String rValue = "";

        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(call).ConfigureAwait(false);

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