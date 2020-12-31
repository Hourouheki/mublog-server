using Dapper.FluentMap;

namespace Mublog.Server.Infrastructure.Common.Config.Mappings.Dapper
{
    public static class DapperMapper
    {
        public static void Initialize()
        {
            FluentMapper.Initialize(options =>
            {
                options.AddMap(new AccountMap());
                options.AddMap(new CommentMap());
                options.AddMap(new MediaMap());
                options.AddMap(new PostMap());
                options.AddMap(new TransferPostMap());
                options.AddMap(new ProfileMap());
            });
        }
    }
}