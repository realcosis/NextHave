using NextHave.BL.Models.Groups;
using NextHave.DAL.MySQL.Entities;

namespace NextHave.BL.Mappers
{
    internal static class GroupsMappers
    {
        internal static Group Map(this GroupEntity entity)
            => entity.GetMap<GroupEntity, Group>((dest => dest.MemberCount, src => src.Members.Where(m => !m.IsPending).Count()),
                (dest => dest.RequestCount, src => src.Members.Where(m => m.IsPending).Count()),
                (dest => dest.Members, src => src.Members.Where(m => !m.IsPending).ToDictionary(k => k.UserId, v => v.Map())),
                (dest => dest.Requests, src => src.Members.Where(m => m.IsPending).ToDictionary(k => k.UserId, v => v.Map())));

        internal static GroupMember Map(this GroupMembershipEntity entity)
            => entity.GetMap<GroupMembershipEntity, GroupMember>((dest => dest.EnteredAt, src => src.JoinedAt!),
                (dest => dest.Username!, src => src.User!.Username!),
                (dest => dest.Look!, src => src.User!.Look!),
                (dest => dest.UserId!, src => src.User!.Id!));

        internal static GroupElement Map(this GroupElementEntity entity)
            => entity.GetMap<GroupElementEntity, GroupElement>();
    }
}