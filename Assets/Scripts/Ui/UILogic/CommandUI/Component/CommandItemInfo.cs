public class CommandItemInfo
{
    public  CommandType cacheCommandEnum;
    public  int cacheCount;
    public  int cacheTime;
}

public class WaitingItemInfo:CommandItemInfo
{
    
}

public class UsedItemInfo : CommandItemInfo
{
    public int currentTime;
}
