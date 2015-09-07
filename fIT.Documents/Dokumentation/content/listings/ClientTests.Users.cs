[TestMethod]
public void UpdateCurrentUserData()
{
	const string NEWNAME = "TestUser";

	using (var service = new ManagementService(ServiceUrl)) |\label{line:ClientTests:Disposable}|
	using (IManagementSession session = service.LoginAsync(USERNAME, PASSWORD).Result)
	{
		var data = session.Users.GetUserDataAsync().Result; |\label{line:ClientTests:Async}|
		Assert.AreEqual(USERNAME, data.UserName);

		try
		{
			data.UserName = NEWNAME;
			session.Users.UpdateUserDataAsync(data).Wait();

			data = session.Users.GetUserDataAsync().Result;
			Assert.AreEqual(NEWNAME, data.UserName);
		}
		finally
		{
			data.UserName = USERNAME;
			session.Users.UpdateUserDataAsync(data).Wait();
		}
	}
}
