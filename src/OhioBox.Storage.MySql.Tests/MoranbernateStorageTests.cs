using System;
using NUnit.Framework;
using OhioBox.Storage.MySql.Bootstrap;
using OhioBox.Storage.MySql.Moranbernate;
using OhioBox.Storage.MySql.Tests.Bootstrap;
using OhioBox.Storage.MySql.Tests.Mapping;

namespace OhioBox.Storage.MySql.Tests
{
	[TestFixture]
	public class MoranbernateStorageTests
	{
		private ISqlConnectionFactory _sqlConnectionFactory;

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			_sqlConnectionFactory = Bootstrapper.Init();
		}


		private IStorage<UserDto> _target;

		[SetUp]
		public void Setup()
		{
			_target = new MoranbernateStorage<UserDto>(_sqlConnectionFactory, new DummyLogger(), new DefaultMetricsReporter());
			ClearDatabase();
		}

		[TearDown]
		public void TearDown()
		{
			ClearDatabase();
		}

		private void ClearDatabase()
		{
			var all = _target.Query(q => q.IsNotNull(x => x.Id));
			_target.Delete(all);
		}

		[Test]
		public void Query_WhenCalled_ReturnsItemsByPredicate()
		{
			AddUser(1L, "Doron");
			AddUser(2L, "Shani");
			AddUser(3L, "Tomer");

			var result = _target.Query(q => q.Equal(x => x.Name, "Shani"));

			Assert.That(result, Has.Count.EqualTo(1));
			Assert.That(result, Has.Some.Matches<UserDto>(u => u.Id == 2L && u.Name == "Shani"));
		}

		[Test]
		public void Query_WhenQueryWithStartsWith_ReturnAllUsersStartingWithGivenPrefix()
		{
			AddUser(1L, "Doron");
			AddUser(2L, "Shani");
			AddUser(3L, "Dror");
			AddUser(4L, "Danni");

			var result = _target.Query(q => q.StartWith(x => x.Name, "D"));

			Assert.That(result, Has.Count.EqualTo(3));
			Assert.That(result, Has.Some.Matches<UserDto>(x => x.Id == 1L && x.Name == "Doron"));
			Assert.That(result, Has.Some.Matches<UserDto>(x => x.Id == 3L && x.Name == "Dror"));
			Assert.That(result, Has.Some.Matches<UserDto>(x => x.Id == 4L && x.Name == "Danni"));
		}
		
		[Test]
		public void Query_WhenQueryWithContains_ReturnAllUsersWithNameContainingGivenString()
		{
			AddUser(1L, "Doron");
			AddUser(2L, "Shani");
			AddUser(3L, "Dror");
			AddUser(4L, "Danni");

			var result = _target.Query(q => q.Contains(x => x.Name, "a"));

			Assert.That(result, Has.Count.EqualTo(2));
			Assert.That(result, Has.Some.Matches<UserDto>(x => x.Id == 2L && x.Name == "Shani"));
			Assert.That(result, Has.Some.Matches<UserDto>(x => x.Id == 4L && x.Name == "Danni"));
		}

		[Test]
		public void Query_WhenQueryHasInAny_DoNothingSinceCollectionsAreNotSupported()
		{
			AddUser(1L, "Doron", permissions: new [] { "read", "write" });
			AddUser(2L, "Shani", permissions: new [] { "read" } );

			var result = _target.Query(q => q.InAny(x => x.Permissions, new[] { "write" }));

			Assert.That(result, Has.Count.EqualTo(2));
			Assert.That(result, Has.Some.Matches<UserDto>(x => x.Id == 1L));
			Assert.That(result, Has.Some.Matches<UserDto>(x => x.Id == 2L));
		}

		[Test]
		public void Query_WhenQueryHasNotInAny_DoNothingSinceCollectionsAreNotSupported()
		{
			AddUser(1L, "Doron", permissions: new[] { "read", "write" });
			AddUser(2L, "Shani", permissions: new[] { "read" });

			var result = _target.Query(q => q.NotInAny(x => x.Permissions, new[] { "write" }));

			Assert.That(result, Has.Count.EqualTo(2));
			Assert.That(result, Has.Some.Matches<UserDto>(x => x.Id == 1L));
			Assert.That(result, Has.Some.Matches<UserDto>(x => x.Id == 2L));
		}

