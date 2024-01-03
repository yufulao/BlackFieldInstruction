using System;

public static class CommandIndex
{
    public const int Up = 0;
    public const int Down = 1;
    public const int Right = 2;
    public const int Left = 3;
    public const int Wait = 4;

    public static int GetCommandIndexByType(CommandType enumType)
    {
        switch (enumType)
        {
            case CommandType.Up:
                return Up;
            case CommandType.Down:
                return Down;
            case CommandType.Right:
                return Right;
            case CommandType.Left:
                return Left;
            case CommandType.Wait:
                return Wait;
        }

        return -1;
    }
}

public enum CommandType///指令的种类
{
    //移动
    Up,//上
    Down,//下
    Right,//右
    Left,//左
    Wait//等待
}