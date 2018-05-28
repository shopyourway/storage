using OhioBox.Moranbernate.Mapping;

namespace OhioBox.Storage.MySql.Tests.Mapping
{
	public class UserDtoMap : ClassMap<UserDto>
	{
		public UserDtoMap()
		{
			Table("users");

			Id(x => x.Id).GeneratedBy.Assigned();
			Map(x => x.Name);
			Map(x => x.UpdateDate);
			Map(x => x.VisitCount);
		}
	}
}