		[Test]
		public void UpdateByQuery_WhenCalled_UpdatesOnlyRelevantRows()
		{
			AddUser(1L, "Doron");
			AddUser(2L, "Shani");
			AddUser(3L, "Dror");

			var result = _target.UpdateByQuery(q => q.GreaterOrEqual(x => x.Id, 2), u => u.Set(x => x.Name, "TEST"));

			Assert.That(result, Is.EqualTo(2));

			var users = _target.Query(q => {});
			Assert.That(users, Has.Some.Matches<UserDto>(x => x.Id == 1L && x.Name == "Doron"));
			Assert.That(users, Has.Some.Matches<UserDto>(x => x.Id == 2L && x.Name == "TEST"));
			Assert.That(users, Has.Some.Matches<UserDto>(x => x.Id == 3L && x.Name == "TEST"));
		}

		[Test]
		public void UpdateByQuery_WhenUpdatingSeveralFields_UpdatesAllFieldsInRelevantRows()
		{
			AddUser(1L, "Doron", new DateTime(2018, 5, 10));
			AddUser(2L, "Shani", new DateTime(2018, 5, 11));
			AddUser(3L, "Dror", new DateTime(2018, 5, 12));

			var result = _target.UpdateByQuery(q => q.GreaterOrEqual(x => x.Id, 2), u => u.Set(x => x.Name, "TEST").Set(x => x.UpdateDate, new DateTime(2018, 5, 21)));

			Assert.That(result, Is.EqualTo(2));

			var users = _target.Query(q => { });
			Assert.That(users, Has.Some.Matches<UserDto>(x => x.Id == 1L && x.Name == "Doron" && x.UpdateDate == new DateTime(2018, 5, 10)));
			Assert.That(users, Has.Some.Matches<UserDto>(x => x.Id == 2L && x.Name == "TEST" && x.UpdateDate == new DateTime(2018, 5, 21)));
			Assert.That(users, Has.Some.Matches<UserDto>(x => x.Id == 3L && x.Name == "TEST" && x.UpdateDate == new DateTime(2018, 5, 21)));
		}

		[Test]
		public void UpdateByQuery_WhenFieldIsIncremented_IncrementFieldBySpecifiedValue()
		{
			AddUser(1L, "Doron", visitCount: 1);
			AddUser(2L, "Shani", visitCount: 2);
			AddUser(3L, "Dror", visitCount: 3);

			var result = _target.UpdateByQuery(q => q.GreaterOrEqual(x => x.Id, 2), u => u.Set(x => x.Name, "TEST").Increment(x => x.VisitCount, 1));

			Assert.That(result, Is.EqualTo(2));

			var users = _target.Query(q => { });
			Assert.That(users, Has.Some.Matches<UserDto>(x => x.Id == 1L && x.Name == "Doron" && x.VisitCount == 1));
			Assert.That(users, Has.Some.Matches<UserDto>(x => x.Id == 2L && x.Name == "TEST" && x.VisitCount == 3));
			Assert.That(users, Has.Some.Matches<UserDto>(x => x.Id == 3L && x.Name == "TEST" && x.VisitCount == 4));
		}

		[Test]
		public void UpdateByQuery_WhenAddToSetIsUsed_DoNothingCauseThereIsNoCollectionSupportInMoranbernate()
		{
			AddUser(1L, "Doron", visitCount: 1);
			AddUser(2L, "Shani", visitCount: 2);

			Assert.DoesNotThrow(() => _target.UpdateByQuery(q => q.GreaterOrEqual(x => x.Id, 2), u => u.Set(x => x.Name, "TEST").AddToSet(x => x.Permissions, "read")));
		}

		private void AddUser(long id, string name, DateTime? updateDate = null, int? visitCount = null, string[] permissions = null)
		{
			_target.Add(new UserDto { Id = id, Name = name, UpdateDate = updateDate, VisitCount = visitCount, Permissions = permissions});
		}

		private class DummyLogger : IPerfLogger<UserDto>
		{
			public void Error(string errorText, Exception exception)
			{

			}

			public void Warn(string message, string newLine, string stackTrace)
			{

			}
		}
	}


}