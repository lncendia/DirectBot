using CsvHelper.Configuration;
using DirectBot.Core.DTO;

namespace DirectBot.BLL.Mapper;

public sealed class InstaUserMapper : ClassMap<InstaUser>
{
    public InstaUserMapper()
    {
        Map(m => m.Pk).Name("Pk").Default(0);
        Map(m => m.Username).Name("Username");
    }
}