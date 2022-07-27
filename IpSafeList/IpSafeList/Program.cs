using cli.Builders;
using cli.Models;
using IpSafeList.Contexts;
using IpSafeList.Extensions;
using IpSafeList.Models;
using IpSafeList.Services;
using System.Net;

public class Program
{
	static void Main(string[] args)
	{
		CommandGroup main = CommandBuilder.CommandGroup("safelist");
		main.ShowHelpIfUnresolvable();
		main.Verb("help")
				.LoggedAction(args =>
				{
					Console.WriteLine(main.GetHelpText());
					return 0;
				})
			.Group("add")
				.Verb("identity")
					.Option("f", "firstName")
						.WithValidator(s => !string.IsNullOrWhiteSpace(s))
						.Required()
					.Option("l", "lastName")
						.WithValidator(s => !string.IsNullOrWhiteSpace(s))
						.Required()
					.Option("lx", "locked")
						.WithValidator(s => !string.IsNullOrWhiteSpace(s))
						.IsFlag()
					.LoggedAction(args =>
					{
						var entity = new IdentityEntity
						{
							FirstName = args.ValueOf("f", "firstName"),
							LastName = args.ValueOf("l", "lastName"),
							IsLocked = args.Get<bool>("lx", "locked"),
						};
						SafeListContext.Connection.Insert(entity);
						LoggerService.Log($"identity {entity.FirstName}, {entity.LastName} created successfully", LogSeverity.SUCCESS);
						return 0;
					})
				.Verb("entry")
					.Option("f", "firstName")
						.WithValidator(s => !string.IsNullOrWhiteSpace(s))
						.Required()
					.Option("l", "lastName")
						.WithValidator(s => !string.IsNullOrWhiteSpace(s))
						.Required()
					.Option("ip", "ipAddress")
						.WithValidator(s => !string.IsNullOrWhiteSpace(s))
						.Required()
					.LoggedAction(args =>
					{
						var firstName = args.ValueOf("f", "firstName");
						var lastName = args.ValueOf("l", "lastName");
						var ipAddress = args.ValueOf("ip", "ipAddress");
						if (!IPAddress.TryParse(ipAddress, out _)) 
							throw new Exception($"unable to parse {ipAddress} to valid IP address");
						var identity = SafeListContext.Connection.Select<IdentityEntity>().Where(id => id.FirstName == firstName && id.LastName == lastName).FirstOrDefault();
						if (identity == null)
							throw new Exception($"identity {lastName}, {firstName} not found");
						SafeListContext.Connection.Insert(new IpEntryEntity
						{
							IdentityId = identity.Id,
							IpAddress = ipAddress,
						});
						LoggerService.Log($"entry {ipAddress} added for identity {lastName}, {firstName}", LogSeverity.SUCCESS);
						return 0;
					})
			.Group("rm")
				.Verb("identity")
					.Option("f", "firstName")
						.WithValidator(s => !string.IsNullOrWhiteSpace(s))
						.Required()
					.Option("l", "lastName")
						.WithValidator(s => !string.IsNullOrWhiteSpace(s))
						.Required()
					.LoggedAction(args =>
					{
						var firstName = args.ValueOf("f", "firstName");
						var lastName = args.ValueOf("l", "lastName");
						var identity = SafeListContext.Connection.Select<IdentityEntity>().Where(id => id.FirstName == firstName && id.LastName == lastName).FirstOrDefault();
						if (identity == null)
							throw new Exception($"identity {lastName}, {firstName} not found");
						SafeListContext.Connection.Delete(identity);
						var entries = SafeListContext.Connection.Select<IpEntryEntity>().Where(ipEntry => ipEntry.IdentityId == identity.Id);
						foreach (var entry in entries) SafeListContext.Connection.Delete(entry);
						LoggerService.Log($"identity {lastName}, {firstName} deleted", LogSeverity.SUCCESS);
						return 0;
					})
				.Verb("entry")
					.Option("f", "firstName")
						.WithValidator(s => !string.IsNullOrWhiteSpace(s))
						.Required()
					.Option("l", "lastName")
						.WithValidator(s => !string.IsNullOrWhiteSpace(s))
						.Required()
					.Option("ip", "ipAddress")
						.WithValidator(s => !string.IsNullOrWhiteSpace(s))
						.Required()
					.LoggedAction(args =>
					{
						var firstName = args.ValueOf("f", "firstName");
						var lastName = args.ValueOf("l", "lastName");
						var ipAddress = args.ValueOf("ip", "ipAdress");
						var identity = SafeListContext.Connection.Select<IdentityEntity>().Where(id => id.FirstName == firstName && id.LastName == lastName).FirstOrDefault();
						if (identity == null)
							throw new Exception($"identity {lastName}, {firstName} not found");
						SafeListContext.Connection.Delete(identity);
						var entries = SafeListContext.Connection.Select<IpEntryEntity>().Where(ipEntry => ipEntry.IdentityId == identity.Id && ipEntry.IpAddress == ipAddress);
						foreach (var entry in entries) SafeListContext.Connection.Delete(entry);
						LoggerService.Log($"entry {ipAddress} for identity {lastName}, {firstName} deleted", LogSeverity.SUCCESS);
						return 0;
					})
			.Group("ls")
				.Verb("entries")
					.Option("f", "firstName")
						.WithValidator(s => !string.IsNullOrWhiteSpace(s))
						.Required()
					.Option("l", "lastName")
						.WithValidator(s => !string.IsNullOrWhiteSpace(s))
						.Required()
					.LoggedAction(args =>
					{
						var firstName = args.ValueOf("f", "firstName");
						var lastName = args.ValueOf("l", "lastName");
						var identity = SafeListContext.Connection.Select<IdentityEntity>().Where(id => id.FirstName == firstName && id.LastName == lastName).FirstOrDefault();
						if (identity == null)
							throw new Exception($"identity {lastName}, {firstName} not found");
						SafeListContext.Connection.Delete(identity);
						var entries = SafeListContext.Connection.Select<IpEntryEntity>().Where(ipEntry => ipEntry.IdentityId == identity.Id);
						LoggerService.Log(entries.ToList(), new List<string>(){ "Id" });
						return 0;
					})
				.Verb("identities")
					.Option("a", "all").IsFlag()
					.Option("lx", "locked").IsFlag()
					.LoggedAction(args =>
					{
						bool all = args.Get<bool>("a", "all");
						bool locked = args.Get<bool>("lx", "locked");

						var identities = SafeListContext.Connection.Select<IdentityEntity>();
						if (locked) identities = identities.Where(i => i.IsLocked).ToList();

						LoggerService.Log(identities.ToList());

						return 0;
					});

		main.Execute(args);

	}
}
