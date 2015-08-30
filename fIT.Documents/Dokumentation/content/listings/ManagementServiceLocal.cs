public static ManagementService service { get; private set; }

public async Task<bool> SignIn(string username, string password)
{
    bool result = false;
    this.username = username;
    this.password = password;
    try
    {
        session = await getSession(username, password);
        result = true;
    }
    catch (ServerException e){[...]throw;}
    catch (Exception exc){[...]}
    return result;
}