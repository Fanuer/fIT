public async Task<Guid> SignIn(string username, string password)
{
    User user = new fITNat.DBModels.User();
    Guid userId;
    user.Username = username;
    user.Password = password;
    if (Online)
    {
        try
        {
            bool success = await mgnService.SignIn(username, password);
            if (success)
            {
                userId = mgnService.actualSession().CurrentUserId;
                user.UserId = userId.ToString();
                if (db != null)
                {
                    db.insertUpdateUser(user);
                }
                return userId;
            }
        }
        catch(ServerException ex){[...]throw;}
        catch(Exception exc){[...]}
        return new Guid();
    }
    else
    {
        try
        {
            //Lokal nachschauen
            Guid result = db.findUser(username, password);
            if (result != null)
                return result;
        }
        catch(Exception exc){[...]throw;}
        return new Guid();
    }
} 