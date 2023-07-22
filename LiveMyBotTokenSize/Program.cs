using static ChessChallenge.Application.TokenCounter;
using Float64 = System.Double;

const int UnknownTokenCount = -1;
const int BarWidth = 64;

// Get file path
string thisPath = Directory.GetCurrentDirectory();
string subfolder = thisPath;
for (int i = 0; i < 4; i++)
    subfolder = Path.GetDirectoryName(subfolder)!;
string pathToFile = Path.Combine(subfolder, "Chess-Challenge", "src", "My Bot", "MyBot.cs");
string fileName = Path.GetFileName(pathToFile);
int tokens = UnknownTokenCount;
Console.CursorVisible = false;

while (true)
{
    // Try get file
    string code = "";
    string error = "";
    try
    {
        code = File.ReadAllText(pathToFile);
    }
    catch (Exception e)
    {
        error = e.Message;
    }

    // Get tokens count
    if (code != "")
    {
        tokens = CountTokens(code);
    }
    else
    {
        tokens = UnknownTokenCount;
    }

    // Show tokens count
    Console.ResetColor();
    Console.SetCursorPosition(0, 0);
    Console.WriteLine("Name of file: " + fileName);

    if (tokens != UnknownTokenCount)
    {
        Console.WriteLine("Tokens count: " + tokens + "/1024    ");

        if (tokens < 1024)
        {
            Float64 barPercent = (Float64)tokens / 1024;
            int barSize = (int)(barPercent * BarWidth);
            Console.Write("[");
            if (barPercent < 0.40)
                Console.ForegroundColor = ConsoleColor.Green;
            else if (barPercent < 0.80)
                Console.ForegroundColor = ConsoleColor.Yellow;
            else
                Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(new string('█', barSize));
            Console.Write(new string(' ', BarWidth - barSize));
            Console.ResetColor();
            Console.Write("]");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Maxed");
            Console.ResetColor();
        }
    }
    else
    {
        Console.WriteLine("Failed to get tokens count");
        Console.WriteLine(error);
    }


    Thread.Sleep(500);
}