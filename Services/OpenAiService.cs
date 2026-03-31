using MovieWatchlistAPI.Services.Interfaces;
using OpenAI;
using OpenAI.Chat;

namespace MovieWatchlistAPI.Services
{
    public class OpenAiService(OpenAIClient openAiClient) : IAiService
    {
        // This method generates a catchy movie pitch using the OpenAI API based on the movie's title and plot.
        public async Task<string> GenerateMoviePitchAsync(string title, string plot)
        {
            try
            {
                // We use the "gpt-4o-mini" model for generating the pitch, which is a smaller and faster version of GPT-4.
                var chatClient = openAiClient.GetChatClient("gpt-4o-mini");

                // We construct a prompt that instructs the AI to act as a funny movie critic and create a hilarious 1-sentence pitch for the given movie title and plot.
                string prompt = $"You are a funny movie critic. Write a hilarious 1-sentence pitch for '{title}' based on: {plot}. Max 25 words.";

                // We call the OpenAI API to get the completion (the generated pitch) and return the text content of the first choice.
                ChatCompletion completion = await chatClient.CompleteChatAsync(prompt);
                return completion.Content[0].Text;
            }
            catch { return "A great movie for your list!"; } // In case of any errors (e.g., API issues), we return a default pitch to ensure the user still gets a response.
        }
    }
}
