using RogueGenesia.Data;

namespace OwO.Talents;

public class GenderRevealStone : PassiveTalent
{
    public override string GetName()
    {
        return OwOMod.Owofy(OwOMod.ProcessAvatars("Gender Reveal Stone"));
    }

    public override string GetDescription()
    {
        return OwOMod.Owofy(OwOMod.ProcessAvatars("<color='red'>The gale spread</color> <color='orange'>the fire, and</color><color='yellow'> the Gods " +
                                                  "of</color><color='green'> the wild were</color><color='lightblue'> not pleased. 'It's</color>" +
                                                  "<color='blue'> a Rog!' the</color><color='purple'> people shouted.</color>\n" +
                                                  "\n" +
                                                  "In a more evolved society it would have selected a random avatar on run start, " +
                                                  "but we can't have nice things." +
                                                  "\n\n" +
                                                  "<color='#fcd1d1'><i>And Plexus said it doesn't fit the lore...</i></color>"));
    }
}