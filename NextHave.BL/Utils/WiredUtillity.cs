using NextHave.DAL.Enums;

namespace NextHave.BL.Utils
{
    public static class WiredUtility
    {
        public static bool IsCondition(this InteractionTypes type)
            => type switch
            {
                InteractionTypes.ConditionAtLeastOneConditionTrue or
                InteractionTypes.ConditionDateTime or
                InteractionTypes.ConditionFurniHaveUsers or
                InteractionTypes.ConditionFurniTypeDoesNotMatch or
                InteractionTypes.ConditionFurniTypeMatch or
                InteractionTypes.ConditionHowManyUsersInRoom or
                InteractionTypes.ConditionNegativeHowManyUsers or
                InteractionTypes.ConditionNegativeStatePos or
                InteractionTypes.ConditionNotHasFurniOnFurni or
                InteractionTypes.ConditionStatePos or
                InteractionTypes.ConditionTimeLessThan or
                InteractionTypes.ConditionTimeMoreThan or
                InteractionTypes.ConditionTriggerNotOnFurni or
                InteractionTypes.ConditionHasFurniOnFurni or
                InteractionTypes.ConditionUserNotOnFurni or
                InteractionTypes.ConditionTriggerOnFurni or
                InteractionTypes.ConditionUserHasObject or
                InteractionTypes.ConditionUserHasBadge or
                InteractionTypes.ConditionUserNotHasBadge or
                InteractionTypes.ConditionUserHasEffect or
                InteractionTypes.ConditionUserHasNoEffect or
                InteractionTypes.ConditionUserIsInGroup or
                InteractionTypes.ConditionUserIsNotInGroup or
                InteractionTypes.ConditionUserInTeam or
                InteractionTypes.ConditionUserIsNotInTeam
                    => true,
                _ => false
            };

        public static bool IsWired(this InteractionTypes type)
            => type switch
            {
                InteractionTypes.TriggerLongRepeater or
                InteractionTypes.ActionGiveEnable or
                InteractionTypes.ConditionAtLeastOneConditionTrue or
                InteractionTypes.ActionFreeze or
                InteractionTypes.ActionUserCollision or
                InteractionTypes.ActionGiveScoreNew or
                InteractionTypes.TriggerTimer or
                InteractionTypes.TriggerRoomEnter or
                InteractionTypes.TriggerGameEnd or
                InteractionTypes.TriggerGameStart or
                InteractionTypes.TriggerRepeater or
                InteractionTypes.TriggerOnUserSay or
                InteractionTypes.TriggerScoreAchieved or
                InteractionTypes.TriggerStateChanged or
                InteractionTypes.TriggerWalkOnFurni or
                InteractionTypes.TriggerWalkOffFurni or
                InteractionTypes.TriggerBotWalkToUser or
                InteractionTypes.TriggerBotWalkToFurni or
                InteractionTypes.ActionGiveScore or
                InteractionTypes.ActionPosReset or
                InteractionTypes.ActionMoveRotate or
                InteractionTypes.ActionResetTimer or
                InteractionTypes.ActionShowMessage or
                InteractionTypes.ActionTeleportTo or
                InteractionTypes.ActionToggleState or
                InteractionTypes.ActionKickUser or
                InteractionTypes.ActionGiveReward or
                InteractionTypes.ActionChangeDirection or
                InteractionTypes.ActionFleeUser or
                InteractionTypes.ActionFixRoom or
                InteractionTypes.ActionMuteUser or
                InteractionTypes.ActionBotChangeLook or
                InteractionTypes.ActionBotWalkToFurni or
                InteractionTypes.ActionBotTalkToRoom or
                InteractionTypes.ActionBotTalkToUser or
                InteractionTypes.ActionBotFollowUser or
                InteractionTypes.ActionBotGiveObject or
                InteractionTypes.ActionBotTeleport or
                InteractionTypes.ActionExecuteWiredGroup or
                InteractionTypes.ActionFurniChangeDirection or
                InteractionTypes.ActionRandomEffect or
                InteractionTypes.ConditionFurniHaveUsers or
                InteractionTypes.ConditionStatePos or
                InteractionTypes.ConditionTimeLessThan or
                InteractionTypes.ConditionTimeMoreThan or
                InteractionTypes.ConditionTriggerOnFurni or
                InteractionTypes.ConditionHasFurniOnFurni or
                InteractionTypes.ConditionNotHasFurniOnFurni or
                InteractionTypes.ConditionUserNotHasBadge or
                InteractionTypes.ConditionUserIsInGroup or
                InteractionTypes.ConditionUserIsNotInGroup or
                InteractionTypes.ConditionUserHasEffect or
                InteractionTypes.ConditionUserHasNoEffect or
                InteractionTypes.ConditionUserNotOnFurni or
                InteractionTypes.ConditionNegativeStatePos or
                InteractionTypes.ConditionUserInTeam or
                InteractionTypes.ConditionUserIsNotInTeam or
                InteractionTypes.ConditionTriggerNotOnFurni or
                InteractionTypes.ConditionDateTime or
                InteractionTypes.ConditionUserHasObject or
                InteractionTypes.ConditionFurniTypeMatch or
                InteractionTypes.ConditionFurniTypeDoesNotMatch or
                InteractionTypes.ConditionHowManyUsersInRoom or
                InteractionTypes.ConditionNegativeHowManyUsers or
                InteractionTypes.TriggerCollision or
                InteractionTypes.ActionFollowUser or
                InteractionTypes.ConditionUserHasBadge or
                InteractionTypes.ActionGivePoints or
                InteractionTypes.ActionGiveScoreToTeam or
                InteractionTypes.ActionJoinTeam or
                InteractionTypes.ActionRemoveFromTeam or
                InteractionTypes.SpecialRandom
                    => true,
                _ => false
            };
    }
}