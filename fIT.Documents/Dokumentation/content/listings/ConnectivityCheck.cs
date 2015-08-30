Task.Run(async () =>
{
    while (true)
    {
        bool status = false;
        try
        {
            status = await mgnServiceServer.PingAsync();
        }
        catch(Exception ex)
        {
            status = false;
        }
        finally
        {
            if (status)
            {
                //Online = true;
                setzeStatus(true);
                //vorher Offline => jetzt die Aktionen ausführen, die nur lokal gemacht werden konnten
                if (WasOffline)
                {
                    await checkSync();
                    setzeWasOffline(false);
                }
            }
            else
            {
                setzeStatus(false);
                setzeWasOffline(true);
            }
            //Timeout 10sek.
            System.Threading.Thread.Sleep(10000);
        }
    }
});