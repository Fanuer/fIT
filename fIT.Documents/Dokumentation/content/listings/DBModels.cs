[Table("User")]
public class User
{
    [PrimaryKey, AutoIncrement]
    public int LocalId { get; set; }
    public bool wasOffline { get; set; }
    public string UserId { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public override string ToString()
    {
        return string.Format("[User: LocalId={0}, UserId={1}, Username={2}, Password={3}]", LocalId, UserId, Username, Password);
    }
}