public class Singleton<T> where T : class , new()
{
    protected static T s_instance = null ;

    static public T getInstance()
    {
        if ( s_instance == null )
        {
            s_instance = new T();
        }
        return s_instance ;
    }
}
