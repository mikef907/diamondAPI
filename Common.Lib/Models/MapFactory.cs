using AutoMapper;

namespace Common.Lib.Models
{
    public static class MapFactory
    {
        public static IMapper CreateIdentityMapper() => CreateIdentityConfig().CreateMapper();

        public static IMapper CreateGamesMapper() => CreateGamesConfig().CreateMapper();

        public static MapperConfiguration CreateIdentityConfig() =>
            new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<DM.User, EM.User>()
                    .ForMember(e => e.Password, o => { o.Condition(u => !string.IsNullOrEmpty(u.Password)); })
                    .ForMember(e => e.Id, o => { o.Condition(u => u.Id.HasValue); });

                cfg.CreateMap<EM.User, DM.User>()
                    .ForMember(e => e.Password, o => o.Ignore());

                cfg.CreateMap<EM.User, DM.AuthenticateModel>();
            });

        public static MapperConfiguration CreateGamesConfig() =>
            new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<EM.PlayerMatch, DM.PlayerMatch>()
                    .ForPath(e => e.Player.PlayerMatches, o => o.Ignore())
                    .ForPath(e => e.Match.PlayerMatches, o => o.Ignore())
                    .ForMember(e => e.Player, o => o.MapFrom(d => d.Player))
                    .ForMember(e => e.Match, o => o.MapFrom(d => d.Match))
                    .ReverseMap();

                cfg.CreateMap<EM.GameState, DM.GameState>().ReverseMap();

                cfg.CreateMap<EM.Match, DM.Match>()
                    .ForMember(e => e.PlayerMatches, o => o.Ignore())
                    .ReverseMap();

                cfg.CreateMap<EM.Player, DM.Player>()
                    .ForMember(e => e.PlayerMatches, o => o.Ignore())
                    .ReverseMap();

                cfg.CreateMap<EM.GameMove, DM.GameMove>()
                    .ForMember(e => e.GameState, o => o.Ignore())
                    .ReverseMap();
            });

    }
}
