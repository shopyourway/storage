using System;

namespace OhioBox.Storage.MySql.Tests.Mapping
{
	public class UserDto
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public DateTime? UpdateDate { get; set; }
		public int? VisitCount { get; set; }
	}
}