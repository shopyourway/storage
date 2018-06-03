using System.Configuration;
using NUnit.Framework;
using OhioBox.Storage.MySql.Bootstrap;

namespace OhioBox.Storage.MySql.Tests
{
	[TestFixture]
	public class MoranbernateBootstrapTests
	{
		[Test]
		public void GetCandidates_WhenGivenTypeIsNotPerfLogger_ThrowException()
		{
			Assert.Throws<StorageRegistrationException>(() => MoranbernateBootstrap.GetRegistrationCandidates<DefaultMetricsReporter>(this.GetType()));
		}

		[Test]
		public void GetCandidates_WhenGivenTypeIsPerfLogger_DoNotThrowException()
		{
			Assert.DoesNotThrow(() => MoranbernateBootstrap.GetRegistrationCandidates<DefaultMetricsReporter>(typeof(DefaultPerfLogger<>)));
		}
	}
}