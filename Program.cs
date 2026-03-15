

namespace S3Snake
{
    public static class Program
    {
        static void Main()
        {
            using var game = new S3SnakeGame();
            game.Run();
        }
    }
}

