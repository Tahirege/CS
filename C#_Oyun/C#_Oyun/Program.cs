using System;
using System.Diagnostics;
using System.IO; 

class Program
{
    
    static string logPath = "game_log.txt";

    static void Main()
    {
      
        File.WriteAllText(logPath, "--- OYUN BASLADI ---\n");

        bool isGameOn = true;
        bool makeNewObject = true;
        int score = 0;
        int gameTime = 2 * 60;
        int width = 90;
        int height = 20;

        try
        {
            Console.SetWindowSize(width, height);
            Console.SetBufferSize(width, height);
        }
        catch
        { 
           // Linux ve MacOS'de  Console.SetWindowSize(width, height) çalışmıyor
        }

            width = Console.WindowWidth;
        height = Console.WindowHeight;

        int CharX = width / 2;
        int CharY = height - 1;
        int objectX = 0;
        int objectY = 0;

        const int targetFPS = 120;
        const double frameTime = 1000.0 / targetFPS;
        var stopwatch = new Stopwatch();
        var random = new Random();
        double lastFallTime = 0.0;

        stopwatch.Start();
        WriteCharacter(CharX, CharY);

        while (isGameOn)
        {
            double frameStart = stopwatch.Elapsed.TotalMilliseconds;
            double currentTime = stopwatch.Elapsed.TotalSeconds;

            if (makeNewObject)
            {
                objectX = random.Next(0, width);
                objectY = 0;
                makeNewObject = false;
                // LOG: Yeni nesne doğuşu
                LogYaz($"UPDATE → itemSpawned x={objectX} y={objectY}");
            }

            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                DeleteLastCharecter(CharX, CharY);

                if (key == ConsoleKey.RightArrow)
                {
                    if (CharX < width - 2) CharX++;
                    LogYaz($"INPUT → key=RightArrow playerX={CharX} playerY={CharY}");
                }
                if (key == ConsoleKey.LeftArrow)
                {
                    if (CharX > 1) CharX--;
                    LogYaz($"INPUT → key=LeftArrow playerX={CharX} playerY={CharY}");
                }
            }

            int time = gameTime - (int)stopwatch.Elapsed.TotalSeconds;
            if (time < 0) isGameOn = false;

            WriteScore(score);
            WriteTime(time, width);
            WriteFallingObject(objectX, objectY);

            if (currentTime - lastFallTime >= 0.25)
            {
                DeleteLastObject(objectX, objectY);
                objectY++;
                lastFallTime = currentTime;
                // LOG: Nesne hareketi
                LogYaz($"UPDATE → objectMoved x={objectX} y={objectY}");
            }

            // Çarpışma kontrolü
            if (objectY == CharY - 1 && (objectX >= CharX - 1 && objectX <= CharX + 1))
            {
                makeNewObject = true;
                score += 50;
                DeleteLastObject(objectX, objectY - 1);
                // LOG: Çarpışma ve Skor
                LogYaz($"COLLISION → score={score} playerX={CharX} objectX={objectX}");
            }

            if (objectY == height - 1)
            {
                makeNewObject = true;
                DeleteLastObject(objectX, objectY - 1);
            }

            WriteCharacter(CharX, CharY);

            double frameEnd = stopwatch.Elapsed.TotalMilliseconds;
            int sleepTime = (int)(frameTime - (frameEnd - frameStart));
            if (sleepTime > 0) Thread.Sleep(sleepTime);
        }
        Console.Clear();
        LogYaz($"GAME OVER → Final Score={score}");
        GameOverScreen(height, width, score);
    }

    
    public static void LogYaz(string mesaj)
    {
        using (StreamWriter sw = File.AppendText(logPath))
        {
            sw.WriteLine(mesaj);
        }
    }

    
    public static void WriteCharacter(int x, int y)
    {
        Console.SetCursorPosition(x, y);
        Console.Write("@");
        Console.SetCursorPosition(x - 1, y - 1);
        Console.Write("___");
    }
    public static void WriteFallingObject(int x, int y)
    {
        Console.SetCursorPosition(x, y);
        Console.Write("*");
    }
    public static void DeleteLastObject(int x, int y)
    {
        Console.SetCursorPosition(x, y);
        Console.Write(" ");
    }
    public static void DeleteLastCharecter(int x, int y)
    {
        Console.SetCursorPosition(x - 1, y - 1);
        Console.Write("   ");
        Console.SetCursorPosition(x, y);
        Console.Write(" ");
    }
    public static void WriteScore(int score)
    {
        Console.SetCursorPosition(0, 0);
        Console.Write($"SKOR: {score:D4}");
    }
    public static void WriteTime(int time, int width)
    {
        Console.SetCursorPosition(width - 11, 0);
        Console.Write($"TIME: {time / 60:D2}:{time % 60:D2}");
    }
    static void GameOverScreen(int height, int width, int score)
    {
        Console.Clear();
        Console.WriteLine($"OYUN BITTI! SKOR: {score}");
        Console.WriteLine("Cikmak icin X");
        while (Console.ReadKey(true).Key != ConsoleKey.X) ;
    }
}